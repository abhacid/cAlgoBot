//#reference: ..\Indicators\AverageTrueRange.algo

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class SuperTrendPlus : Indicator
    {
        private AverageTrueRange _averageTrueRange;
        private ExponentialMovingAverage _ema;
        private bool[] _trend;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 1, MaxValue = 2, MinValue = 1)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 1.618, MaxValue = 2.0, MinValue = 1.0)]
        public double Multiplier { get; set; }


        [Output("UpTrend", Color = Colors.Green, PlotType = PlotType.Points, Thickness = 2)]
        public IndicatorDataSeries UpTrend { get; set; }

        [Output("DownTrend", Color = Colors.Red, PlotType = PlotType.Points, Thickness = 2)]
        public IndicatorDataSeries DownTrend { get; set; }

        [Output("EMA")]
        public IndicatorDataSeries EMA { get; set; }


        protected override void Initialize()
        {
            _trend = new bool[1];
            _ema = Indicators.ExponentialMovingAverage(Source, Period);
            _averageTrueRange = Indicators.GetIndicator<AverageTrueRange>(Period);
        }

        public override void Calculate(int index)
        {
            double close = MarketSeries.Close[index];

            if (index < 1)
            {
                _trend[index] = true;
                UpTrend[index] = close;
                DownTrend[index] = close;

                return;
            }

            Array.Resize(ref _trend, _trend.Length + 1);

            double median = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;

            if (close > DownTrend[index - 1])
            {
                _trend[index] = true;

            }
            else if (close < UpTrend[index - 1])
            {
                _trend[index] = false;
            }
            else
            {
                _trend[index] = _trend[index - 1];
            }

            double lowerValue = median - _averageTrueRange.Result[index] * Multiplier;
            double upperValue = median + _averageTrueRange.Result[index] * Multiplier;

            if (_trend[index] && !_trend[index - 1])
            {
                UpTrend[index] = lowerValue;
            }
            else if (!_trend[index] && _trend[index - 1])
            {
                DownTrend[index] = upperValue;
            }
            else if (_trend[index])
            {
                UpTrend[index] = lowerValue > UpTrend[index - 1] ? lowerValue : UpTrend[index - 1];
            }
            else
            {
                DownTrend[index] = upperValue < DownTrend[index - 1] ? upperValue : DownTrend[index - 1];
            }

            EMA[index] = _ema.Result[index];
        }
    }
}
