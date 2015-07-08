using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class Sma252:Robot
    {
        private SimpleMovingAverage _sma;

        private Position _position;
        private bool _isShortPositionOpen;
        private bool _isLongPositionOpen;


        [Parameter(DefaultValue = 252)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 10000)]
        public int Volume { get; set; }
        
        [Parameter]
        public DataSeries Source { get; set; }
        

        protected override void OnStart()
        {
            _sma = Indicators.SimpleMovingAverage(Source, Period);
            
        }

        protected override void OnBar()
        {            
            if (Trade.IsExecuting)
                return;

            var lastIndex = MarketSeries.Close.Count - 1;
            
            _isLongPositionOpen = _position != null && _position.TradeType == TradeType.Buy;
            _isShortPositionOpen = _position != null && _position.TradeType == TradeType.Sell;

            double close = MarketSeries.Close[lastIndex - 1];
            double lastClose = MarketSeries.Close[lastIndex - 2];
            double sma = _sma.Result[lastIndex - 1];
            double lastSma = _sma.Result[lastIndex - 2];

            if (!_isLongPositionOpen && sma < close && lastSma > lastClose)
            {
                ClosePosition(_position);
                Buy();
            }
            else if (!_isShortPositionOpen && sma > close && lastSma < lastClose)
            {
                ClosePosition(_position);
                Sell();                
            }
        }

        /// <summary>
        /// _position holds the latest opened position
        /// </summary>
        /// <param name="openedPosition"></param>
        protected override void OnPositionOpened(Position openedPosition)
        {
            _position = openedPosition;                    
        }

        /// <summary>
        /// Create Buy Order
        /// </summary>
        private void Buy()
        {
            Trade.CreateBuyMarketOrder(Symbol, Volume);
        }

        /// <summary>
        /// Create Sell Order
        /// </summary>
        private void Sell()
        {
            Trade.CreateSellMarketOrder(Symbol, Volume);
        }

        /// <summary>
        /// Close Position
        /// </summary>
        /// <param name="pos"></param>
        private void ClosePosition(Position pos)
        {
            if (pos == null) return;
            Trade.Close(pos);

        }

        /// <summary>
        /// Print Error
        /// </summary>
        /// <param name="error"></param>
        protected override void OnError(Error error)
        {
            Print(error.Code.ToString());
        }
    }
}
