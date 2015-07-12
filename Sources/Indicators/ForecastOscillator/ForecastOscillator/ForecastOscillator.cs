using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class ForecastOscillator : Indicator
    {
        private MovingAverage _ma;

        [Parameter()]
        public DataSeries Price { get; set; }

        [Parameter("MA Period", DefaultValue = 1)]
        public int MaPeriod { get; set; }

        [Parameter("Regr Period", DefaultValue = 14)]
        public int RegrPeriod { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _ma = Indicators.MovingAverage(Price, MaPeriod, MAType);
        }

        public override void Calculate(int index)
        {
            double sumXY = 0.0;
            double sumX = 0.0;
            double sumY = 0.0;
            double sumX2 = 0.0;

            for (int i = index - RegrPeriod; i < index; i++)
            {
                sumXY += _ma.Result[i] * i;
                // if MaPeriod = 1 => ma.Result[i] = Price[i]  
                sumX += i;
                sumX2 += i * i;
                sumY += _ma.Result[i];
            }
            // slope
            double m = (RegrPeriod * sumXY - sumX * sumY) / (RegrPeriod * sumX2 - sumX * sumX);
            //y Intercept 
            double b = (sumY - m * sumX) / RegrPeriod;
            // linear regression 
            double regression = m * index + b;
            Result[index] = ((MarketSeries.Close[index] - regression) * 100) / MarketSeries.Close[index];

        }
    }
}
