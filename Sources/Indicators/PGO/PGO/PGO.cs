using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class PGO : Indicator
    {
        private ExponentialMovingAverage _ema;
        private SimpleMovingAverage _sma;
        private TrueRange _trueRange;

        [Parameter("Period", DefaultValue = 14)]
        public int Period { get; set; }

        [Output("PGO")]
        public IndicatorDataSeries Result { get; set; }

		[Output("ZeroLine", Color = Colors.MidnightBlue)]
        public IndicatorDataSeries ZeroLine { get; set; }


        protected override void Initialize()
        {
            _sma = Indicators.SimpleMovingAverage(MarketSeries.Close, Period);
            _trueRange = (TrueRange) Indicators.TrueRange();
            _ema = Indicators.ExponentialMovingAverage(_trueRange.Result, Period);
        }
        public override void Calculate(int index)
        {
            Result[index] = (MarketSeries.Close[index] - _sma.Result[index])/_ema.Result[index];
			ZeroLine[index] = 0;

        }
    }
}
