using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class Tema : Indicator
    {
        private ExponentialMovingAverage _ema1;
        private ExponentialMovingAverage _ema2;
        private ExponentialMovingAverage _ema3;

        [Output("TEMA")]
        public IndicatorDataSeries Result { get; set; }

        [Parameter("Data Source")]
        public DataSeries DataSource { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        protected override void Initialize()
        {
            _ema1 = Indicators.ExponentialMovingAverage(DataSource, Period);
            _ema2 = Indicators.ExponentialMovingAverage(_ema1.Result, Period);
            _ema3 = Indicators.ExponentialMovingAverage(_ema2.Result, Period);
        }

        public override void Calculate(int index)
        {
            Result[index] = 3*_ema1.Result[index] - 3*_ema2.Result[index] + _ema3.Result[index];
        }
    }
}