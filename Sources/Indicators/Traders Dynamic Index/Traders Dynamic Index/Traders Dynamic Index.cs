using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Levels(32,50,68)]
    [Indicator(AccessRights = AccessRights.None)]
    public class TradersDynamicIndex : Indicator
    {
        private RelativeStrengthIndex _rsi;
        private MovingAverage _price;
        private MovingAverage _signal;
        private BollingerBands _bollingerBands;

        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter("RSI Period", DefaultValue = 13)]
        public int RsiPeriod { get; set; }
            
        [Parameter("Price Period", DefaultValue = 2)]
        public int PricePeriod { get; set; }

        [Parameter("Signal Period", DefaultValue = 7)]
        public int SignalPeriod { get; set; }

        [Parameter("Volatility Band", DefaultValue = 34)]
        public int Volatility { get; set; }

        [Parameter("Standard Deviations", DefaultValue = 2)]
        public int StDev { get; set; }

        [Parameter("Price Ma Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType PriceMaType { get; set; }

        [Parameter("Signal Ma Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType SignalMaType { get; set; }

        [Output("Upper Band", Color = Colors.Blue)]
        public IndicatorDataSeries Up { get; set; }
        
        [Output("Lower Band", Color = Colors.Blue)]
        public IndicatorDataSeries Down { get; set; }

        [Output("Middle Band", Color = Colors.Orange, Thickness = 2)]
        public IndicatorDataSeries Middle { get; set; }

        [Output("Price", Color = Colors.Green, Thickness = 2)]
        public IndicatorDataSeries PriceSeries { get; set; }

        [Output("Signal", Color = Colors.Red, Thickness = 2)]
        public IndicatorDataSeries SignalSeries { get; set; }

        protected override void Initialize()
        {
            _rsi = Indicators.RelativeStrengthIndex(Source, RsiPeriod);
            _bollingerBands = Indicators.BollingerBands(_rsi.Result, Volatility, StDev, MovingAverageType.Simple);
            _price = Indicators.MovingAverage(_rsi.Result, PricePeriod, PriceMaType);
            _signal = Indicators.MovingAverage(_rsi.Result, SignalPeriod, SignalMaType);
        }

        

        public override void Calculate(int index)
        {
            Up[index] = _bollingerBands.Top[index];
            Down[index] = _bollingerBands.Bottom[index];
            Middle[index] = _bollingerBands.Main[index];

            PriceSeries[index] = _price.Result[index];
            SignalSeries[index] = _signal.Result[index];

        }

        
    }
}
