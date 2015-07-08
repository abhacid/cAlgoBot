using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator("Highest High Lowest Low Bands", IsOverlay = true, AccessRights = AccessRights.None)]
    public class HighestHighLowestLow : Indicator
    {

        [Parameter(DefaultValue = 10, MinValue = 1)]
        public int PeriodsHigh { get; set; }

        [Parameter(DefaultValue = 10, MinValue = 1)]
        public int PeriodsLow { get; set; }

        [Output("Top", Color = Colors.Red)]
        public IndicatorDataSeries Top { get; set; }

        [Output("Bottom", Color = Colors.Blue)]
        public IndicatorDataSeries Bottom { get; set; }


        public override void Calculate(int index)
        {
            Top[index] = MarketSeries.High.Maximum(PeriodsHigh);
            Bottom[index] = MarketSeries.Low.Minimum(PeriodsLow);
        }
    }
}
