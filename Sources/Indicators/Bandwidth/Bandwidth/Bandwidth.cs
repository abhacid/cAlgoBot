using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class Bandwidth : Indicator
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("MAType")]
        public MovingAverageType matype { get; set; }

        [Parameter("Period", DefaultValue = 20)]
        public int Period { get; set; }

        [Parameter("Standard Deviation", DefaultValue = 2.0)]
        public double std { get; set; }

        [Output("Main", Color = Colors.Red)]
        public IndicatorDataSeries Result { get; set; }

        BollingerBands bb;

        protected override void Initialize()
        {
            bb = Indicators.BollingerBands(Source, Period, std, matype);
        }

        public override void Calculate(int index)
        {
            Result[index] = (bb.Top[index] - bb.Bottom[index]) / bb.Main[index];
        }
    }
}
