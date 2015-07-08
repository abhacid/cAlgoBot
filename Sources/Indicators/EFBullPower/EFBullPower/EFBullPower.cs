using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class EFBullPower : Indicator
    {
        private ExponentialMovingAverage _ema;

        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter("Period", DefaultValue = 13)]
        public int Period { get; set; }

        [Output("Bull Power", PlotType = PlotType.Histogram)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _ema = Indicators.ExponentialMovingAverage(Source, Period);
        }

        public override void Calculate(int index)
        {
            Result[index] = MarketSeries.High[index] - _ema.Result[index];
        }
    }
}
