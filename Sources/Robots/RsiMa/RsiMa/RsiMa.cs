using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class RsiMa : Robot
    {
        private Position _position;
        private MovingAverage ma;
        private RelativeStrengthIndex rsi;

        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("RSI Periods", DefaultValue = 4)]
        public int Periods { get; set; }

        [Parameter("MA Period", DefaultValue = 200)]
        public int MAPeriod { get; set; }

        [Parameter("Volume", DefaultValue = 10000, MinValue = 0)]
        public int Volume { get; set; }

        [Parameter("MA Type")]
        public MovingAverageType MAType { get; set; }

        protected override void OnStart()
        {
            rsi = Indicators.RelativeStrengthIndex(Source, Periods);
            ma = Indicators.MovingAverage(Source, MAPeriod, MAType);
        }

        protected override void OnBar()
        {
            if (Trade.IsExecuting) return;

            if (rsi.Result.LastValue < 25 && _position == null)
            {
                OpenPosition(TradeType.Buy);
            }

            if (rsi.Result.LastValue > 75 && _position == null)
            {
                OpenPosition(TradeType.Sell);
            }
            if (_position != null)
            {
                if (_position.TradeType == TradeType.Buy && rsi.Result.LastValue > 55)
                {
                    Trade.Close(_position);
                }

                if (_position.TradeType == TradeType.Sell && rsi.Result.LastValue < 45)
                {
                    Trade.Close(_position);
                }
            }
        }

        private void OpenPosition(TradeType command)
        {
            Trade.CreateMarketOrder(command, Symbol, Volume);
        }

        protected override void OnPositionOpened(Position openedPosition)
        {
            _position = openedPosition;
        }

        protected override void OnPositionClosed(Position closedPosition)
        {
            _position = null;
        }
    }
}