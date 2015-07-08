using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class VelaEspecial : Indicator
    {
        public IndicatorDataSeries _Open;
        public IndicatorDataSeries _Close;
        public double Open;
        public double Close;
        public double Low;
        public double High;

        [Parameter("Candle width", DefaultValue = 20)]
        public int CandleWidth { get; set; }

        [Parameter("Up color", DefaultValue = "Green")]
        public string UpColor { get; set; }

        [Parameter("Down color", DefaultValue = "Pink")]
        public string DownColor { get; set; }

        private Colors _upColor;
        private Colors _downColor;
        private bool _incorrectColors;
        private Random _random = new Random();

        protected override void Initialize()
        {
            _Open = CreateDataSeries();
            _Close = CreateDataSeries();

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
            if ((Server.Time.Minute == 4 || Server.Time.Minute == 9 || Server.Time.Minute == 14 || Server.Time.Minute == 19 || Server.Time.Minute == 24 || Server.Time.Minute == 29 || Server.Time.Minute == 34 || Server.Time.Minute == 39 || Server.Time.Minute == 44 || Server.Time.Minute == 49 || Server.Time.Minute == 54 || Server.Time.Minute == 59) && Server.Time.Second > 55)
            {
                var open = MarketSeries.Open[index];
                var high = MarketSeries.High[index];
                var low = MarketSeries.Low[index];
                var close = MarketSeries.Close[index];

                var open1 = MarketSeries.Open[index - 1];
                var high1 = MarketSeries.High[index - 1];
                var low1 = MarketSeries.Low[index - 1];
                var close1 = MarketSeries.Close[index - 1];

                var open2 = MarketSeries.Open[index - 2];
                var high2 = MarketSeries.High[index - 2];
                var low2 = MarketSeries.Low[index - 2];
                var close2 = MarketSeries.Close[index - 2];

                var open3 = MarketSeries.Open[index - 3];
                var high3 = MarketSeries.High[index - 3];
                var low3 = MarketSeries.Low[index - 3];
                var close3 = MarketSeries.Close[index - 3];

                var open4 = MarketSeries.Open[index - 4];
                var high4 = MarketSeries.High[index - 4];
                var low4 = MarketSeries.Low[index - 4];
                var close4 = MarketSeries.Close[index - 4];

                Close = close;
                double Open;
                if (index > 0)
                    Open = open4;
                else
                    Open = open4;

                High = Math.Max(Math.Max(Math.Max(Math.Max(high, high1), high2), high3), high4);
                Low = Math.Min(Math.Min(Math.Min(Math.Min(low, low1), low2), low3), low4);

                var color = Open > Close ? _downColor : _upColor;
                ChartObjects.DrawLine("candle" + index, index, Open, index, Close, color, CandleWidth, LineStyle.Solid);
                ChartObjects.DrawLine("line" + index, index, High, index, Low, color, 1, LineStyle.Solid);

                _Open[index] = Open;
                _Close[index] = Close;
            }
        }
    }
}
