using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class KeltnerChannelsATR : Indicator
    {
        private MovingAverage _ma;
        private AverageTrueRange _atr;

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        [Output("UpperBand", Color = Colors.Blue)]
        public IndicatorDataSeries UpperBand { get; set; }

        [Output("LowerBand", Color = Colors.Blue)]
        public IndicatorDataSeries LowerBand { get; set; }

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MaType { get; set; }

        [Parameter("MA Period", DefaultValue = 20)]
        public int MaPeriod { get; set; }

        [Parameter("ATR Period", DefaultValue = 10)]
        public int AtrPeriod { get; set; }

        [Parameter("ATR Multiplier", DefaultValue = 2.5)]
        public int AtrMultiplier { get; set; }

        protected override void Initialize()
        {
            _ma = Indicators.MovingAverage(Source, MaPeriod, MaType);
            _atr = Indicators.AverageTrueRange(AtrPeriod, MovingAverageType.Simple);
        }

        public override void Calculate(int index)
        {
            Result[index] = _ma.Result[index];
            UpperBand[index] = _ma.Result[index] + AtrMultiplier * _atr.Result[index];
            LowerBand[index] = _ma.Result[index] - AtrMultiplier * _atr.Result[index];
        }
    }
}
