using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MovingAverageColorCandles : Indicator
    {

        private MovingAverage _Hma;
        private MovingAverage _Lma;
        private MovingAverage _ma;


        [Parameter("MA Period", DefaultValue = 34)]
        public int maPeriod { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MAType { get; set; }

        [Parameter("Candle width", DefaultValue = 5)]
        public int CandleWidth { get; set; }

        [Parameter("Wick width", DefaultValue = 1)]
        public int WickWidth { get; set; }

        [Parameter("Above up color", DefaultValue = "LimeGreen")]
        public string AboveUpColor { get; set; }

        [Parameter("Above down color", DefaultValue = "DarkGreen")]
        public string AboveDownColor { get; set; }

        [Parameter("Middle up color", DefaultValue = "LightBlue")]
        public string MiddleUpColor { get; set; }

        [Parameter("Middle down color", DefaultValue = "DodgerBlue")]
        public string MiddleDownColor { get; set; }

        [Parameter("Below up color", DefaultValue = "Tomato")]
        public string BelowUpColor { get; set; }

        [Parameter("Below down color", DefaultValue = "Crimson")]
        public string BelowDownColor { get; set; }

        [Output("High Moving Average", Color = Colors.SlateGray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries HMaResult { get; set; }

        [Output("Low Moving Average", Color = Colors.SlateGray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries LMaResult { get; set; }

        [Output("Middle Moving Average", Color = Colors.SlateGray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries MaResult { get; set; }

        private Colors _AboveUpColor;
        private Colors _AboveDownColor;
        private Colors _MiddleUpColor;
        private Colors _MiddleDownColor;
        private Colors _BelowUpColor;
        private Colors _BelowDownColor;
        private Colors color;

        private bool _incorrectColors;
        private Random _random = new Random();

        protected override void Initialize()
        {
            _Hma = Indicators.MovingAverage(MarketSeries.High, maPeriod, MAType);
            _Lma = Indicators.MovingAverage(MarketSeries.Low, maPeriod, MAType);
            _ma = Indicators.MovingAverage(MarketSeries.Close, maPeriod, MAType);

            if (!Enum.TryParse<Colors>(AboveUpColor, out _AboveUpColor) || !Enum.TryParse<Colors>(AboveDownColor, out _AboveDownColor) || !Enum.TryParse<Colors>(BelowUpColor, out _BelowUpColor) || !Enum.TryParse<Colors>(BelowDownColor, out _BelowDownColor) || !Enum.TryParse<Colors>(MiddleUpColor, out _MiddleUpColor) || !Enum.TryParse<Colors>(MiddleDownColor, out _MiddleDownColor))
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

            var HMA = _Hma.Result[index];
            var LMA = _Lma.Result[index];
            var MA = _ma.Result[index];

            HMaResult[index] = HMA;
            LMaResult[index] = LMA;
            MaResult[index] = MA;

            if (close > HMA)
                color = open > close ? _AboveDownColor : _AboveUpColor;

            if (close < LMA)
                color = open > close ? _BelowDownColor : _BelowUpColor;

            if (close >= LMA && close <= HMA)
                color = open > close ? _MiddleDownColor : _MiddleUpColor;

            ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
            ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);



        }
    }
}
