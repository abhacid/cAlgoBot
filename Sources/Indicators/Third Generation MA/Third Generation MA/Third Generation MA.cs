using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ThirdGenerationMA : Indicator
    {
        private double _alpha;
        private MovingAverage _movingAverage1;
        private MovingAverage _movingAverage2;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 220, MinValue = 16)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 50, MinValue = 8)]
        public int SamplingPeriod { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }


        [Output("Main", Color = Colors.Magenta)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            if (Period < 2 * SamplingPeriod)
                ChartObjects.DrawText("message", "Period >= Sampling Period * 2", StaticPosition.TopCenter, Colors.Red);

            var lamda = (double)Period / SamplingPeriod;
            _alpha = lamda * (Period - 1) / (Period - lamda);

            _movingAverage1 = Indicators.MovingAverage(Source, Period, MAType);
            _movingAverage2 = Indicators.MovingAverage(_movingAverage1.Result, SamplingPeriod, MAType);
        }


        public override void Calculate(int index)
        {
            Result[index] = (_alpha + 1) * _movingAverage1.Result[index] - (_alpha * _movingAverage2.Result[index]);
        }
    }
}
