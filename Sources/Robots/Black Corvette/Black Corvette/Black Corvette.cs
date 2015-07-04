using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Linq;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class BlackCorvette : Robot
    {
        const string Label = "BlackCorvette robot label";

        [Parameter(DefaultValue = 30)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 2.5)]
        public double StandardDeviation { get; set; }

        [Parameter(DefaultValue = MovingAverageType.TimeSeries)]
        public MovingAverageType MAType { get; set; }

        [Parameter(DefaultValue = 10000)]
        public int Volume { get; set; }

        [Parameter(DefaultValue = 113)]
        public int StopLoss { get; set; }

        [Parameter(DefaultValue = 193)]
        public int TakeProfit { get; set; }

        private BollingerBands _bb;
        private TradeType _lastTradeType;

        protected override void OnStart()
        {
            if (Symbol.Code != "EURUSD" || TimeFrame != TimeFrame.Minute5)
            {
                Print("This robot can be run only on EURUSD Minute5");
                Stop();
                return;
            }

            _bb = Indicators.BollingerBands(MarketSeries.High, Period, StandardDeviation, MAType);
        }

        protected override void OnBar()
        {
            var index = MarketSeries.Close.Count - 2;
            var position = Positions.Find(Label);

            if (MarketSeries.Close[index] > _bb.Top[index] && MarketSeries.Close[index - 1] <= _bb.Top[index - 1] && _lastTradeType != TradeType.Sell)
            {
                if (position != null)
                    ClosePosition(position);
                ExecuteMarketOrder(TradeType.Sell, Symbol, Volume, Label, StopLoss, TakeProfit);
                _lastTradeType = TradeType.Sell;
            }
            if (MarketSeries.Close[index] < _bb.Bottom[index] && MarketSeries.Close[index - 1] <= _bb.Bottom[index - 1] && _lastTradeType != TradeType.Buy)
            {
                if (position != null)
                    ClosePosition(position);
                ExecuteMarketOrder(TradeType.Buy, Symbol, Volume, Label, StopLoss, TakeProfit);
                _lastTradeType = TradeType.Buy;
            }
        }
    }
}
