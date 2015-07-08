using System;
using cAlgo.API;
using cAlgo.API.Indicators;


namespace cAlgo.Indicators
{
    [Indicator(ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class ZeroLagMACD : Indicator
    {
        private ExponentialMovingAverage _emaLong;
        private ExponentialMovingAverage _emaShort;
        private ExponentialMovingAverage _emaSignal;
        private ExponentialMovingAverage _emaLong2;
        private ExponentialMovingAverage _emaShort2;

        [Parameter("Long Cycle", DefaultValue = 26)]
        public int LongCycle { get; set; }

        [Parameter("Short Cycle", DefaultValue = 12)]
        public int ShortCycle { get; set; }

        [Parameter("Signal Periods", DefaultValue = 9)]
        public int SignalPeriods { get; set; }

        [Output("Histogram", Color = Colors.Turquoise, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries Histogram { get; set; }

        [Output("MACD", Color = Colors.Blue)]
        public IndicatorDataSeries MACD { get; set; }

        [Output("Signal", Color = Colors.Red, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries Signal { get; set; }

        private IndicatorDataSeries _zeroLagEmaShort;
        private IndicatorDataSeries _zeroLagEmaLong;

        protected override void Initialize()
        {
            _zeroLagEmaShort = CreateDataSeries();
            _zeroLagEmaLong = CreateDataSeries();

            _emaLong = Indicators.ExponentialMovingAverage(MarketSeries.Close, LongCycle);
            _emaLong2 = Indicators.ExponentialMovingAverage(_emaLong.Result, LongCycle);

            _emaShort = Indicators.ExponentialMovingAverage(MarketSeries.Close, ShortCycle);
            _emaShort2 = Indicators.ExponentialMovingAverage(_emaShort.Result, ShortCycle);

            _emaSignal = Indicators.ExponentialMovingAverage(MACD, SignalPeriods);
        }

        public override void Calculate(int index)
        {
            _zeroLagEmaShort[index] = _emaShort.Result[index] * 2 - _emaShort2.Result[index];
            _zeroLagEmaLong[index] = _emaLong.Result[index] * 2 - _emaLong2.Result[index];

            MACD[index] = _zeroLagEmaShort[index] - _zeroLagEmaLong[index];

            Signal[index] = _emaSignal.Result[index];
            Histogram[index] = MACD[index] - Signal[index];
        }
    }
}
