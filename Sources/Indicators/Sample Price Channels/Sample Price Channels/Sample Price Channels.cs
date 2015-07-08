// -------------------------------------------------------------------------------------------------
//
//    This code is a cAlgo API example.
//    
//    All changes to this file will be lost on next application start.
//    If you are going to modify this file please make a copy using the "Duplicate" command.
//
// -------------------------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AutoRescale = false, AccessRights = AccessRights.None)]
    public class SamplePriceChannels : Indicator
    {
        [Parameter(DefaultValue = 20)]
        public int Periods { get; set; }

        [Output("Upper", Color = Colors.Pink)]
        public IndicatorDataSeries Upper { get; set; }

        [Output("Lower", Color = Colors.Pink)]
        public IndicatorDataSeries Lower { get; set; }

        [Output("Center", Color = Colors.Pink)]
        public IndicatorDataSeries Center { get; set; }

        public override void Calculate(int index)
        {
            double upper = double.MinValue;
            double lower = double.MaxValue;

            for (int i = index - Periods; i <= index - 1; i++)
            {
                upper = Math.Max(MarketSeries.High[i], upper);
                lower = Math.Min(MarketSeries.Low[i], lower);
            }

            Upper[index] = upper;
            Lower[index] = lower;
            Center[index] = (upper + lower) / 2;
        }
    }
}
