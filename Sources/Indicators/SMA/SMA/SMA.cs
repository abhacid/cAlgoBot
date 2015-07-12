using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class SMA : Indicator
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Periods { get; set; }

        [Output("Main", Color = Colors.Turquoise)]
        public IndicatorDataSeries Result { get; set; }

        public override void Calculate(int index)
        {
            double sum = 0.0;

            for (int i = index - Periods + 1; i <= index; i++)
            {
                sum += Source[i];
            }
            Result[index] = sum / Periods;
        }
    }
}
