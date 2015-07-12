using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class HoltEmaAl : Indicator
    {
        private double _alpha;
        private double _gamma;
        private IndicatorDataSeries _trend;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 89, MinValue = 50)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 144, MinValue = 50)]
        public int TrendPeriod { get; set; }

        [Parameter(DefaultValue = 3, MinValue = 2, MaxValue = 3)]
        public int Poles { get; set; }

        [Output("Main", Color = Colors.Violet)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _trend = CreateDataSeries();
            _alpha = 2 / (1.0 + Period);
            _gamma = 2 / (1.0 + TrendPeriod);
        }

        public override void Calculate(int index)
        {
            if (index == 0)
            {
                Result[index] = Source[index];
                _trend[index] = 0.0;
                return;
            }
            Result[index] = _alpha * Source[index] + (1 - _alpha) * (Result[index - 1] + _trend[index - 1]);
            _trend[index] = _gamma * (Result[index] - Result[index - 1]) + (1 - _gamma) * _trend[index - 1];

        }
    }
}
