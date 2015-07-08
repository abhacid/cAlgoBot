using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class DoubleCandleIndicator : Indicator
    {
        [Parameter("Signal Fineness", DefaultValue = 0.01)]
        public double SignalFineness { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Signal { get; set; }

        #region Globals
        const int _basePlot = 0;
        const int _sizeSignalPlot = 10;
        const int _spaceBetweenPlot = 2 * _sizeSignalPlot + 5;

        public const int _Neutral = _basePlot;
        public const int _Up = _basePlot + _sizeSignalPlot;
        public const int _Dn = _basePlot - _sizeSignalPlot;

        double _candleCeil;

        #endregion



        protected override void Initialize()
        {
            // Initialize and create nested indicators
            _candleCeil = SignalFineness * Symbol.PipSize;
        }

        public override void Calculate(int index)
        {
            double signal = _Neutral;

            int previewIndex = index - 1;

            double previewOpen = MarketSeries.Open[previewIndex];
            double previewClose = MarketSeries.Close[previewIndex];
            double lastOpen = MarketSeries.Open[index];
            double lastClose = MarketSeries.Close[index];

            if ((lastClose > lastOpen + _candleCeil) && (previewClose > previewOpen + _candleCeil) && (lastOpen >= previewClose))
                signal = _Up;
            else if ((lastClose + _candleCeil < lastOpen) && (previewClose + _candleCeil < previewOpen) && (lastOpen <= previewClose))
                signal = _Dn;

            Signal[index] = signal;

        }
    }
}
