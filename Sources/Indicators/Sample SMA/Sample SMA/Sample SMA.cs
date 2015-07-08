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
    public class SampleSMA : Indicator
    {
        [Parameter("Source")]
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
