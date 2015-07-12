using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot()]

    public class NonLagDotRobot : Robot
    {
        [Parameter()]
        public DataSeries Price { get; set; }

        [Parameter("Length", DefaultValue = 60)]
        public int Length { get; set; }

        [Parameter("Displace", DefaultValue = 0)]
        public int Displace { get; set; }

        [Parameter("Filter", DefaultValue = 0)]
        public int Filter { get; set; }

        [Parameter("Color", DefaultValue = 1)]
        public int ColorFront { get; set; }

        [Parameter("ColorBarBack", DefaultValue = 2)]
        public int ColorBarBack { get; set; }

        [Parameter("Deviation", DefaultValue = 0)]
        public double Deviation { get; set; }

        [Parameter("Volume", DefaultValue = 10000, MinValue = 0)]
        public int Volume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 10)]
        public int StopLoss { get; set; }


		private NonLagDot _trnd;
		string _label = "NonLagDotRobot";


        protected override void OnStart()
        {
            _trnd = Indicators.GetIndicator<NonLagDot>(Price, Length, Displace, Filter, ColorFront, ColorBarBack, Deviation);
        }

        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;

			bool longposition = (Positions.FindAll(_label, Symbol, TradeType.Buy)).Length != 0;
			bool shortposition = (Positions.FindAll(_label, Symbol, TradeType.Sell)).Length != 0;

			int index = MarketSeries.Open.Count-2;

			if (double.IsNaN(_trnd.UpBuffer[index]))
				return;

            if ((_trnd.UpBuffer[index] < MarketSeries.Low[index]) && !longposition)
            {
                ClosePositions(TradeType.Sell);
                Buy();
            }

			if (double.IsNaN(_trnd.DnBuffer[index]))
				return;

			if((_trnd.DnBuffer[index] > MarketSeries.High[index]) && !shortposition)
            {
                ClosePositions(TradeType.Buy);
                Sell();
            }
        }

        private void ClosePositions(TradeType tradeType)
        {
			foreach (Position position in Positions.FindAll(_label,Symbol,tradeType ))
				ClosePosition(position);
          
        }

        private void Buy()
        {
            ExecuteMarketOrder(TradeType.Buy,Symbol, Volume,_label);
        }

        private void Sell()
        {
            ExecuteMarketOrder(TradeType.Sell,Symbol, Volume,_label);
        }

        protected override void OnPositionOpened(Position openedPosition)
        {
            ModifyPosition(openedPosition, GetAbsoluteStopLoss(openedPosition, StopLoss), null);
        }

        private double GetAbsoluteStopLoss(Position position, int StopLoss)
        {
            return position.TradeType == TradeType.Buy ? position.EntryPrice - Symbol.PipSize * StopLoss : position.EntryPrice + Symbol.PipSize * StopLoss;
        }
    }
}
