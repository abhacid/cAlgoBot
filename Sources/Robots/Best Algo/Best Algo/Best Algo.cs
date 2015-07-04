using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot("AI Forex")]
    public class BestAlgo : Robot
    {
        [Parameter("Percent Balance", DefaultValue = 300, MinValue = 0)]
        public int PercentBalance { get; set; }

        [Parameter("Take Profit", DefaultValue = 1000, MinValue = 0)]
        public int TakeProfit { get; set; }

        [Parameter("Pip Step", DefaultValue = 50)]
        public int PipStep { get; set; }

        [Parameter("Max Orders", DefaultValue = 100, MinValue = 2)]
        public int MaxOrders { get; set; }

        [Parameter("No First Lot", DefaultValue = false)]
        public bool NoFirstLot { get; set; }

        [Parameter("Use MACD?", DefaultValue = true)]
        public bool UseMACD { get; set; }

        [Parameter("MACD LongCycle", DefaultValue = 26, MinValue = 1)]
        public int LongCycle { get; set; }

        [Parameter("MACD ShortCycle", DefaultValue = 12, MinValue = 1)]
        public int ShortCycle { get; set; }

        [Parameter("MACD Period", DefaultValue = 9, MinValue = 1)]
        public int MACDPeriod { get; set; }

        [Parameter("Use EMA?", DefaultValue = true)]
        public bool UseEMA { get; set; }

        [Parameter("EMA FastPeriods", DefaultValue = 4, MinValue = 1)]
        public int FastPeriods { get; set; }

        [Parameter("EMA SlowPeriods", DefaultValue = 12, MinValue = 1)]
        public int SlowPeriods { get; set; }

        [Parameter("Use RSI?", DefaultValue = true)]
        public bool UseRSI { get; set; }

        [Parameter("RSI Periods", DefaultValue = 9, MinValue = 1)]
        public int RSIPeriods { get; set; }

        [Parameter("Use STO?", DefaultValue = true)]
        public bool UseSTO { get; set; }

        [Parameter("STO %K Periods", DefaultValue = 12, MinValue = 1)]
        public int KPeriods { get; set; }

        [Parameter("STO %K Slowing", DefaultValue = 3, MinValue = 1)]
        public int KSlowing { get; set; }

        [Parameter("STO %D Periods", DefaultValue = 4, MinValue = 1)]
        public int DPeriods { get; set; }

        private bool Sellingongoing = false;
        private bool Buyingongoing = false;

        private int FirstLot = 1000;
        private int LotStep = 1000;
        private int PipStart;
        private int Tral_Start = 50;
        private int Tral_Stop = 50;

        private int PipMulti;

        private Position position;
        private bool RobotStopped;

        private StochasticOscillator _SOC;
        private RelativeStrengthIndex _RSI;
        private MacdCrossOver _MACD;

        private ExponentialMovingAverage slowMa;
        private ExponentialMovingAverage fastMa;
        private const string label = "Secret to Rich";

        private int MaxCout = 0;
        private double MinMargin = 99999999;
        private double MinEquity = 99999999;
        private double EquityBalance = 99999999;
        long TotalBuy = 0;

        protected override void OnStart()
        {
            _SOC = Indicators.StochasticOscillator(KPeriods, KSlowing, DPeriods, MovingAverageType.Simple);
            _MACD = Indicators.MacdCrossOver(LongCycle, ShortCycle, MACDPeriod);

            _RSI = Indicators.RelativeStrengthIndex(MarketSeries.Close, RSIPeriods);

            fastMa = Indicators.ExponentialMovingAverage(MarketSeries.Close, FastPeriods);
            slowMa = Indicators.ExponentialMovingAverage(MarketSeries.Close, SlowPeriods);

            TakeProfit = TakeProfit * 10;
            PipStart = PipStep / 10;
            PipStep = PipStep * 10;

            PipMulti = PipStep / 100;
        }

        protected override void OnTick()
        {
            double Bid = Symbol.Bid;
            double Ask = Symbol.Ask;
            double Point = Symbol.PointSize;

            if (Account.Positions.Count > MaxCout)
            {
                MaxCout = Account.Positions.Count;
            }

            if (Trade.IsExecuting)
                return;
            if (Account.Positions.Count > 0 && RobotStopped)
                return;
            else
                RobotStopped = false;

            if (Account.Positions.Count == 0)
            {

                int intValue = (int)(Account.Balance * PercentBalance / 100);
                FirstLot = (int)((intValue / 1000) * 1000);

                if (FirstLot < 1000)
                {
                    FirstLot = 1000;
                }

                LotStep = FirstLot;

                if (!NoFirstLot)
                {
                    SendFirstOrder(FirstLot);
                }
            }
            else
            {
                ControlSeries();
            }

            if (Account.Equity > Account.Balance)
                switch (GetPositionsSide())
                {
                    case 0:
                        if (GetStdIlanSignal() == 0)
                        {
                            foreach (var po in Positions)
                            {
                                ClosePosition(po);
                            }
                        }
                        break;
                    case 1:
                        if (GetStdIlanSignal() == 1)
                        {
                            foreach (var po in Positions)
                            {
                                ClosePosition(po);
                            }
                        }
                        break;
                }


            foreach (var position in Account.Positions)
            {
                if (position.SymbolCode == Symbol.Code)
                {

                    if (position.TradeType == TradeType.Buy)
                    {
                        if (Bid - GetAveragePrice(TradeType.Buy) >= Tral_Start * Point)
                            if (Bid - Tral_Stop * Point >= position.StopLoss)
                                Trade.ModifyPosition(position, Bid - Tral_Stop * Point, position.TakeProfit);
                    }

                    if (position.TradeType == TradeType.Sell)
                    {
                        if (GetAveragePrice(TradeType.Sell) - Ask >= Tral_Start * Point)
                            if (Ask + Tral_Stop * Point <= position.StopLoss || position.StopLoss == 0)
                                Trade.ModifyPosition(position, Ask + Tral_Stop * Point, position.TakeProfit);
                    }
                }

                if (Account.Equity < MinEquity)
                {
                    MinEquity = Account.Equity;
                    EquityBalance = Account.Equity * 100 / Account.Balance;
                    MinMargin = (double)Account.MarginLevel;
                    if (MinEquity <= 0 || MinMargin <= 0)
                    {
                        this.Stop();
                    }
                }
            }
        }

        protected override void OnError(Error CodeOfError)
        {
            if (CodeOfError.Code == ErrorCode.NoMoney)
            {
                RobotStopped = true;
                Print("ERROR!!! No money for order open, robot is stopped!");
            }
            else if (CodeOfError.Code == ErrorCode.BadVolume)
            {
                RobotStopped = true;
                Print("ERROR!!! Bad volume for order open, robot is stopped!");
            }
        }

        private void SendFirstOrder(int OrderVolume)
        {
            int Signal = GetStdIlanSignal();
            if (!(Signal < 0))
                switch (Signal)
                {
                    case 0:
                        Trade.CreateSellMarketOrder(Symbol, OrderVolume);
                        TotalBuy += OrderVolume;
                        break;
                    case 1:
                        Trade.CreateBuyMarketOrder(Symbol, OrderVolume);
                        TotalBuy += OrderVolume;
                        break;
                }

        }

        protected override void OnPositionOpened(Position openedPosition)
        {
            double? StopLossPrice = null;
            double? TakeProfitPrice = null;

            if (Account.Positions.Count == 1)
            {
                position = openedPosition;
                if (position.TradeType == TradeType.Buy)
                    TakeProfitPrice = position.EntryPrice + TakeProfit * Symbol.PointSize;
                if (position.TradeType == TradeType.Sell)
                    TakeProfitPrice = position.EntryPrice - TakeProfit * Symbol.PointSize;
            }
            else
                switch (GetPositionsSide())
                {
                    case 0:
                        TakeProfitPrice = GetAveragePrice(TradeType.Buy) + TakeProfit * Symbol.PointSize;
                        break;
                    case 1:
                        TakeProfitPrice = GetAveragePrice(TradeType.Sell) - TakeProfit * Symbol.PointSize;
                        break;
                }

            for (int i = 0; i < Account.Positions.Count; i++)
            {
                position = Account.Positions[i];
                if (StopLossPrice != null || TakeProfitPrice != null)
                    Trade.ModifyPosition(position, position.StopLoss, TakeProfitPrice);
            }
        }

        private double GetAveragePrice(TradeType TypeOfTrade)
        {
            double Result = Symbol.Bid;
            double AveragePrice = 0;
            long Count = 0;

            for (int i = 0; i < Account.Positions.Count; i++)
            {
                position = Account.Positions[i];
                if (position.TradeType == TypeOfTrade)
                {
                    AveragePrice += position.EntryPrice * position.Volume;
                    Count += position.Volume;
                }
            }
            if (AveragePrice > 0 && Count > 0)
                Result = (AveragePrice / Count);
            return Result;
        }

        private int GetPositionsSide()
        {
            int Result = -1;
            int i, BuySide = 0, SellSide = 0;

            for (i = 0; i < Account.Positions.Count; i++)
            {
                if (Account.Positions[i].TradeType == TradeType.Buy)
                    BuySide++;
                if (Account.Positions[i].TradeType == TradeType.Sell)
                    SellSide++;
            }
            if (BuySide == Account.Positions.Count)
                Result = 0;
            if (SellSide == Account.Positions.Count)
                Result = 1;
            return Result;
        }

        private void ControlSeries()
        {
            int _pipstep, NewVolume, Rem;
            int BarCount = 25;
            int Del = MaxOrders - 1;

            if (PipStep == 0)
                _pipstep = GetDynamicPipstep(BarCount, Del);
            else
            {
                _pipstep = PipStep;
                //_pipstep = (PipStep * PipStart * (Account.Positions.Count) / PipMulti);
            }



            if (Account.Positions.Count < MaxOrders)
                switch (GetPositionsSide())
                {
                    case 0:
                        if (Symbol.Ask < FindLastPrice(TradeType.Buy) - _pipstep * Symbol.PointSize)
                        {
                            NewVolume = Math.DivRem((int)(FirstLot + FirstLot * Account.Positions.Count), LotStep, out Rem) * LotStep;
                            if (!(NewVolume < LotStep) && GetStdIlanSignal() == 1)
                            {
                                Trade.CreateBuyMarketOrder(Symbol, NewVolume);
                                TotalBuy += NewVolume;
                            }
                        }
                        break;
                    case 1:
                        if (Symbol.Bid > FindLastPrice(TradeType.Sell) + _pipstep * Symbol.PointSize)
                        {
                            NewVolume = Math.DivRem((int)(FirstLot + FirstLot * Account.Positions.Count), LotStep, out Rem) * LotStep;
                            if (!(NewVolume < LotStep) && GetStdIlanSignal() == 0)
                            {
                                Trade.CreateSellMarketOrder(Symbol, NewVolume);
                                TotalBuy += NewVolume;
                            }
                        }
                        break;
                }
        }

        private int GetDynamicPipstep(int CountOfBars, int Del)
        {
            int Result;
            double HighestPrice = 0, LowestPrice = 0;
            int StartBar = MarketSeries.Close.Count - 2 - CountOfBars;
            int EndBar = MarketSeries.Close.Count - 2;

            for (int i = StartBar; i < EndBar; i++)
            {
                if (HighestPrice == 0 && LowestPrice == 0)
                {
                    HighestPrice = MarketSeries.High[i];
                    LowestPrice = MarketSeries.Low[i];
                    continue;
                }
                if (MarketSeries.High[i] > HighestPrice)
                    HighestPrice = MarketSeries.High[i];
                if (MarketSeries.Low[i] < LowestPrice)
                    LowestPrice = MarketSeries.Low[i];
            }
            Result = (int)((HighestPrice - LowestPrice) / Symbol.PointSize / Del);
            return Result;
        }

        private double FindLastPrice(TradeType TypeOfTrade)
        {
            double LastPrice = 0;

            for (int i = 0; i < Account.Positions.Count; i++)
            {
                position = Account.Positions[i];
                if (TypeOfTrade == TradeType.Buy)
                    if (position.TradeType == TypeOfTrade)
                    {
                        if (LastPrice == 0)
                        {
                            LastPrice = position.EntryPrice;
                            continue;
                        }
                        if (position.EntryPrice < LastPrice)
                            LastPrice = position.EntryPrice;
                    }
                if (TypeOfTrade == TradeType.Sell)
                    if (position.TradeType == TypeOfTrade)
                    {
                        if (LastPrice == 0)
                        {
                            LastPrice = position.EntryPrice;
                            continue;
                        }
                        if (position.EntryPrice > LastPrice)
                            LastPrice = position.EntryPrice;
                    }
            }
            return LastPrice;
        }


        protected override void OnStop()
        {
            Print("Min Equity = " + MinEquity);
            Print("Min Margin Level = " + MinMargin);
            Print("Equity/Balance Percent = " + EquityBalance);
            Print("MaxCout = " + MaxCout);
            Print("TotalBuy = " + TotalBuy);
        }

        private int GetStdIlanSignal()
        {
            int Result = -1;
            int Result1 = 0;
            int Result2 = 0;
            int Result3 = 0;
            int Result4 = 0;
            int Result5 = 0;
            int sum = 0;
            //1 = buy ,  result = 1
            //0 = sell , result = -1


            if (UseMACD)
            {
                if (_MACD.MACD.Last(1) < _MACD.Signal.Last(1) && _MACD.MACD.Last(0) > _MACD.Signal.Last(0) && _MACD.Signal.Last(0) < 0 && _MACD.Signal.IsRising())
                {
                    Result2 = 1;
                }

                if (_MACD.MACD.Last(1) > _MACD.Signal.Last(1) && _MACD.MACD.Last(0) < _MACD.Signal.Last(0) && _MACD.Signal.Last(0) > 0 && _MACD.Signal.IsFalling())
                {
                    Result2 = -1;
                }
            }

            if (UseEMA)
            {
                //Moving Average
                var currentSlowMa = slowMa.Result.Last(0);
                var currentFastMa = fastMa.Result.Last(0);
                var previousSlowMa = slowMa.Result.Last(1);
                var previousFastMa = fastMa.Result.Last(1);

                if (previousSlowMa > previousFastMa && currentSlowMa <= currentFastMa)
                {
                    Result3 = 1;
                }
                else if (previousSlowMa < previousFastMa && currentSlowMa >= currentFastMa)
                {
                    Result3 = -1;
                }
            }

            if (UseRSI)
            {
                //RSI
                var currentRSI = _RSI.Result.Last(0);
                var previousRSI = _RSI.Result.Last(1);
                if (currentRSI <= 70 && previousRSI > 70)
                {
                    Result4 = -1;
                }
                else if (currentRSI >= 30 && previousRSI < 30)
                {
                    Result4 = 1;
                }
            }

            if (UseSTO)
            {
                //StochasticOscillator
                var currentD = _SOC.PercentD.Last(0);
                var currentK = _SOC.PercentK.Last(0);
                var previousD = _SOC.PercentD.Last(1);
                var previousK = _SOC.PercentK.Last(1);

                if (currentK >= currentD && previousK < previousD && currentK < 20)
                {
                    Result5 = 1;
                    //Print("currentK=" + currentK + " >= currentD=" + currentD + "previousK=" + previousK + " < previousD=" + previousD);
                }
                else if (currentK <= currentD && previousK > previousD && currentK > 80)
                {
                    Result5 = -1;
                    //Print("currentK=" + currentK + " <= currentD=" + currentD + "previousK=" + previousK + " > previousD=" + previousD);
                }
            }

            //Summary of indicator 
            sum = Result2 + Result3 + Result4 + Result5;
            if (sum > 0)
            {
                Result = 1;
                //Print("BUY ! [MACD= " + Result2 + "] [EMA= " + Result3 + "] [RSI= " + Result4 + "] [SOC= " + Result5 + "]");
            }
            else if (sum < 0)
            {
                Result = 0;
                //Print("SELL ![MACD= " + Result2 + "] [EMA= " + Result3 + "] [RSI= " + Result4 + "] [SOC= " + Result5 + "]");
            }
            else
            {
                Result = -1;
            }

            return Result;
        }


    }
}
