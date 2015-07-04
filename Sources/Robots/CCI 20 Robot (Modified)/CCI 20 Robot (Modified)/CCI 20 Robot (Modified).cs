using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Requests;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class Cci20 : Robot
    {
        private CommodityChannelIndex _cci;
        private Position _position;

        [Parameter(DefaultValue = 1, MinValue = 0, MaxValue = 50)]
        public int RiskPct { get; set; }


        [Parameter(DefaultValue = 500, MinValue = 100, MaxValue = 500)]
        public int Leverage { get; set; }


        [Parameter("Periods", DefaultValue = 20, MinValue = 1)]
        public int Periods { get; set; }


        [Parameter("Stop Loss (pips)", DefaultValue = 10, MinValue = 0)]
        public int StopLoss { get; set; }


        [Parameter("Take Profit (pips)", DefaultValue = 0, MinValue = 0)]
        public int TakeProfit { get; set; }


        [Parameter("Volume", DefaultValue = 10000, MinValue = 1000)]
        public int Volume { get; set; }


        protected int GetVolume
        {
            get
            {
                var risk = (int) (RiskPct*Account.Balance/100);

                int volumeOnRisk = StopLoss > 0 ? (int) (risk*Symbol.Ask/(Symbol.PipSize*StopLoss)) : Volume;

                double maxVolume = Account.Equity*Leverage*100/101;


                double vol = Math.Min(volumeOnRisk, maxVolume);

                return (int) Math.Truncate(Math.Round(vol)/10000)*10000; // round to 10K
            }
        }

        protected override void OnStart()
        {
            _cci = Indicators.CommodityChannelIndex(Periods);
        }


        protected override void OnBar()
        {
            if (Trade.IsExecuting)
                return;

            bool isLongPositionOpen = _position != null && _position.TradeType == TradeType.Buy;
            bool isShortPositionOpen = _position != null && _position.TradeType == TradeType.Sell;

            if (_cci.Result.HasCrossedBelow(0.0, 2) && !isShortPositionOpen)
                OpenPosition(TradeType.Sell);
            else if (_cci.Result.HasCrossedAbove(0.0, 2) && !isLongPositionOpen)
                OpenPosition(TradeType.Buy);
        }


        private void OpenPosition(TradeType type)
        {
            if (_position != null)
                Trade.Close(_position);

            Volume = GetVolume;
            Print(Volume);
            Request request = new MarketOrderRequest(type, Volume)
                                  {
                                      Label = "CCI 20",
                                      StopLossPips = StopLoss > 0 ? StopLoss : (int?) null,
                                      TakeProfitPips = TakeProfit > 0 ? TakeProfit : (int?) null,
                                  };
            Trade.Send(request);
        }


        protected override void OnPositionOpened(Position openedPosition)
        {
            _position = openedPosition;
        }
        
        protected override void OnPositionClosed(Position position)
        {
            _position = null;
        }
    }
}