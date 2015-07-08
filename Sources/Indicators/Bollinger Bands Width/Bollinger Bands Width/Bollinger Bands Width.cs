using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class BollingerBandsWidth : Indicator
    {
        [Parameter("Period", DefaultValue = 20)]
        public int Period { get; set; }

        [Parameter("SD Weight Coef", DefaultValue = 2)]
        public int K { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Parameter()]
        public DataSeries Source { get; set; }

        private BollingerBands _bollingerBands;

        [Output("diff")]
        public IndicatorDataSeries Diff { get; set; }

        protected override void Initialize()
        {
            _bollingerBands = Indicators.BollingerBands(Source, Period, K, MaType);
        }

        public override void Calculate(int index)
        {
            Diff[index] = (_bollingerBands.Top[index] - _bollingerBands.Bottom[index]) / _bollingerBands.Main[index];
        }
    }
}
