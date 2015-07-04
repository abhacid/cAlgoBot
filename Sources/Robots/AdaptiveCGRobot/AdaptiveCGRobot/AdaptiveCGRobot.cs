//# reference: ..\Indicators\AdaptiveCG.algo
using cAlgo.API;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot()]
    public class AdaptiveCGRobot : Robot
    {
        private AdaptiveCG _adaptiveCG;
        private Position _position;

        [Parameter(DefaultValue = 10000, MinValue = 0)]
        public int Volume { get; set; }

        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        protected override void OnStart()
        {
            _adaptiveCG = Indicators.GetIndicator<AdaptiveCG>(Alpha);
        }

        protected override void OnBar()
        {
            if (Trade.IsExecuting)
                return;

            bool isLongPositionOpen = _position != null && _position.TradeType == TradeType.Buy;
            bool isShortPositionOpen = _position != null && _position.TradeType == TradeType.Sell;

            if (_adaptiveCG.Result.HasCrossedAbove(_adaptiveCG.Trigger, 0) && !isLongPositionOpen)
            {
                ClosePosition();
                Buy();
            }
            if (_adaptiveCG.Result.HasCrossedBelow(_adaptiveCG.Trigger, 0) && !isShortPositionOpen)
            {
                ClosePosition();
                Sell();
            }

        }

        private void ClosePosition()
        {
            if (_position != null)
            {
                Trade.Close(_position);
                _position = null;
            }
        }

        private void Buy()
        {
            Trade.CreateBuyMarketOrder(Symbol, Volume);
        }

        private void Sell()
        {
            Trade.CreateSellMarketOrder(Symbol, Volume);
        }

        protected override void OnPositionOpened(Position openedPosition)
        {
            _position = openedPosition;
        }


    }
}
