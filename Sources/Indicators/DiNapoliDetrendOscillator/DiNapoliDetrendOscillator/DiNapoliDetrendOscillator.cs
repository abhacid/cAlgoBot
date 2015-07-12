using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Levels(0)]
    [Indicator(ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class DiNapoliDetrendOscillator : Indicator
    {
        private MovingAverage _movingAverage;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Output("DiNapoliDPO")]
        public IndicatorDataSeries Result { get; set; }

        [Output("OverBought", Color = Colors.Gray)]
        public IndicatorDataSeries OverBought { get; set; }

        [Output("OverSold", Color = Colors.Gray)]
        public IndicatorDataSeries OverSold { get; set; }

        protected override void Initialize()
        {
            _movingAverage = Indicators.MovingAverage(Source, Period, MaType);
        }
        public override void Calculate(int index)
        {
            if (index < Period)
            {
                Result[index] = 0;
                return;
            }

            Result[index] = Source[index] - _movingAverage.Result[index];
            OverSold[index] = -0.01;
            OverBought[index] = 0.01;
        }
    }
}
