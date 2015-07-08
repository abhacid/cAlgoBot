//#reference: ..\Indicators\AdxVma.algo

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class AdxVmaBands : Indicator
    {
        private AdxVma _adxVma;
        private AdxVma _offset;
        private TypicalPrice _typicalPrice;
        private IndicatorDataSeries _diff;

        [Parameter(DefaultValue = 6)]
        public int Period { get; set; }
        [Parameter(DefaultValue = 1.6)]
        public double Multiplier { get; set; }

        [Output("Rising", Color = Colors.Green, PlotType = PlotType.Points, Thickness = 2)]
        public IndicatorDataSeries Rising { get; set; }
        [Output("Falling", Color = Colors.Red, PlotType = PlotType.Points, Thickness = 2)]
        public IndicatorDataSeries Falling { get; set; }
        [Output("Flat", Color = Colors.Gold, PlotType = PlotType.Points, Thickness = 2)]
        public IndicatorDataSeries Flat { get; set; }

        [Output("Upper Band", Color = Colors.Gray)]
        public IndicatorDataSeries UpperBand { get; set; }
        [Output("Lower Band", Color = Colors.Gray)]
        public IndicatorDataSeries LowerBand { get; set; }

        protected override void Initialize()
        {
            _diff = CreateDataSeries();
            _typicalPrice = Indicators.TypicalPrice();
            _adxVma = Indicators.GetIndicator<AdxVma>(_typicalPrice.Result, Period);
            _offset = Indicators.GetIndicator<AdxVma>(_diff, Period);
        }

        public override void Calculate(int index)
        {
            _diff[index] = MarketSeries.High[index] - MarketSeries.Low[index];

            if (index < Period)
                return;


            UpperBand[index] = _adxVma.Result[index] + _offset.Result[index] * Multiplier;
            LowerBand[index] = _adxVma.Result[index] - _offset.Result[index] * Multiplier;

            Rising[index] = _adxVma.Rising[index];
            Falling[index] = _adxVma.Falling[index];
            Flat[index] = _adxVma.Flat[index];

        }
    }
}
