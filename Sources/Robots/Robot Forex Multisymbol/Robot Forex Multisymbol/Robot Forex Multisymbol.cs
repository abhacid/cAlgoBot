using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot("Robot Forex", AccessRights = AccessRights.None)]
    public class RobotForexMS : Robot
    {
        // Declare extra symbol, series
        private MarketSeries _series2;
        private Symbol _symbol2;

        // Add extra symbols as input parameters
        [Parameter(DefaultValue = "GBPUSD")]
        public string Symbol2 { get; set; }

        [Parameter(DefaultValue = "RobotForexMS")]
        public string RobotLabel { get; set; }

        [Parameter(DefaultValue = 10000, MinValue = 10000)]
        public int FirstLot { get; set; }

        [Parameter(DefaultValue = 10000, MinValue = 0)]
        public int LotStep { get; set; }

        [Parameter("Take_Profit", DefaultValue = 180, MinValue = 10)]
        public int TakeProfit { get; set; }

        [Parameter("Tral_Start", DefaultValue = 50)]
        public int TraiStart { get; set; }

        [Parameter("TraiStop", DefaultValue = 50)]
        public int TraiStop { get; set; }

        [Parameter(DefaultValue = 300)]
        public int PipStep { get; set; }

        [Parameter(DefaultValue = 5, MinValue = 2)]
        public int MaxOrders { get; set; }

        protected override void OnStart()
        {
            // Initialize extra symbols, series 
            InitializeSeries(Symbol2);
        }

        private void InitializeSeries(string symbolCode)
        {
            _symbol2 = MarketData.GetSymbol(symbolCode);

            _series2 = MarketData.GetSeries(_symbol2, TimeFrame);
        }


        protected override void OnTick()
        {
            // Execute each symbol 
            Execute(MarketSeries, Symbol);  // chart symbol
            Execute(_series2, _symbol2);    // additional symbol
        }

        private void Execute(MarketSeries marketSeries, Symbol symbol)
        {
            Position[] positions = Positions.FindAll(RobotLabel, symbol);

            if (positions.Length == 0)
                SendFirstOrder(FirstLot, marketSeries, symbol);
            else
                ControlSeries(marketSeries, symbol);

            TrailPositions(symbol);
        }

        private void TrailPositions(Symbol symbol)
        {
            double bid = symbol.Bid;
            double ask = symbol.Ask;
            double pipSize = symbol.PipSize;

            Position[] positions = Positions.FindAll(RobotLabel, symbol);
            foreach (Position position in positions)
            {
                if (position.TradeType == TradeType.Buy)
                {
                    if (bid - GetAveragePrice(TradeType.Buy, symbol) >= TraiStart*pipSize &&
                        bid - TraiStop*pipSize >= position.StopLoss || position.StopLoss == null)
                        ModifyPosition(position, bid - TraiStop*pipSize, position.TakeProfit);
                }
                else
                {
                    if (GetAveragePrice(TradeType.Sell, symbol) - ask >= TraiStart*pipSize &&
                        (ask + TraiStop*pipSize <= position.StopLoss || position.StopLoss == null))
                        ModifyPosition(position, ask + TraiStop*pipSize, position.TakeProfit);
                }
            }
        }

        private void ControlSeries(MarketSeries marketSeries, Symbol symbol)
        {
            int pipstep = PipStep == 0 ? GetDynamicPipstep(marketSeries, symbol) : PipStep;
            Position[] positions = Positions.FindAll(RobotLabel, symbol);

            if (positions.Length < MaxOrders)
            {
                int lotStep = LotStep == 0 ? (int) symbol.VolumeStep : LotStep;
                var newVolume = (int) (Math.Truncate((double) ((FirstLot + FirstLot*positions.Length)/lotStep))*lotStep);
                switch (GetPositionsSide(symbol))
                {
                    case 0:
                        if (symbol.Ask < FindLastPrice(TradeType.Buy, symbol) - pipstep*symbol.PipSize)
                        {
                            if (!(newVolume < lotStep))
                            {
                                TradeResult result = ExecuteMarketOrder(TradeType.Buy, symbol, newVolume, RobotLabel,
                                                                        null, TakeProfit);
                                if (!result.IsSuccessful)
                                    if (result.Error.Equals(ErrorCode.NoMoney) ||
                                        result.Error.Equals(ErrorCode.BadVolume))
                                        Stop();
                            }
                        }
                        break;
                    case 1:
                        if (symbol.Bid > FindLastPrice(TradeType.Sell, symbol) + pipstep*symbol.PipSize)
                        {
                            if (!(newVolume < lotStep))
                            {
                                TradeResult result = ExecuteMarketOrder(TradeType.Sell, symbol, newVolume, RobotLabel,
                                                                        null, TakeProfit);
                                if (!result.IsSuccessful)
                                    if (result.Error.Equals(ErrorCode.NoMoney) ||
                                        result.Error.Equals(ErrorCode.BadVolume))
                                        Stop();
                            }
                        }
                        break;
                }
            }
        }

        private void SendFirstOrder(int orderVolume, MarketSeries marketSeries, Symbol symbol)
        {
            int signal = GetStdIlanSignal(marketSeries);

            if (!(signal < 0))
            {
                TradeResult result;
                switch (signal)
                {
                    case 0:
                        result = ExecuteMarketOrder(TradeType.Buy, symbol, orderVolume, RobotLabel, null, TakeProfit);
                        if (!result.IsSuccessful)
                            if (result.Error.Equals(ErrorCode.NoMoney) ||
                                result.Error.Equals(ErrorCode.BadVolume))
                                Stop();

                        break;

                    case 1:
                        result = ExecuteMarketOrder(TradeType.Sell, symbol, orderVolume, RobotLabel, null, TakeProfit);
                        if (!result.IsSuccessful)
                            if (result.Error.Equals(ErrorCode.NoMoney) ||
                                result.Error.Equals(ErrorCode.BadVolume))
                                Stop();
                        break;
                }
            }
        }

        private double GetAveragePrice(TradeType typeOfTrade, Symbol symbol)
        {
            double averagePrice = 0;
            long count = 0;

            Position[] positions = Positions.FindAll(RobotLabel, symbol, typeOfTrade);
            foreach (Position position in positions)
            {
                averagePrice += position.EntryPrice*position.Volume;
                count += position.Volume;
            }

            if (averagePrice > 0 && count > 0)
                return averagePrice/count;

            return symbol.Bid;
        }

        private int GetPositionsSide(Symbol symbol)
        {
            Position[] positions = Positions.FindAll(RobotLabel, symbol);
            int count = positions.Count(position => position.TradeType == TradeType.Buy);

            if (positions.Length == count)
                return 0;

            count = positions.Count(position => position.TradeType == TradeType.Sell);
            if (positions.Length == count)
                return 1;

            return -1;
        }

        private int GetDynamicPipstep(MarketSeries marketSeries, Symbol symbol)
        {
            const int countOfBars = 25;
            int startBar = marketSeries.Close.Count - 2 - countOfBars;
            int endBar = marketSeries.Close.Count - 2;

            double highestPrice = marketSeries.High[startBar];
            double lowestPrice = marketSeries.Low[startBar];

            for (int i = startBar; i < endBar; i++)
            {
                if (marketSeries.High[i] > highestPrice)
                    highestPrice = marketSeries.High[i];

                if (marketSeries.Low[i] < lowestPrice)
                    lowestPrice = marketSeries.Low[i];
            }

            return (int) ((highestPrice - lowestPrice)/symbol.PipSize/(MaxOrders - 1));
        }

        private double FindLastPrice(TradeType typeOfTrade, Symbol symbol)
        {
            Position[] positions = Positions.FindAll(RobotLabel, symbol, typeOfTrade);
            Position lastPosition = positions.LastOrDefault();
            return lastPosition != null ? lastPosition.EntryPrice : 0;
        }

        private int GetStdIlanSignal(MarketSeries marketSeries)
        {
            int lastBarIndex = marketSeries.Close.Count - 2;
            int prevBarIndex = lastBarIndex - 1;


            if (marketSeries.Close[lastBarIndex] > marketSeries.Open[lastBarIndex] &&
                marketSeries.Close[prevBarIndex] > marketSeries.Open[prevBarIndex]) return 0;

            if (marketSeries.Close[lastBarIndex] < marketSeries.Open[lastBarIndex] &&
                marketSeries.Close[prevBarIndex] < marketSeries.Open[prevBarIndex]) return 1;

            return -1;
        }

        protected override void OnError(Error codeOfError)
        {
            if (codeOfError.Code == ErrorCode.NoMoney)
            {
                Print("ERROR!!! No money for order open, robot is stopped!");
                Stop();
            }
            else if (codeOfError.Code == ErrorCode.BadVolume)
            {
                Print("ERROR!!! Bad volume for order open, robot is stopped!");
                Stop();
            }
        }
    }
}