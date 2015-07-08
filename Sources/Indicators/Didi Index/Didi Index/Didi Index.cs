using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.ESouthAmericaStandardTime, AccessRights = AccessRights.None)]
    public class DidiIndex : Indicator
    {
        [Parameter("Short Source")]
        public DataSeries ShortSource { get; set; }

        [Parameter(DefaultValue = 3)]
        public int ShortPeriods { get; set; }

        [Output("Short Color", Color = Colors.Lime)]
        public IndicatorDataSeries ShortResult { get; set; }

        [Parameter("Main Source")]
        public DataSeries MainSource { get; set; }

        [Parameter(DefaultValue = 8)]
        public int MainPeriods { get; set; }

        [Output("Main Color", Color = Colors.White)]
        public IndicatorDataSeries MainResult { get; set; }

        [Parameter("Long Source")]
        public DataSeries LongSource { get; set; }

        [Parameter(DefaultValue = 20)]
        public int LongPeriods { get; set; }

        [Output("Long Color", Color = Colors.Yellow)]
        public IndicatorDataSeries LongResult { get; set; }

        public override void Calculate(int index)
        {
            double sum;

            sum = 0.0;
            for (int i = index - MainPeriods + 1; i <= index; i++)
            {
                sum += MainSource[i];
            }
            MainResult[index] = sum / MainPeriods;

            sum = 0.0;
            for (int i = index - ShortPeriods + 1; i <= index; i++)
            {
                sum += ShortSource[i];
            }
            ShortResult[index] = sum / ShortPeriods;

            sum = 0.0;
            for (int i = index - LongPeriods + 1; i <= index; i++)
            {
                sum += LongSource[i];
            }
            LongResult[index] = sum / LongPeriods;

            ShortResult[index] /= MainResult[index];
            LongResult[index] /= MainResult[index];
            MainResult[index] = 1;
        }
    }
}
