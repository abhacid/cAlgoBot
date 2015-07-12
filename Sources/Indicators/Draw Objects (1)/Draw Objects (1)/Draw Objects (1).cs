using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class DrawObjects : Indicator
    {
        private string upArrow = "▲";
        private string downArrow = "▼";
        private string diamond = "♦";
        private string bullet = "●";
        private string stop = "x";

        private const VerticalAlignment vAlign = VerticalAlignment.Top;
        private const HorizontalAlignment hAlign = HorizontalAlignment.Center;
        Colors colorDown = Colors.Fuchsia;
        Colors colorUp = Colors.Green;
        Colors colorStop = Colors.Yellow;

        private double arrowOffset;

        protected override void Initialize()
        {
            arrowOffset = Symbol.PipSize * 5;
        }
        public override void Calculate(int index)
        {
            int x = index;
            double y;
            string arrowName;

            var volume = MarketSeries.TickVolume[index];
            var volume1 = MarketSeries.TickVolume[index - 1];
            double volume2 = MarketSeries.TickVolume[index - 2];
            var high = MarketSeries.High[index];
            var low = MarketSeries.Low[index];
            var close = MarketSeries.Close[index];
            double close1 = MarketSeries.Close[index - 1];
            double close2 = MarketSeries.Close[index - 2];
            var currentHighMinusLow = high - low;
            var previousHighMinusLow = MarketSeries.High[index - 1] - MarketSeries.Low[index - 1];

            bool sellStop = close2 < close1 && close1 < close && volume2 < volume1 && volume1 < volume;
            bool buyStop = close2 > close1 && close1 > close && volume2 < volume1 && volume1 < volume;

            if (sellStop)
            {
                arrowName = string.Format("bulletSell {0}", index);
                y = high + arrowOffset;
                ChartObjects.DrawText(arrowName, bullet, x, y, vAlign, hAlign, Colors.Orange);
                y = high + arrowOffset * 2;
                arrowName = string.Format("diamondSell {0}", index);
                ChartObjects.DrawText(arrowName, diamond, x, y, vAlign, hAlign, colorDown);
            }
            else if (buyStop)
            {
                arrowName = string.Format("bulletBuy {0}", index);
                y = low - arrowOffset;
                ChartObjects.DrawText(arrowName, bullet, x, y, vAlign, hAlign, colorUp);
                arrowName = string.Format("diamondBuy {0}", index);
                y = low - arrowOffset * 2;
                ChartObjects.DrawText(arrowName, diamond, x, y, vAlign, hAlign, colorUp);
            }

            arrowName = string.Format("Arrow {0}", index);

            if (currentHighMinusLow > previousHighMinusLow)
            {
                if (volume > volume1)
                {
                    if (high - close < close - low)
                    {

                        y = low - arrowOffset;
                        ChartObjects.DrawText(arrowName, upArrow, x, y, vAlign, hAlign, colorUp);
                    }
                    else if (high - close > close - low)
                    {
                        y = high + arrowOffset;
                        ChartObjects.DrawText(arrowName, downArrow, x, y, vAlign, hAlign, colorDown);
                    }

                }
            }
            else if (currentHighMinusLow < previousHighMinusLow)
            {
                if (volume > volume1)
                {
                    y = high + arrowOffset;
                    ChartObjects.DrawText(arrowName, stop, x, y, vAlign, hAlign, colorStop);
                }
            }

        }
    }
}
