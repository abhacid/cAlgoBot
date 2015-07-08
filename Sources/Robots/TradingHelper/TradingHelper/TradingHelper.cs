using System;
using System.Threading;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TradingHelper : Robot
    {
        // Перевод в безубыток до установки сейфа
        [Parameter("BreakEven Level 1", DefaultValue = 10, MinValue = 0)]
        public int BreakEven_Level_1 { get; set; }

        // Перевод в безубыток после установки сейфа
        [Parameter("BreakEven Level 2", DefaultValue = 30, MinValue = 0)]
        public int BreakEven_Level_2 { get; set; }

        // Сейф
        [Parameter("Safe Level", DefaultValue = 15)]
        public int SafeLevel { get; set; }

        [Parameter("Safe Fraction", DefaultValue = 0.5, MinValue = 0.1, MaxValue = 0.9)]
        public double SafeFraction { get; set; }

        // Первоначальный стоп
        [Parameter("StopLoss Initial", DefaultValue = 20)]
        public int StopLoss_Initial { get; set; }

        // Шаг изменения StopLoss
        [Parameter("StopLoss Step", DefaultValue = 20)]
        public int StopLoss_Step { get; set; }

        // Траллинг стопа
        [Parameter("StopLoss Tralling", DefaultValue = 30)]
        public int StopLoss_Tralling { get; set; }

        // Устанавливается когда срабатывает сейф
        [Parameter("Take Profit", DefaultValue = 100)]
        public int TakeProfit { get; set; }

        protected override void OnStart()
        {
            Positions.Opened += OnPositionsOpened;
            Timer.Start(1);
        }
        
        private double last = 0;
        private int ticks = 10;
        
        protected override void OnTimer()
        {
            ticks = 0;
            OnTick();
        }
        
        private void OnPositionsOpened(PositionOpenedEventArgs args)
        {
            if (args.Position.SymbolCode == Symbol.Code)
            {
                ticks = 0;
                OnTick();
            }
        }
        
        private double ND(double p)
        {
            return Math.Round(p, Symbol.Digits);
        }
        
        protected override void OnTick()
        {
            if (Monitor.TryEnter(this))
            {
                try
                {
                    DoWork();
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }
        
        private void DoWork()
        {
            if (--ticks > 0 && Math.Abs(last - MarketSeries.Close.LastValue) < Symbol.PipSize / 2)
            {
                return;
            }
            
            RefreshData();
            
            ticks = 10;
            last = MarketSeries.Close.LastValue;

            foreach (Position position in Positions)
            {
                if (position.SymbolCode != Symbol.Code)
                {
                    continue;
                }
                
                double  l_ord_OpenPrice;
                double? l_ord_StopLoss = position.StopLoss;
                double? l_ord_TakeProfit = position.TakeProfit;

                TradeResult result;
                
                switch (position.TradeType)
                {
                case TradeType.Buy:

                    l_ord_OpenPrice = Symbol.Bid - position.Pips * Symbol.PipSize;
                    
                    if (l_ord_StopLoss == null)
                    {
                        result = ModifyPosition(position, ND(l_ord_OpenPrice - StopLoss_Initial * Symbol.PipSize), l_ord_TakeProfit);
                        if (!result.IsSuccessful)
                        {
                            Print("ERROR: Setup SL : {0}", result.Error);
                        }
                    }
                    else if (BreakEven_Level_1 > 0 || StopLoss_Tralling > 0 || BreakEven_Level_2 > 0)
                    {
                        double l_ord_BreakEven_Level_1 = ND(l_ord_OpenPrice + BreakEven_Level_1 * Symbol.PipSize);
                        double l_ord_BreakEven_Level_2 = ND(l_ord_OpenPrice + BreakEven_Level_2 * Symbol.PipSize);
                        double l_ord_BreakEven = ND(l_ord_OpenPrice + 2 * (-position.Commissions / Symbol.PipValue / position.Volume) * Symbol.PipSize);
                        double l_ord_StopLoss_Next = ND((l_ord_StopLoss.Value > l_ord_BreakEven ? l_ord_StopLoss.Value : l_ord_OpenPrice) + StopLoss_Step * Symbol.PipSize);
                        double? l_price = ND(Symbol.Bid - StopLoss_Tralling * Symbol.PipSize);
                        
                        if (StopLoss_Step > 0 && l_price >= l_ord_StopLoss_Next)
                        {
                            l_price = l_ord_StopLoss_Next;
                        }
                        else if (BreakEven_Level_2 > 0 && Symbol.Bid >= l_ord_BreakEven_Level_2)
                        {
                            l_price = l_ord_BreakEven;
                        }
                        else if (BreakEven_Level_1 > 0 && Symbol.Bid >= l_ord_BreakEven_Level_1 && l_ord_TakeProfit == null)
                        {
                            l_price = l_ord_BreakEven;
                        }
                        else
                        {
                            l_price = null;
                        }

                        if (l_price != null && l_price > l_ord_StopLoss)
                        {
                            result = ModifyPosition(position, l_price, l_ord_TakeProfit);
                            if (!result.IsSuccessful)
                            {
                                Print("ERROR: Tralling SL : move SL : {0}", result.Error);
                                break;
                            }
                        }
                    }
                    
                    // Safe

                    if (l_ord_TakeProfit == null && Symbol.Bid - l_ord_OpenPrice >= SafeLevel * Symbol.PipSize)
                    {
                        double netProfit = position.NetProfit;

                        long closing = Symbol.NormalizeVolume(position.Volume * SafeFraction);
                        long remaining = position.Volume - closing;

                        result = ClosePosition(position, closing);
                        if (!result.IsSuccessful)
                        {
                            Print("ERROR: Safe : close {0}/{1} : {2}", closing, remaining, result.Error);
                            break;
                        }

                        double pipsSL = (netProfit - position.NetProfit) / Symbol.PipValue / position.Volume;

                        result = ModifyPosition(position, ND(l_ord_OpenPrice - pipsSL * Symbol.PipSize), ND(l_ord_OpenPrice + TakeProfit * Symbol.PipSize));
                        if (!result.IsSuccessful)
                        {
                            Print("ERROR: Safe : move SL : {0}", result.Error);
                            break;
                        }
                    }
                    
                    break;

                case TradeType.Sell:

                    l_ord_OpenPrice = Symbol.Ask + position.Pips * Symbol.PipSize;
                    
                    if (l_ord_StopLoss == null)
                    {
                        result = ModifyPosition(position, ND(l_ord_OpenPrice + StopLoss_Initial * Symbol.PipSize), position.TakeProfit);
                        if (!result.IsSuccessful)
                        {
                            Print("ERROR: Setup SL : {0}", result.Error);
                        }
                    }
                    else if (BreakEven_Level_1 > 0 || StopLoss_Tralling > 0 || BreakEven_Level_2 > 0)
                    {
                        double l_ord_BreakEven_Level_1 = ND(l_ord_OpenPrice - BreakEven_Level_1 * Symbol.PipSize);
                        double l_ord_BreakEven_Level_2 = ND(l_ord_OpenPrice - BreakEven_Level_2 * Symbol.PipSize);
                        double l_ord_BreakEven = ND(l_ord_OpenPrice - 2 * (-position.Commissions / Symbol.PipValue / position.Volume) * Symbol.PipSize);
                        double l_ord_StopLoss_Next = ND((l_ord_StopLoss.Value < l_ord_BreakEven ? l_ord_StopLoss.Value : l_ord_OpenPrice) - StopLoss_Step * Symbol.PipSize);
                        double? l_price = ND(Symbol.Ask + StopLoss_Tralling * Symbol.PipSize);
                        
                        if (StopLoss_Step > 0 && l_price <= l_ord_StopLoss_Next)
                        {
                            l_price = l_ord_StopLoss_Next;
                        }
                        else if (BreakEven_Level_2 > 0 && Symbol.Ask <= l_ord_BreakEven_Level_2)
                        {
                            l_price = l_ord_BreakEven;
                        }
                        else if (BreakEven_Level_1 > 0 && Symbol.Ask <= l_ord_BreakEven_Level_1 && l_ord_TakeProfit == null)
                        {
                            l_price = l_ord_BreakEven;
                        }
                        else
                        {
                            l_price = null;
                        }

                        if (l_price != null && l_price < l_ord_StopLoss)
                        {
                            result = ModifyPosition(position, l_price, l_ord_TakeProfit);
                            if (!result.IsSuccessful)
                            {
                                Print("ERROR: Tralling SL : move SL : {0}", result.Error);
                                break;
                            }
                        }                   
                    }
                    
                    // Safe

                    if (l_ord_TakeProfit == null && l_ord_OpenPrice - Symbol.Ask >= SafeLevel * Symbol.PipSize)
                    {
                        double netProfit = position.NetProfit;

                        long closing = Symbol.NormalizeVolume(position.Volume * SafeFraction);
                        long remaining = position.Volume - closing;
                        
                        result = ClosePosition(position, closing);
                        if (!result.IsSuccessful)
                        {
                            Print("ERROR: Safe : close {0}/{1} : {2}", closing, remaining, result.Error);
                            break;
                        }

                        double pipsSL = (netProfit - position.NetProfit) / Symbol.PipValue / position.Volume;

                        result = ModifyPosition(position, ND(l_ord_OpenPrice + pipsSL * Symbol.PipSize), ND(l_ord_OpenPrice - TakeProfit * Symbol.PipSize));
                        if (!result.IsSuccessful)
                        {
                            Print("ERROR: Safe : move SL : {0}", result.Error);
                            break;
                        }
                    }
                    
                    break;
                }
            }
        }

        protected override void OnStop()
        {}
    }
}
