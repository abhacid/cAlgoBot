
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class TimeSeriesForecast : Indicator
    {
        [Parameter()]
        public DataSeries DataSource { get; set; }

        [Output("TSF")]
        public IndicatorDataSeries tsf { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 3)]
        public int forecast { get; set; }

        public IndicatorDataSeries y;

        double SUM;
        protected override void Initialize()
        {
            y = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            double sumX = ((double)Period * (Period - 1) * 0.5);
            double divisor = sumX * sumX - (double)Period * Period * (Period - 1) * (2 * Period - 1) / 6;

            double sumXY = 0;
            for (int count = 0; count < Period && index - count >= 0; count++)
            {
                sumXY += count * DataSource[index - count];
            }
            y[index] = DataSource[index];
            SUM = 0;
            for (int i = 0; i < Period; i++)
            {
                SUM += y[index - i];
            }

            double slope = ((double)Period * sumXY - sumX * SUM) / divisor;
            double intercept = ((SUM - (slope * sumX)) / Period);
            tsf[index] = (intercept + (slope * ((Period - 1) + forecast)));
        }
    }
}
