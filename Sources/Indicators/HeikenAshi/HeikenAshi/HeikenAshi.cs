using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class HeikenAshi : Indicator
    {
        private IndicatorDataSeries _haOpen;
        private IndicatorDataSeries _haClose;

        [Parameter("Candle width", DefaultValue = 5)]
        public int CandleWidth { get; set; }

        [Parameter("Up color", DefaultValue = "Blue")]
        public string UpColor { get; set; }

        [Parameter("Down color", DefaultValue = "Red")]
        public string DownColor { get; set; }

        private Colors _upColor;
        private Colors _downColor;
        private bool _incorrectColors;
        private Random _random = new Random();

        protected override void Initialize()
        {
            _haOpen = CreateDataSeries();
            _haClose = CreateDataSeries();

            if (!Enum.TryParse<Colors>(UpColor, out _upColor) || !Enum.TryParse<Colors>(DownColor, out _downColor))
                _incorrectColors = true;
        }

        public override void Calculate(int index)
        {
            if (_incorrectColors)
            {
                var errorColor = _random.Next(2) == 0 ? Colors.Red : Colors.White;
                ChartObjects.DrawText("Error", "Incorrect colors", StaticPosition.Center, errorColor);
                return;
            }

            var open = MarketSeries.Open[index];
            var high = MarketSeries.High[index];
            var low = MarketSeries.Low[index];
            var close = MarketSeries.Close[index];

            var haClose = (open + high + low + close) / 4;
            double haOpen;
            if (index > 0)
                haOpen = (_haOpen[index - 1] + _haClose[index - 1]) / 2;
            else
                haOpen = (open + close) / 2;

            var haHigh = Math.Max(Math.Max(high, haOpen), haClose);
            var haLow = Math.Min(Math.Min(low, haOpen), haClose);

            var color = haOpen > haClose ? _downColor : _upColor;
            ChartObjects.DrawLine("candle" + index, index, haOpen, index, haClose, color, CandleWidth, LineStyle.Solid);
            ChartObjects.DrawLine("line" + index, index, haHigh, index, haLow, color, 1, LineStyle.Solid);

            _haOpen[index] = haOpen;
            _haClose[index] = haClose;
        }
    }
}
