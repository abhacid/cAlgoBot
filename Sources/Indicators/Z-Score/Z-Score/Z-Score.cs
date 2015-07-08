using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Levels(0.0, 2.0)]
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ZScore : Indicator
    {
        private MovingAverage movingAverage;
        private StandardDeviation standardDeviation;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 20)]
        public int Period { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }


        protected override void Initialize()
        {
            // Initialize and create nested indicators
            movingAverage = Indicators.MovingAverage(Source, Period, MaType);
            standardDeviation = Indicators.StandardDeviation(Source, Period, MaType);
        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index
            Result[index] = (Source[index] - movingAverage.Result[index]) / standardDeviation.Result[index];
        }
    }
}
