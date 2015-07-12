using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    internal class KeltnerChannels : Indicator
    {
        private MovingAverage _ma;
        private MovingAverage _maBands;
        private IndicatorDataSeries _typicalPrice, _highLowRange;

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        [Output("UpperBand", Color = Colors.Blue)]
        public IndicatorDataSeries UpperBand { get; set; }

        [Output("LowerBand", Color = Colors.Blue)]
        public IndicatorDataSeries LowerBand { get; set; }

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Parameter("MA Period", DefaultValue = 10)]
        public int MaPeriod { get; set; }


        protected override void Initialize()
        {
            _typicalPrice = CreateDataSeries();
            _highLowRange = CreateDataSeries();
            _ma = Indicators.MovingAverage(_typicalPrice, MaPeriod, MaType);
            _maBands = Indicators.MovingAverage(_highLowRange, MaPeriod, MaType);
        }

        public override void Calculate(int index)
        {
            // Middle Channel
            _typicalPrice[index] = (MarketSeries.High[index] + MarketSeries.Low[index] + MarketSeries.Close[index]) / 3;
            Result[index] = _ma.Result[index];
            // Upper and Lower Channel
            _highLowRange[index] = MarketSeries.High[index] - MarketSeries.Low[index];
            UpperBand[index] = _ma.Result[index] + _maBands.Result[index];
            LowerBand[index] = _ma.Result[index] - _maBands.Result[index];
        }
    }
}
