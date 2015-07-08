//#reference: ..\Indicators\AverageTrueRange.algo
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class FibonacciBands : Indicator
    {
        private AverageTrueRange _averageTrueRange;
        private ExponentialMovingAverage _exponentialMovingAverage;

        [Parameter(DefaultValue = 55)]
        public int PeriodEma { get; set; }

        [Parameter(DefaultValue = 21)]
        public int PeriodAtr { get; set; }

        [Output("Upper Band 1", Color = Colors.DarkCyan)]
        public IndicatorDataSeries UpperBand1 { get; set; }

        [Output("Upper Band 2", Color = Colors.DarkCyan)]
        public IndicatorDataSeries UpperBand2 { get; set; }

        [Output("Upper Band 3", Color = Colors.DarkCyan)]
        public IndicatorDataSeries UpperBand3 { get; set; }

        [Output("Upper Band 4", Color = Colors.DarkCyan)]
        public IndicatorDataSeries UpperBand4 { get; set; }

        [Output("Lower Band 1", Color = Colors.DarkGreen)]
        public IndicatorDataSeries LowerBand1 { get; set; }

        [Output("Lower Band 2", Color = Colors.DarkGreen)]
        public IndicatorDataSeries LowerBand2 { get; set; }

        [Output("Lower Band 3", Color = Colors.DarkGreen)]
        public IndicatorDataSeries LowerBand3 { get; set; }

        [Output("Lower Band 4", Color = Colors.DarkGreen)]
        public IndicatorDataSeries LowerBand4 { get; set; }

        [Output("EMA", Color = Colors.Blue)]
        public IndicatorDataSeries Ema { get; set; }

        protected override void Initialize()
        {
            _averageTrueRange = Indicators.GetIndicator<AverageTrueRange>(PeriodAtr);
            _exponentialMovingAverage = Indicators.ExponentialMovingAverage(MarketSeries.Close, PeriodEma);
        }

        public override void Calculate(int index)
        {
            double ema = _exponentialMovingAverage.Result[index];
            double atr = _averageTrueRange.Result[index];

            UpperBand1[index] = ema + 1.62 * atr;
            UpperBand2[index] = ema + 2.62 * atr;
            UpperBand3[index] = ema + 4.23 * atr;
            UpperBand4[index] = ema + 1 * atr;
            LowerBand1[index] = ema - 1.62 * atr;
            LowerBand2[index] = ema - 2.62 * atr;
            LowerBand3[index] = ema - 4.23 * atr;
            LowerBand4[index] = ema - 1 * atr;

            Ema[index] = ema;
        }
    }
}
