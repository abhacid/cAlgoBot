using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{

    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class InsideBarPatternRecognition : Indicator
    {
        [Output("Up Point", Color = Colors.Orange, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries UpPoint { get; set; }
        [Output("Down Point", Color = Colors.Orange, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries DownPoint { get; set; }

        public override void Calculate(int index)
        {
            var motherCandleHigh = MarketSeries.High.Last(2);
            var motherCandleLow = MarketSeries.Low.Last(2);
            var motherCandleOpen = MarketSeries.Open.Last(2);
            var motherCandleClose = MarketSeries.Close.Last(2);

            var childCandleHigh = MarketSeries.High.Last(1);
            var childCandleLow = MarketSeries.Low.Last(1);
            var childCandleOpen = MarketSeries.Open.Last(1);
            var childCandleClose = MarketSeries.Close.Last(1);

            if (childCandleHigh < motherCandleHigh && childCandleLow > motherCandleLow && Math.Abs(motherCandleOpen - motherCandleClose) > Math.Abs(childCandleOpen - childCandleClose))
                DrawPoint(index);
        }
// Draws a point next to the parent bar
        private void DrawPoint(int index)
        {
            UpPoint[index - 2] = MarketSeries.High[index - 2] + 0.0005;
            DownPoint[index - 2] = MarketSeries.Low[index - 2] - 0.0005;
        }
    }
}
