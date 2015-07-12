using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class CoppockCurve : Indicator
    {
        private MovingAverage _maofRoc;
        private PriceROC _rocLong, _rocShort;
        private IndicatorDataSeries _rocSum;
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("Roc Period Long", DefaultValue = 14)]
        public int RocPeriodLong { get; set; }

        [Parameter("Roc Period Short", DefaultValue = 11)]
        public int RocPeriodShort { get; set; }

        [Parameter("WMA Period", DefaultValue = 10)]
        public int WmaPeriod { get; set; }

        [Output("Coppock Curve")]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _rocLong = Indicators.PriceROC(Source, RocPeriodLong);
            _rocShort = Indicators.PriceROC(Source, RocPeriodShort);
            _rocSum = CreateDataSeries();
            _maofRoc = Indicators.WeightedMovingAverage(_rocSum, WmaPeriod);

        }
        public override void Calculate(int index)
        {
            _rocSum[index] = _rocLong.Result[index] + _rocShort.Result[index];
            Result[index] = _maofRoc.Result[index];
        }
    }
}
