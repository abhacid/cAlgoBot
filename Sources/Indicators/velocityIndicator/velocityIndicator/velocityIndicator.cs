// -------------------------------------------------------------------------------------------------
//
//    Leonardo Hermoso, modifié par https://www.facebook.com/ab.hacid
//    
//    leonardo.hermoso arroba hotmail.com
//    If you are going to modify this file please make a copy using the "Duplicate" command.
//
// -------------------------------------------------------------------------------------------------

using System;
using cAlgo.API;


namespace cAlgo.Indicators
{
    [Indicator(TimeZone = TimeZones.UTC, IsOverlay = false, AccessRights = AccessRights.None)]
    public class VelocityIndicator : Indicator
    {
        #region cIndicators Parammeters
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("VelocityPeriod", DefaultValue = 14, MinValue = 2)]
        public int VelocityPeriod { get; set; }

        [Output("Acceleration", Color = Colors.BlueViolet)]
        public IndicatorDataSeries acceleration { get; set; }

        [Output("VelocityIndicator", Color = Colors.Gray)]
        public IndicatorDataSeries velocity { get; set; }
        #endregion

        protected override void Initialize()
        {

        }

        public override void Calculate(int index)
        {

            double a = ((2 * (Source[index] - Source[index - VelocityPeriod]) * (1 / Symbol.PipSize)) / Math.Pow(VelocityPeriod, 2));
            // S = S0 + V0t +(at^2)/2   a = (2*(S-S0))/t^2
            velocity[index] = (a * VelocityPeriod);
            // v = v0 +at
            acceleration[index] = 10 * a;

            //acceleration[index] =(((2 * (Math.Abs(Source[index] - Source[index - VelocityPeriod])   * (1/Symbol.PipSize) ))/ Math.Pow(VelocityPeriod,2)))*10 ;

        }
    }
}



