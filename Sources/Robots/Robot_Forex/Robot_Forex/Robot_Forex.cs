using System;
using cAlgo.API;

namespace cAlgo.Robots
{
    [Robot("Robot Forex", AccessRights = AccessRights.None)]
    public class Robot_Forex : Robot
    {
        [Parameter(DefaultValue = 10000, MinValue = 10000)]
        public int FirstLot { get; set; }

        [Parameter("Take_Profit", DefaultValue = 180, MinValue = 10)]
        public int TakeProfit { get; set; }

        [Parameter("Tral_Start", DefaultValue = 50)]
        public int Tral_Start { get; set; }

        [Parameter("Tral_Stop", DefaultValue = 50)]
        public int Tral_Stop { get; set; }

        [Parameter(DefaultValue = 300)]
        public int PipStep { get; set; }

        [Parameter(DefaultValue = 5, MinValue = 2)]
        public int MaxOrders { get; set; }

        private Position position;
        private bool RobotStopped;
        private int LotStep = 10000;

        protected override void OnStart()
        {

        }

        protected override void OnTick()
        {
            double Bid = Symbol.Bid;
            double Ask = Symbol.Ask;
            double Point = Symbol.PointSize;

            if (Trade.IsExecuting)
                return;
            if (Account.Positions.Count > 0 && RobotStopped)
                return;
            else
                RobotStopped = false;

            if (Account.Positions.Count == 0)
                SendFirstOrder(FirstLot);
            else
                ControlSeries();

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
                        Trade.CreateBuyMarketOrder(Symbol, OrderVolume);
                        break;
                    case 1:
                        Trade.CreateSellMarketOrder(Symbol, OrderVolume);
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
                Result = AveragePrice / Count;
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
                _pipstep = PipStep;

            if (Account.Positions.Count < MaxOrders)
                switch (GetPositionsSide())
                {
                    case 0:
                        if (Symbol.Ask < FindLastPrice(TradeType.Buy) - _pipstep * Symbol.PointSize)
                        {
                            NewVolume = Math.DivRem((int)(FirstLot + FirstLot * Account.Positions.Count), LotStep, out Rem) * LotStep;
                            if (!(NewVolume < LotStep))
                                Trade.CreateBuyMarketOrder(Symbol, NewVolume);
                        }
                        break;
                    case 1:
                        if (Symbol.Bid > FindLastPrice(TradeType.Sell) + _pipstep * Symbol.PointSize)
                        {
                            NewVolume = Math.DivRem((int)(FirstLot + FirstLot * Account.Positions.Count), LotStep, out Rem) * LotStep;
                            if (!(NewVolume < LotStep))
                                Trade.CreateSellMarketOrder(Symbol, NewVolume);
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

        private int GetStdIlanSignal()
        {
            int Result = -1;
            int LastBarIndex = MarketSeries.Close.Count - 2;
            int PrevBarIndex = LastBarIndex - 1;

            if (MarketSeries.Close[LastBarIndex] > MarketSeries.Open[LastBarIndex])
                if (MarketSeries.Close[PrevBarIndex] > MarketSeries.Open[PrevBarIndex])
                    Result = 0;
            if (MarketSeries.Close[LastBarIndex] < MarketSeries.Open[LastBarIndex])
                if (MarketSeries.Close[PrevBarIndex] < MarketSeries.Open[PrevBarIndex])
                    Result = 1;
            return Result;
        }
    }
}
