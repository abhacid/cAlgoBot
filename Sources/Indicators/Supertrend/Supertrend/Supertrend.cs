using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Supertrend : Indicator
    {
        [Parameter(DefaultValue = 10)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 3.0)]
        public double Multiplier { get; set; }

        [Output("UpTrend", Color = Colors.Green, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries UpTrend { get; set; }

        [Output("DownTrend", Color = Colors.Red, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries DownTrend { get; set; }

        private IndicatorDataSeries _upBuffer;
        private IndicatorDataSeries _downBuffer;
        private AverageTrueRange _averageTrueRange;
        private int[] _trend;
        private bool _changeofTrend;

        protected override void Initialize()
        {
            _trend = new int[1];
            _upBuffer = CreateDataSeries();
            _downBuffer = CreateDataSeries();
            _averageTrueRange = Indicators.AverageTrueRange(Period, MovingAverageType.WilderSmoothing);
        }

        public override void Calculate(int index)
        {
            // Init
            UpTrend[index] = double.NaN;
            DownTrend[index] = double.NaN;

            double median = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;
            double atr = _averageTrueRange.Result[index];

            _upBuffer[index] = median + Multiplier * atr;
            _downBuffer[index] = median - Multiplier * atr;


            if (index < 1)
            {
                _trend[index] = 1;
                return;
            }

            Array.Resize(ref _trend, _trend.Length + 1);

            // Main Logic
            if (MarketSeries.Close[index] > _upBuffer[index - 1])
            {
                _trend[index] = 1;
                if (_trend[index - 1] == -1)
                    _changeofTrend = true;
            }
            else if (MarketSeries.Close[index] < _downBuffer[index - 1])
            {
                _trend[index] = -1;
                if (_trend[index - 1] == -1)
                    _changeofTrend = true;
            }
            else if (_trend[index - 1] == 1)
            {
                _trend[index] = 1;
                _changeofTrend = false;
            }
            else if (_trend[index - 1] == -1)
            {
                _trend[index] = -1;
                _changeofTrend = false;
            }

            if (_trend[index] < 0 && _trend[index - 1] > 0)
                _upBuffer[index] = median + (Multiplier * atr);
            else if (_trend[index] < 0 && _upBuffer[index] > _upBuffer[index - 1])
                _upBuffer[index] = _upBuffer[index - 1];

            if (_trend[index] > 0 && _trend[index - 1] < 0)
                _downBuffer[index] = median - (Multiplier * atr);
            else if (_trend[index] > 0 && _downBuffer[index] < _downBuffer[index - 1])
                _downBuffer[index] = _downBuffer[index - 1];

            // Draw Indicator
            if (_trend[index] == 1)
            {
                UpTrend[index] = _downBuffer[index];
                if (_changeofTrend)
                {
                    UpTrend[index - 1] = DownTrend[index - 1];
                    _changeofTrend = false;
                }
            }
            else if (_trend[index] == -1)
            {
                DownTrend[index] = _upBuffer[index];
                if (_changeofTrend)
                {
                    DownTrend[index - 1] = UpTrend[index - 1];
                    _changeofTrend = false;
                }
            }
        }
    }
}
