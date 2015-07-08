
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class BollingerBands2 : Indicator
    {
        private MovingAverage _movingAverage;
        private StandardDeviation _standardDeviation;

        [Parameter("Period", DefaultValue = 20)]
        public int Period { get; set; }

        [Parameter("SD Weight Coef", DefaultValue = 2)]
        public int K { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Parameter()]
        public DataSeries Price { get; set; }

        [Output("Main", Color = Colors.Blue)]
        public IndicatorDataSeries Main { get; set; }

        [Output("Upper", Color = Colors.Red)]
        public IndicatorDataSeries Upper { get; set; }

        [Output("Lower")]
        public IndicatorDataSeries Lower { get; set; }

        protected override void Initialize()
        {
            _movingAverage = Indicators.MovingAverage(Price, Period, MaType);
            _standardDeviation = Indicators.StandardDeviation(Price, Period, MaType);
        }
        public override void Calculate(int index)
        {

            Main[index] = _movingAverage.Result[index];
            Upper[index] = _movingAverage.Result[index] + K * _standardDeviation.Result[index];
            Lower[index] = _movingAverage.Result[index] - K * _standardDeviation.Result[index];
        }
    }
}
