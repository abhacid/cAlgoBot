using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class GannHighLow : Indicator
    {
        private SimpleMovingAverage _smaHigh;
        private SimpleMovingAverage _smaLow;
        private double _d;

        [Parameter("Period", DefaultValue = 10)]
        public int Period { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _smaHigh = Indicators.SimpleMovingAverage(MarketSeries.High, Period);
            _smaLow = Indicators.SimpleMovingAverage(MarketSeries.Low, Period);
        }

        public override void Calculate(int index)
        {
            if (index < Period)
                return;

            double close = MarketSeries.Close[index];
            double smaHigh = _smaHigh.Result[index - 1];
            double smaHighPrev = _smaHigh.Result[index - 2];
            double smaLow = _smaLow.Result[index - 1];

            if (close > smaHigh)
                Result[index] = smaLow;
            else
            {
                if (close < smaLow)
                    Result[index] = smaHigh;
                else
                {
                    if (Result[index - 1] == smaHighPrev)
                        Result[index] = smaHigh;
                    else
                        Result[index] = smaLow;
                }
            }

        }
    }
}
