//#reference: ..\Indicators\SMI.algo
using cAlgo.API;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot()]
    public class SmiBot : Robot
    {
        private SMI _smi;
        private Position _position;

        [Parameter(DefaultValue = 10000, MinValue = 0)]
        public int Volume { get; set; }

        [Parameter("Period", DefaultValue = 10)]
        public int Period { get; set; }


        protected override void OnStart()
        {
            _smi = Indicators.GetIndicator<SMI>(Period, MovingAverageType.Simple);
        }

        protected override void OnBar()
        {
            if (Trade.IsExecuting)
                return;

            bool isLongPositionOpen = _position != null && _position.TradeType == TradeType.Buy;
            bool isShortPositionOpen = _position != null && _position.TradeType == TradeType.Sell;

            if (_smi.Result.HasCrossedAbove(_smi.BuyLine, 0) && !isLongPositionOpen)
            {
                ClosePosition();
                Buy();
            }

            if (_smi.Result.HasCrossedBelow(_smi.SellLine, 0) && !isShortPositionOpen)
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
