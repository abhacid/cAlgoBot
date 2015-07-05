//# reference: ..\Indicators\Hull Moving Average.algo
//# reference: ..\Indicators\BelkhayatePRC.algo

// Golden Dragon v1.3
// Released 20th October 2013
// Created by Craig Stone (except for trailing stops code which is drawn from Spotware's sample code)

using System;
using System.Linq;
using System.IO;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.API.Requests;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC)]
    public class GoldenDragon : Robot
    {
        [Parameter("Dragon Number", DefaultValue = 1, MinValue = 1)]
        public int DragonNumber { get; set; }

        [Parameter("COG Degree", DefaultValue = 3, MinValue = 1, MaxValue = 4)]
        public int cogDegree { get; set; }

        [Parameter("COG Period", DefaultValue = 260, MinValue = 1)]
        public int cogPeriod { get; set; }

        [Parameter("1st Channel Offset", DefaultValue = 1.4, MinValue = 0.1)]
        public double Inner { get; set; }

        [Parameter("2nd Channel Offset", DefaultValue = 2.4, MinValue = 0.1)]
        public double Middle { get; set; }

        [Parameter("3rd Channel Offset", DefaultValue = 3.4, MinValue = 0.1)]
        public double Outer { get; set; }

        [Parameter("COG Trade Biasing", DefaultValue = false)]
        public bool cogBias { get; set; }

        [Parameter("Adaptive Trade Biasing", DefaultValue = false)]
        public bool AdaptiveBias { get; set; }

        [Parameter("Hull Trade Biasing", DefaultValue = true)]
        public bool HullBias { get; set; }

        [Parameter("Hull Period", DefaultValue = 5, MinValue = 1)]
        public int HullPeriod { get; set; }

        [Parameter("ATR Filter 1 Period", DefaultValue = 0, MinValue = 0)]
        public int atr1Period { get; set; }

        [Parameter("ATR Filter 1 MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType atr1Type { get; set; }

        [Parameter("ATR Filter 2 Period", DefaultValue = 0, MinValue = 0)]
        public int atr2Period { get; set; }

        [Parameter("ATR Filter 2 MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType atr2Type { get; set; }

        [Parameter("Long Trades", DefaultValue = 1, MinValue = 0)]
        public int MaxLongTrades { get; set; }

        [Parameter("Short Trades", DefaultValue = 1, MinValue = 0)]
        public int MaxShortTrades { get; set; }

        [Parameter("Buy Wait (bars)", DefaultValue = 20, MinValue = 1)]
        public int BuyWait { get; set; }

        [Parameter("Buy Wait on Loss (bars)", DefaultValue = 260)]
        public int LossDelay { get; set; }

        [Parameter("Stop Loss (pips)", DefaultValue = 100, MinValue = 0)]
        public int StopLoss { get; set; }

        [Parameter("Target Level", DefaultValue = 0, MinValue = -1, MaxValue = 3)]
        public int TargetLevel { get; set; }

        [Parameter("Dynamic Targets", DefaultValue = false)]
        public bool DynamicTargets { get; set; }

        [Parameter("Opening Lot Size", DefaultValue = 10000, MinValue = 1)]
        public int OpeningLotSize { get; set; }

        [Parameter("Maximum Lot Size", DefaultValue = 1000000, MinValue = 0)]
        public int MaxLotSize { get; set; }

        [Parameter("Minimum Profit (pips)", DefaultValue = 1, MinValue = 0)]
        public int MinimumPips { get; set; }

        [Parameter("Maximum Slippage (pips)", DefaultValue = 2, MinValue = 0)]
        public int Slippage { get; set; }

        [Parameter("Entry Window (pips)", DefaultValue = 5, MinValue = 0)]
        public int EntryWindow { get; set; }

        [Parameter("Dynamic Stops", DefaultValue = false)]
        public bool DynamicStops { get; set; }

        [Parameter("Trailing Stops", DefaultValue = false)]
        public bool TrailingStops { get; set; }

        [Parameter("Trailing Stop Trigger (pips)", DefaultValue = 20)]
        public int Trigger { get; set; }

        [Parameter("Trailing Stop Distance (pips)", DefaultValue = 20)]
        public int TrailingStop { get; set; }

        [Parameter("Balance Stop Loss (ratio)", DefaultValue = 0.5, MinValue = 0, MaxValue = 1)]
        public double BalanceStop { get; set; }

        [Parameter("Equity Stop Loss (ratio)", DefaultValue = 0.5, MinValue = 0, MaxValue = 1)]
        public double EquityStop { get; set; }

        [Parameter("Equity Trade Filter", DefaultValue = 0.5, MinValue = 0, MaxValue = 1)]
        public double EquityFilter { get; set; }

        [Parameter("Trade on Fridays", DefaultValue = true)]
        public bool FridayTrading { get; set; }

        [Parameter("Money Management", DefaultValue = true)]
        public bool MoneyManagement { get; set; }

        [Parameter("Martingale Enabled", DefaultValue = true)]
        public bool MartingaleEnabled { get; set; }

        [Parameter("Reversingale Enabled", DefaultValue = false)]
        public bool Reversingale { get; set; }

        [Parameter("Martingale Multiplier", DefaultValue = 2, MinValue = 2, MaxValue = 10)]
        public int MartingaleFactor { get; set; }

        [Parameter("Martingale Recursions", DefaultValue = 2, MinValue = 1, MaxValue = 4)]
        public int MartingaleMax { get; set; }

        [Parameter("Debug Level", DefaultValue = 0, MinValue = 0, MaxValue = 3)]
        public int Debug { get; set; }

        private int LongPositions = 0, ShortPositions = 0, MaxLong = 0, MaxShort = 0;
        private int BuyVolume = 0, TakeProfit = 0, Count = 0, MartingaleActive = 0, Quantity = 0;

        private int t1ix;
        private double t1h3, t1h2, t1h1, t1c0, t1l1, t1l2, t1l3;

        private double bid = 0, ask = 0, spread = 0, pipsize = 0, buystop = 0, sellstop = 0, OpeningBalance = 0, LostPips = 0, BotBalance = 0;

        private bool TradeSafe, BuySafe, SellSafe;
        private bool isTrigerred;

        private string DragonID, desktopFolder, filePath;

        StreamWriter _fileWriter;
        StreamReader _fileReader;

        private Position position;

		private BelkhayatePolynomialRegression cog;
        private HMA hull;
        private AverageTrueRange atr1, atr2;

        protected override void OnStart()
        {
            DragonID = "Golden Dragon " + Symbol.Code + "-" + DragonNumber;

            Count = BuyWait;
            BuyVolume = OpeningLotSize;
            Quantity = BuyVolume;
            OpeningBalance = Account.Balance;

            MaxLong = MaxLongTrades;
            MaxShort = MaxShortTrades;

			cog = Indicators.GetIndicator<BelkhayatePolynomialRegression>(cogDegree, cogPeriod, Inner, Middle, Outer);
            hull = Indicators.GetIndicator<HMA>(HullPeriod);

            if (atr1Period > 0 && atr2Period > 0)
            {
                atr1 = Indicators.AverageTrueRange(atr1Period, atr1Type);
                atr2 = Indicators.AverageTrueRange(atr2Period, atr2Type);
            }

            Message(0, "Dragon awakening...");

            foreach (var position in Account.Positions)
            {
                if (position.Label == DragonID)
                {
                    BotBalance += position.GrossProfit;
                    switch (position.TradeType)
                    {
                        case TradeType.Buy:
                            LongPositions++;
                            break;
                        case TradeType.Sell:
                            ShortPositions++;
                            break;
                    }
                }
            }

            if (LongPositions > 0 || ShortPositions > 0)
                Message(0, "Found " + LongPositions + " half-eaten eagle(s) and " + ShortPositions + " rotting sheep");
            else
                Message(0, "No open trades found");

            ChartRefresh();

            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DragonID + ".txt");

            if (MartingaleEnabled && File.Exists(filePath))
            {
                _fileReader = File.OpenText(filePath);
                MartingaleActive = Int32.Parse(_fileReader.ReadLine());
                Message(0, "Martingale Level : " + MartingaleActive);
                _fileReader.Close();
            }
        }

        protected override void OnBar()
        {
            Count++;

            Message(2, "Buy Wait " + Count + ", Long " + LongPositions + " of " + MaxLong + ", Short " + ShortPositions + " of " + MaxShort + ", Martingale Level " + MartingaleActive + ", TradeSafe " + TradeSafe + ", BuySafe " + BuySafe + ", SellSafe " + SellSafe);

            // COGx1
            t1ix = (int)cog.ix;
            t1h3 = (double)cog.sqh3.LastValue;
            t1h2 = (double)cog.sqh2.LastValue;
            t1h1 = (double)cog.sqh.LastValue;
            t1c0 = (double)cog.prc.LastValue;
            t1l1 = (double)cog.sql.LastValue;
            t1l2 = (double)cog.sql2.LastValue;
            t1l3 = (double)cog.sql3.LastValue;

            if (DynamicTargets)
            {
                foreach (var position in Account.Positions)
                {
                    if ((position.Label == DragonID) && (Math.Abs((sbyte)(position.TakeProfit - position.EntryPrice) / pipsize) > MinimumPips))
                        switch (position.TradeType)
                        {
                            case TradeType.Buy:
                                switch (TargetLevel)
                                {
                                    case -1:
                                        Trade.ModifyPosition(position, position.StopLoss, (t1l1 - ask));
                                        break;
                                    case 0:
                                        Trade.ModifyPosition(position, position.StopLoss, (t1c0 - ask));
                                        break;
                                    case 1:
                                        Trade.ModifyPosition(position, position.StopLoss, (t1h1 - ask));
                                        break;
                                    case 2:
                                        Trade.ModifyPosition(position, position.StopLoss, (t1h2 - ask));
                                        break;
                                    case 3:
                                        Trade.ModifyPosition(position, position.StopLoss, (t1h3 - ask));
                                        break;
                                }
                                break;
                            case TradeType.Sell:
                                switch (TargetLevel)
                                {
                                    case -1:
                                        Trade.ModifyPosition(position, position.StopLoss, (bid - t1h1));
                                        break;
                                    case 0:
                                        Trade.ModifyPosition(position, position.StopLoss, (bid - t1c0));
                                        break;
                                    case 1:
                                        Trade.ModifyPosition(position, position.StopLoss, (bid - t1l1));
                                        break;
                                    case 2:
                                        Trade.ModifyPosition(position, position.StopLoss, (bid - t1l2));
                                        break;
                                    case 3:
                                        Trade.ModifyPosition(position, position.StopLoss, (bid - t1l3));
                                        break;
                                }
                                break;
                        }
                }
            }

            if (DynamicStops && !TrailingStops)
            {

                foreach (var position in Account.Positions)
                {
                    if (position.Label == DragonID)
                        switch (position.TradeType)
                        {
                            case TradeType.Buy:
                                Trade.ModifyPosition(position, (t1l3 - t1c0 - t1l3), position.TakeProfit);
                                break;
                            case TradeType.Sell:
                                Trade.ModifyPosition(position, (t1h3 + t1h3 - t1c0), position.TakeProfit);
                                break;
                        }
                }

            }


        }


        protected override void OnTick()
        {
            bid = Symbol.Bid;
            ask = Symbol.Ask;
            spread = Symbol.Spread;
            pipsize = Symbol.PipSize;
            TradeSafe = true;
            BuySafe = true;
            SellSafe = true;

            if (Trade.IsExecuting)
            {
                return;
            }

            if (!FridayTrading && Server.Time.DayOfWeek == DayOfWeek.Friday)
            {
                return;
            }

            if (Account.Equity / Account.Balance < EquityStop)
            {
                Message(0, "Equity protection stop triggered. Profitable positions remain open");
                ClosePositions();
                Stop();
                return;
            }

            if (Account.Equity < Account.Balance * BalanceStop)
            {
                Message(0, "Account balance protection triggered. Profitable positions remain open");
                ClosePositions();
                Stop();
                return;
            }

            if (Account.Equity < Account.Balance * EquityFilter)
                TradeSafe = false;

            if (TrailingStops)
                AdjustTrailingStops();

            if (HullBias)
            {
                if (hull.hma.IsRising())
                    SellSafe = false;
                if (hull.hma.IsFalling())
                    BuySafe = false;
            }

            if (atr1Period > 0 && atr2Period > 0 && atr1.Result.LastValue > atr2.Result.LastValue)
                TradeSafe = false;

            if (cogBias)
            {
                if (cog.prc.IsRising())
                    SellSafe = false;
                if (cog.prc.IsFalling())
                    BuySafe = false;
            }


            if (Count > BuyWait && TradeSafe)
            {

                if (LongPositions < MaxLong && BuySafe && ((hull.hma.HasCrossedAbove(cog.sql3, 1) && (ask < (cog.sql3.LastValue + (EntryWindow * pipsize)))) || (hull.hma.HasCrossedAbove(cog.sql2, 1) && (ask < (cog.sql2.LastValue + (EntryWindow * pipsize))))))
                {
                    OpenPosition(TradeType.Buy);
                    Message(1, "Pursuing an eagle...");
                }

                if (ShortPositions < MaxShort && SellSafe && ((hull.hma.HasCrossedBelow(cog.sqh3, 1) && (bid < (cog.sqh3.LastValue - (EntryWindow * pipsize)))) || (hull.hma.HasCrossedBelow(cog.sqh2, 1) && (bid > (cog.sqh2.LastValue - (EntryWindow * pipsize))))))
                {
                    OpenPosition(TradeType.Sell);
                    Message(1, "Swooping on a sheep...");
                }
            }

            ChartRefresh();

        }

        private void OpenPosition(TradeType tradetype)
        {
            if (Trade.IsExecuting || Count < BuyWait)
            {
                return;
            }

            switch (tradetype)
            {
                case TradeType.Buy:
                    switch (TargetLevel)
                    {
                        case -1:
                            TakeProfit = (int)(Math.Abs(t1l1 - ask) / pipsize);
                            break;
                        case 0:
                            TakeProfit = (int)(Math.Abs(t1c0 - ask) / pipsize);
                            break;
                        case 1:
                            TakeProfit = (int)(Math.Abs(t1h1 - ask) / pipsize);
                            break;
                        case 2:
                            TakeProfit = (int)(Math.Abs(t1h2 - ask) / pipsize);
                            break;
                        case 3:
                            TakeProfit = (int)(Math.Abs(t1h3 - ask) / pipsize);
                            break;
                    }
                    break;
                case TradeType.Sell:
                    switch (TargetLevel)
                    {
                        case -1:
                            TakeProfit = (int)(Math.Abs(bid - t1h1) / pipsize);
                            break;
                        case 0:
                            TakeProfit = (int)(Math.Abs(bid - t1c0) / pipsize);
                            break;
                        case 1:
                            TakeProfit = (int)(Math.Abs(bid - t1l1) / pipsize);
                            break;
                        case 2:
                            TakeProfit = (int)(Math.Abs(bid - t1l2) / pipsize);
                            break;
                        case 3:
                            TakeProfit = (int)(Math.Abs(bid - t1l3) / pipsize);
                            break;
                    }
                    break;
            }

            if (MartingaleEnabled && MartingaleActive > 0)
            {
                TakeProfit = (int)(StopLoss / MartingaleFactor * 1.2);
                Message(1, "St.George sighted...");
            }

            if (MoneyManagement == true && Account.Balance > OpeningBalance)
            {
                Quantity = BuyVolume * (int)(Account.Balance / OpeningBalance);
                Message(3, "Money Management : " + Quantity + "(BV:" + BuyVolume + ")");
            }
            else
            {
                Quantity = BuyVolume;
            }

            if (MaxLotSize > 0 && Quantity > MaxLotSize)
                Quantity = MaxLotSize;

            if (TakeProfit < MinimumPips)
                TakeProfit = MinimumPips;

            Request request = new MarketOrderRequest(tradetype, Quantity) 
            {
                Label = DragonID,
                SlippagePips = Slippage,
                StopLossPips = StopLoss,
                TakeProfitPips = TakeProfit
            };

            Trade.Send(request);
            Count = 0;
        }


        private void ClosePositions()
        {
            foreach (var position in Account.Positions)
            {
                if (position.Label == DragonID)
                {
                    if (position.GrossProfit < 0)
                    {
                        Trade.Close(position);
                        Message(2, "Closing position " + position.Id + " for $" + position.GrossProfit + " loss");
                    }
                }
            }
        }


        protected override void OnPositionOpened(Position openedPosition)
        {
            switch (openedPosition.TradeType)
            {
                case TradeType.Buy:
                    LongPositions++;
                    break;
                case TradeType.Sell:
                    ShortPositions++;
                    break;
            }

            if (MartingaleEnabled && MartingaleActive > 0)
                MartingaleActive--;
        }

        protected override void OnPositionClosed(Position closedPosition)
        {

            switch (closedPosition.TradeType)
            {
                case TradeType.Buy:
                    LongPositions--;
                    break;
                case TradeType.Sell:
                    ShortPositions--;
                    break;
            }


            if (closedPosition.GrossProfit < 1)
            {
                Message(1, "Attacked by St. George!");

                if (LossDelay > 0)
                {
                    Count = -(LossDelay);
                    Message(1, "Dragon is licking his wounds...");
                }

                if (AdaptiveBias == true)
                {
                    switch (closedPosition.TradeType)
                    {
                        case TradeType.Buy:
                            if (MaxLong > 1)
                                MaxLong--;
                            if (MaxShort < MaxShortTrades)
                                MaxShort++;
                            break;

                        case TradeType.Sell:
                            if (MaxShort > 1)
                                MaxShort--;
                            if (MaxLong < MaxLongTrades)
                                MaxLong++;
                            break;
                    }
                }

                if (MartingaleEnabled && MartingaleActive < MartingaleMax)
                {
                    MartingaleActive++;

                    BuyVolume = (int)closedPosition.Volume * MartingaleFactor;

                    if (MaxLotSize > 0 && BuyVolume > MaxLotSize)
                        BuyVolume = MaxLotSize;

                    if (Reversingale)
                    {
                        Message(1, "Taking evasive action");

                        if (closedPosition.TradeType == TradeType.Sell)
                        {
                            TakeProfit = (int)(-closedPosition.Pips / MartingaleFactor * 1.2);
                            if (TakeProfit < MinimumPips)
                                TakeProfit = MinimumPips;
                            Request request = new MarketOrderRequest(TradeType.Buy, BuyVolume) 
                            {
                                Label = DragonID,
                                SlippagePips = Slippage,
                                StopLossPips = StopLoss,
                                TakeProfitPips = TakeProfit
                            };
                            Trade.Send(request);
                        }
                        else
                        {
                            TakeProfit = (int)(-closedPosition.Pips / MartingaleFactor * 1.2);
                            if (TakeProfit < MinimumPips)
                                TakeProfit = MinimumPips;
                            Request request = new MarketOrderRequest(TradeType.Sell, BuyVolume) 
                            {
                                Label = DragonID,
                                SlippagePips = Slippage,
                                StopLossPips = StopLoss,
                                TakeProfitPips = TakeProfit
                            };
                            Trade.Send(request);
                        }
                        Message(2, "Reversingale Stop. Entry " + closedPosition.EntryPrice + ", Loss " + closedPosition.GrossProfit + ", New Entry " + Symbol.Ask + ", New Target " + TakeProfit);
                    }
                }

            }
            else
            {
                BuyVolume = OpeningLotSize;

                if (MartingaleActive > 0)
                {
//                    MartingaleActive--;
//                    Message(1, "St. George defeated! Revenge is sweet");
                }
                else
                    Message(1, "Dinner is served!");
            }
        }


        private void AdjustTrailingStops()
        {
            foreach (var position in Account.Positions)
            {
                if (position.Label == DragonID)
                {
                    if (position.TradeType == TradeType.Buy)
                    {
                        double distance = Symbol.Bid - position.EntryPrice;

                        if (distance >= Trigger * Symbol.PipSize)
                        {
                            if (!isTrigerred)
                            {
                                isTrigerred = true;
                                Message(1, "Trailing Stop Loss triggered...");
                            }

                            double newStopLossPrice = Math.Round(Symbol.Bid - TrailingStop * Symbol.PipSize, Symbol.Digits);

                            if (position.StopLoss == null || newStopLossPrice > position.StopLoss)
                            {
                                Trade.ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                            }
                        }
                    }
                    else
                    {
                        double distance = position.EntryPrice - Symbol.Ask;

                        if (distance >= Trigger * Symbol.PipSize)
                        {
                            if (!isTrigerred)
                            {
                                isTrigerred = true;
                                Message(1, "Trailing Stop Loss triggered...");
                            }

                            double newStopLossPrice = Math.Round(Symbol.Ask + TrailingStop * Symbol.PipSize, Symbol.Digits);

                            if (position.StopLoss == null || newStopLossPrice < position.StopLoss)
                            {
                                Trade.ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                            }
                        }
                    }

                }
            }
        }

        protected void ChartRefresh()
        {
            var name = "info";
            var text = DragonID;
            text += "\nBuy " + BuySafe + " / Sell " + SellSafe + " / Trade " + TradeSafe;
            text += "\nActive Lot Size : " + Quantity;
            text += "\nMartingale Level : " + MartingaleActive;
            text += "\nLong / Short : " + LongPositions + " / " + ShortPositions;
            text += "\nBuy Wait : " + (BuyWait - Count).ToString();

            var staticPos = StaticPosition.TopLeft;
            var color = Colors.Gray;
            ChartObjects.DrawText(name, text, staticPos, color);
        }

        protected void Message(int level, string message)
        {
            if (level <= Debug)
                Print("[{0}] {1}", level, message);
        }

        protected override void OnStop()
        {

            if (MartingaleEnabled)
            {
                File.Delete(filePath);
                File.WriteAllText(filePath, MartingaleActive.ToString());
                Message(0, "Saved Martingale Level : " + MartingaleActive);
            }

            Message(0, "Dragon Sleeping");
        }

    }
}
