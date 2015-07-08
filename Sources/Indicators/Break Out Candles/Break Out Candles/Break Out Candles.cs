using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class BreakOutCandles : Indicator
    {

        [Parameter("How Many Break Out Bars", DefaultValue = 2)]
        public int HowMany { get; set; }

        [Parameter("Candle width", DefaultValue = 5)]
        public int CandleWidth { get; set; }

        [Parameter("Wick width", DefaultValue = 1)]
        public int WickWidth { get; set; }

        [Parameter("Breakout up color", DefaultValue = "LimeGreen")]
        public string UpColor { get; set; }

        [Parameter("Breakout down color", DefaultValue = "Red")]
        public string DownColor { get; set; }

        [Parameter("Range up color", DefaultValue = "LightGray")]
        public string RangeUpColor { get; set; }

        [Parameter("Rangedown color", DefaultValue = "LightGray")]
        public string RangeDownColor { get; set; }


        private Colors _UpColor;
        private Colors _DownColor;
        private Colors _RangeUpColor;
        private Colors _RangeDownColor;
        private Colors color;


        private bool _incorrectColors;
        private Random _random = new Random();

        protected override void Initialize()
        {
            if (!Enum.TryParse<Colors>(UpColor, out _UpColor) || !Enum.TryParse<Colors>(DownColor, out _DownColor) || !Enum.TryParse<Colors>(RangeUpColor, out _RangeUpColor) || !Enum.TryParse<Colors>(RangeDownColor, out _RangeDownColor))
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

            var High1 = MarketSeries.High[index - 1];
            var High2 = MarketSeries.High[index - 2];
            var High3 = MarketSeries.High[index - 3];
            var High4 = MarketSeries.High[index - 4];
            var High5 = MarketSeries.High[index - 5];
            var High6 = MarketSeries.High[index - 6];
            var High7 = MarketSeries.High[index - 7];
            var High8 = MarketSeries.High[index - 8];

            var Low1 = MarketSeries.Low[index - 1];
            var Low2 = MarketSeries.Low[index - 2];
            var Low3 = MarketSeries.Low[index - 3];
            var Low4 = MarketSeries.Low[index - 4];
            var Low5 = MarketSeries.Low[index - 5];
            var Low6 = MarketSeries.Low[index - 6];
            var Low7 = MarketSeries.Low[index - 7];
            var Low8 = MarketSeries.Low[index - 8];



            if (HowMany == 1)
            {
                if (close > High1)
                    color = _UpColor;
                else if (close < Low1)
                    color = _DownColor;
                else if (close > open)
                    color = _RangeUpColor;
                else if (close <= open)
                    color = _RangeDownColor;

            }


            if (HowMany == 2)
            {
                if (close > High1 && close > High2)
                    color = _UpColor;
                else if (close < Low1 && close < Low2)
                    color = _DownColor;
                else if (close > open)
                    color = _RangeUpColor;
                else if (close <= open)
                    color = _RangeDownColor;

            }

            if (HowMany == 3)
            {
                if (close > High1 && close > High2 && close > High3)
                    color = _UpColor;
                else if (close < Low1 && close < Low2 && close < Low3)
                    color = _DownColor;
                else if (close > open)
                    color = _RangeUpColor;
                else if (close <= open)
                    color = _RangeDownColor;

            }

            if (HowMany == 4)
            {
                if (close > High1 && close > High2 && close > High3 && close > High4)
                    color = _UpColor;
                else if (close < Low1 && close < Low2 && close < Low3 && close < Low4)
                    color = _DownColor;
                else if (close > open)
                    color = _RangeUpColor;
                else if (close <= open)
                    color = _RangeDownColor;

            }


            if (HowMany == 5)
            {
                if (close > High1 && close > High2 && close > High3 && close > High4 && close > High5)
                    color = _UpColor;
                else if (close < Low1 && close < Low2 && close < Low3 && close < Low4 && close < Low5)
                    color = _DownColor;
                else if (close > open)
                    color = _RangeUpColor;
                else if (close <= open)
                    color = _RangeDownColor;

            }


            if (HowMany == 6)
            {
                if (close > High1 && close > High2 && close > High3 && close > High4 && close > High5 && close > High6)
                    color = _UpColor;
                else if (close < Low1 && close < Low2 && close < Low3 && close < Low4 && close < Low5 && close < Low6)
                    color = _DownColor;
                else if (close > open)
                    color = _RangeUpColor;
                else if (close <= open)
                    color = _RangeDownColor;

            }


            if (HowMany == 7)
            {
                if (close > High1 && close > High2 && close > High3 && close > High4 && close > High5 && close > High6 && close > High7)
                    color = _UpColor;
                else if (close < Low1 && close < Low2 && close < Low3 && close < Low4 && close < Low5 && close < Low6 && close < Low7)
                    color = _DownColor;
                else if (close > open)
                    color = _RangeUpColor;
                else if (close <= open)
                    color = _RangeDownColor;

            }

            if (HowMany > 7)
            {
                if (close > High1 && close > High2 && close > High3 && close > High4 && close > High5 && close > High6 && close > High7 && close > High8)
                    color = _UpColor;
                else if (close < Low1 && close < Low2 && close < Low3 && close < Low4 && close < Low5 && close < Low6 && close < Low7 && close < Low8)
                    color = _DownColor;
                else if (close > open)
                    color = _RangeUpColor;
                else if (close <= open)
                    color = _RangeDownColor;

            }



























            ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
            ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);

        }
    }
}
