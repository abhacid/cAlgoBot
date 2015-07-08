using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MovingAverageColorCandles : Indicator
    {

        private MovingAverage _ma;

        [Parameter("MA Period", DefaultValue = 34)]
        public int maPeriod { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Weighted)]
        public MovingAverageType MAType { get; set; }

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

        [Output("Moving Average", Color = Colors.Red, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries MaResult { get; set; }

        private Colors _AboveUpColor;
        private Colors _AboveDownColor;
        private Colors _BelowUpColor;
        private Colors _BelowDownColor;
        private Colors color;


        private bool _incorrectColors;
        private Random _random = new Random();

        protected override void Initialize()
        {
            _ma = Indicators.MovingAverage(MarketSeries.Close, maPeriod, MAType);

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

            var MA = _ma.Result[index];
            MaResult[index] = MA;

            if (MA < close)
                color = open > close ? _AboveDownColor : _AboveUpColor;

            if (MA >= close)
                color = open > close ? _BelowDownColor : _BelowUpColor;

            ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
            ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);



        }
    }
}
