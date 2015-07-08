using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class HeikinAshiColorCandles : Indicator
    {

//ADD HA
        private IndicatorDataSeries _haOpen;
        private IndicatorDataSeries _haClose;
//END HA

        [Parameter("Candle width", DefaultValue = 5)]
        public int CandleWidth { get; set; }

        [Parameter("Wick width", DefaultValue = 1)]
        public int WickWidth { get; set; }

        [Parameter("Above up color", DefaultValue = "LimeGreen")]
        public string AboveUpColor { get; set; }

        [Parameter("Above down color", DefaultValue = "DarkGreen")]
        public string AboveDownColor { get; set; }

        [Parameter("Below up color", DefaultValue = "Tomato")]
        public string BelowUpColor { get; set; }

        [Parameter("Below down color", DefaultValue = "Crimson")]
        public string BelowDownColor { get; set; }

        private Colors _AboveUpColor;
        private Colors _AboveDownColor;
        private Colors _BelowUpColor;
        private Colors _BelowDownColor;
        private Colors color;

        private bool _incorrectColors;
        private Random _random = new Random();

        protected override void Initialize()
        {


            _haOpen = CreateDataSeries();
            _haClose = CreateDataSeries();


            if (!Enum.TryParse<Colors>(AboveUpColor, out _AboveUpColor) || !Enum.TryParse<Colors>(AboveDownColor, out _AboveDownColor) || !Enum.TryParse<Colors>(BelowUpColor, out _BelowUpColor) || !Enum.TryParse<Colors>(BelowDownColor, out _BelowDownColor))
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


            if (haOpen < haClose)
                color = open > close ? _AboveDownColor : _AboveUpColor;

            if (haOpen > haClose)
                color = open > close ? _BelowDownColor : _BelowUpColor;


            ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
            ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);


            _haOpen[index] = haOpen;
            _haClose[index] = haClose;

        }
    }
}
