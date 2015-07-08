using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.EEuropeStandardTime, AccessRights = AccessRights.None)]
    public class CentralPivotRange : Indicator
    {
        private int GetStartDayIndex(int index)
        {
            var startDayIndex = index;
            while (startDayIndex > 0 && MarketSeries.OpenTime[startDayIndex].Date == MarketSeries.OpenTime[index].Date)
                startDayIndex--;
            return ++startDayIndex;
        }

        private double GetHigh(int fromIndex, int toIndex)
        {
            var result = MarketSeries.High[fromIndex];
            for (var i = fromIndex; i <= toIndex; i++)
                result = Math.Max(result, MarketSeries.High[i]);
            return result;
        }

        private double GetLow(int fromIndex, int toIndex)
        {
            var result = MarketSeries.Low[fromIndex];
            for (var i = fromIndex; i <= toIndex; i++)
                result = Math.Min(result, MarketSeries.Low[i]);
            return result;
        }

        public override void Calculate(int index)
        {
            if (TimeFrame >= TimeFrame.Daily)
                return;

            var todayStartIndex = GetStartDayIndex(index);
            if (todayStartIndex == 0)
                return;

            var yesterdayStartIndex = GetStartDayIndex(todayStartIndex - 1);
            var yHigh = GetHigh(yesterdayStartIndex, todayStartIndex - 1);
            var yLow = GetLow(yesterdayStartIndex, todayStartIndex - 1);
            var yClose = MarketSeries.Close[todayStartIndex - 1];

            var pivot = (yHigh + yLow + yClose) / 3;
            var bc = (yHigh + yLow) / 2;
            var tc = pivot - bc + pivot;

            ChartObjects.DrawLine("pivot" + todayStartIndex, todayStartIndex, pivot, index + 1, pivot, Colors.Yellow);
            ChartObjects.DrawLine("bc" + todayStartIndex, todayStartIndex, bc, index + 1, bc, Colors.Yellow);
            ChartObjects.DrawLine("tc" + todayStartIndex, todayStartIndex, tc, index + 1, tc, Colors.Yellow);
        }
    }
}
