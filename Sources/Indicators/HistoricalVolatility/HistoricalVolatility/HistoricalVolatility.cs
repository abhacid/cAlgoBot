using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    class HistoricalVolatility : Indicator
    {
        [Parameter("Period", DefaultValue = 14)]
        public int Period { get; set; }

        [Parameter()]
        public DataSeries Price { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        public override void Calculate(int index)
        {
            double mean = 0.0;
            double sum = 0.0;

            for (int i = index - Period; i < index; i++)
            {
                mean += Math.Log(Price[i] / Price[i - 1]);
            }
            mean /= Period;

            for (int i = index - Period; i < index; i++)
            {
                sum += Math.Pow(Math.Log(Price[i] / Price[i - 1]) - mean, 2);
            }

            Result[index] = Math.Sqrt(sum / (Period - 1));

        }
    }
}
