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
    public class SampleEMA : Indicator
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Periods", DefaultValue = 14)]
        public int Periods { get; set; }

        [Output("Main", Color = Colors.Turquoise)]
        public IndicatorDataSeries Result { get; set; }

        private double exp;

        protected override void Initialize()
        {
            exp = 2.0 / (Periods + 1);
        }

        public override void Calculate(int index)
        {
            var previousValue = Result[index - 1];

            if (double.IsNaN(previousValue))
            {
                Result[index] = Source[index];
            }
            else
            {
                Result[index] = Source[index] * exp + previousValue * (1 - exp);
            }
        }
    }
}
