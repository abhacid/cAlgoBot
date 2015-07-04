using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Requests;

namespace cAlgo.Robots
{
    [Robot("Forex")]
    public class Forex : Robot
    {
        private const double BalancePercent = 0.50;
        private const int LotStep = 10000;
        private Position _position;
        private double _startingBalance;


        [Parameter(DefaultValue = "Forex")]
        public string LabelName { get; set; }

        [Parameter(DefaultValue = 10000)]
        public int FirstLot { get; set; }


        [Parameter("Take_Profit", DefaultValue = 300)]
        public int TakeProfit { get; set; }


        [Parameter("Tral_Start", DefaultValue = 50)]
        public int TralStart { get; set; }


        [Parameter("Tral_Stop", DefaultValue = 50)]
        public int TralStop { get; set; }


        [Parameter(DefaultValue = 500)]
        public int PipStep { get; set; }


        [Parameter(DefaultValue = 3, MinValue = 2)]
        public int MaxOrders { get; set; }

        protected int PositionsCount
        {
            get { return Account.Positions.Count(position1 => position1.Label == LabelName); }
        }

        protected override void OnStart()
        {
            _startingBalance = Account.Balance;
        }

        protected override void OnTick()
        {
            double bid = Symbol.Bid;
            double ask = Symbol.Ask;
            double point = Symbol.PointSize;
            
            CheckBalance();
            
            if (Trade.IsExecuting) return;

            if (PositionsCount == 0)
                SendFirstOrder(FirstLot);
            else
                ControlSeries();

            foreach (var position1 in Account.Positions.Where(position1 => position1.Label == LabelName))
            {
                if (position1.TradeType == TradeType.Buy)
                {
                    if (bid - GetAveragePrice(TradeType.Buy) >= TralStart*point)
                        if (bid - TralStop*point >= position1.StopLoss)
                            Trade.ModifyPosition(position1, bid - TralStop*point, position1.TakeProfit);
                }
                else
                {
                    if (GetAveragePrice(TradeType.Sell) - ask >= TralStart*point)
                        if (ask + TralStop*point <= position1.StopLoss || position1.StopLoss == 0)
                            Trade.ModifyPosition(position1, ask + TralStop*point, position1.TakeProfit);
                }
            }
        }


        private void CheckBalance()
        {
            if (Account.Equity <= _startingBalance*BalancePercent)
            {
                // if you only want to close the positions of this robot change to:
                // foreach (var pos in Account.Positions).Where(position1 => position1.Label == LabelName)
                foreach (var pos in Account.Positions)
                    Trade.Close(pos);
                Stop();
            }
        }
        
        private void SendFirstOrder(int orderVolume)
        {
            int signal = GetStdIlanSignal();

            if ((signal < 0)) return;
            
            var tradeType = signal == 0 ? TradeType.Buy : TradeType.Sell;
            ExecuteTrade(tradeType, orderVolume);

        }

        protected override void OnPositionOpened(Position openedPosition)
        {
            double? stopLossPrice = null;
            double? takeProfitPrice = null;

            // Checks only the positions opened by this robot
            if (Account.Positions.Count(position1 => position1.Label == LabelName) == 1)
            {
                _position = openedPosition;

                if (_position.TradeType == TradeType.Buy)
                    takeProfitPrice = _position.EntryPrice + TakeProfit*Symbol.PointSize;

                if (_position.TradeType == TradeType.Sell)
                    takeProfitPrice = _position.EntryPrice - TakeProfit*Symbol.PointSize;
            }
            else
                switch (GetPositionsSide())
                {
                    case 0:
                        takeProfitPrice = GetAveragePrice(TradeType.Buy) + TakeProfit*Symbol.PointSize;
                        break;
                    case 1:
                        takeProfitPrice = GetAveragePrice(TradeType.Sell) - TakeProfit*Symbol.PointSize;
                        break;
                }
            // Checks only the positions opened by this robot
            foreach (var position1 in Account.Positions.Where(position1 => position1.Label == LabelName))
            {
                _position = position1;

                if (stopLossPrice != null || takeProfitPrice != null)
                    Trade.ModifyPosition(_position, _position.StopLoss, takeProfitPrice);
            }
        }

        private double GetAveragePrice(TradeType typeOfTrade)
        {
            double result = Symbol.Bid;
            double averagePrice = 0;
            long count = 0;

            foreach (var position1 in Account.Positions.Where(position1 => position1.Label == LabelName && position1.TradeType == typeOfTrade))
            {
                averagePrice += position1.EntryPrice*position1.Volume;
                count += position1.Volume;
            }

            if (averagePrice > 0 && count > 0)
                result = averagePrice/count;

            return result;
        }

        private int GetPositionsSide()
        {
            int result = -1;
            
            int buySide =
                Account.Positions.Count(
                    position1 => position1.Label == LabelName && position1.TradeType == TradeType.Buy);
            int sellSide =
                Account.Positions.Count(
                    position1 => position1.Label == LabelName && position1.TradeType == TradeType.Sell);

            if (buySide == PositionsCount) result = 0;
            if (sellSide == PositionsCount) result = 1;

            return result;
        }



        private void ControlSeries()
        {
            int newVolume, rem;
            const int barCount = 25;
            int del = MaxOrders - 1;

            int pipstep = PipStep == 0 ? GetDynamicPipstep(barCount, del) : PipStep;

            if (PositionsCount < MaxOrders)
                switch (GetPositionsSide())
                {
                    case 0:
                        if (Symbol.Ask < FindLastPrice(TradeType.Buy) - pipstep * Symbol.PointSize)
                        {
                            newVolume = Math.DivRem((FirstLot + FirstLot*PositionsCount), LotStep, out rem)*
                                        LotStep;
                            if (!(newVolume < LotStep))
                                ExecuteTrade(TradeType.Buy, newVolume);
                        }
                        break;
                    case 1:
                        if (Symbol.Bid > FindLastPrice(TradeType.Sell) + pipstep*Symbol.PointSize)
                        {
                            newVolume = Math.DivRem((FirstLot + FirstLot * PositionsCount), LotStep, out rem) *
                                        LotStep;
                            if (!(newVolume < LotStep))
                                ExecuteTrade(TradeType.Sell, newVolume);
                        }
                        break;
                }
        }

        private void ExecuteTrade(TradeType tradeType, int volume)
        {
            Request request = new MarketOrderRequest(tradeType, volume)
                                  {
                                      Label = LabelName
                                  };
            Trade.Send(request);
        }

        private double FindLastPrice(TradeType typeOfTrade)
        {
            double lastPrice = 0;
            foreach (var position1 in Account.Positions.Where(position1 => position1.Label == LabelName && position1.TradeType == typeOfTrade))
            {
                _position = position1;
                if (typeOfTrade == TradeType.Buy)
                {
                    if (lastPrice == 0)
                    {
                        lastPrice = _position.EntryPrice;
                        continue;
                    }
                    if (_position.EntryPrice < lastPrice)
                        lastPrice = _position.EntryPrice;
                }
                else 
                {
                    if (lastPrice == 0)
                    {
                        lastPrice = _position.EntryPrice;
                        continue;
                    }
                    if (_position.EntryPrice > lastPrice)
                        lastPrice = _position.EntryPrice;
                }
            }
            return lastPrice;
        }

        private int GetDynamicPipstep(int countOfBars, int del)
        {
            double highestPrice = 0, lowestPrice = 0;
            int startBar = MarketSeries.Close.Count - 2 - countOfBars;
            int endBar = MarketSeries.Close.Count - 2;

            for (int i = startBar; i < endBar; i++)
            {
                if (highestPrice == 0 && lowestPrice == 0)
                {
                    highestPrice = MarketSeries.High[i];
                    lowestPrice = MarketSeries.Low[i];
                    continue;
                }
                if (MarketSeries.High[i] > highestPrice) highestPrice = MarketSeries.High[i];
                if (MarketSeries.Low[i] < lowestPrice) lowestPrice = MarketSeries.Low[i];
            }
            var result = (int)((highestPrice - lowestPrice) / Symbol.PointSize / del);
            return result;
        }

        private int GetStdIlanSignal()
        {
            int result = -1;
            int lastBarIndex = MarketSeries.Close.Count - 2;
            int prevBarIndex = lastBarIndex - 1;

            if (MarketSeries.Close[lastBarIndex] > MarketSeries.Open[lastBarIndex])
                if (MarketSeries.Close[prevBarIndex] > MarketSeries.Open[prevBarIndex])
                    result = 0;

            if (MarketSeries.Close[lastBarIndex] < MarketSeries.Open[lastBarIndex])
                if (MarketSeries.Close[prevBarIndex] < MarketSeries.Open[prevBarIndex])
                    result = 1;

            return result;
        }
    }
}