using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class TEMA : Indicator
    {
        [Output("TEMA")]
        public IndicatorDataSeries tema { get; set; }

        [Parameter("Data Source")]
        public DataSeries DataSource { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        private ExponentialMovingAverage ema1;
        private ExponentialMovingAverage ema2;
        private ExponentialMovingAverage ema3;

        protected override void Initialize()
        {
            ema1 = Indicators.ExponentialMovingAverage(DataSource, Period);
            ema2 = Indicators.ExponentialMovingAverage(ema1.Result, Period);
            ema3 = Indicators.ExponentialMovingAverage(ema2.Result, Period);
        }

        public override void Calculate(int index)
        {
            tema[index] = 3 * ema1.Result[index] - 3 * ema2.Result[index] + ema3.Result[index];
        }
    }
}
