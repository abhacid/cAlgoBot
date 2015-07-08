using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class TrendLinesIndicator : Indicator
    {
        [Parameter(DefaultValue = 30, MinValue = 14)]
        public int Period { get; set; }

        protected override void Initialize()
        {
            RedrawLines();
        }

        public override void Calculate(int index)
        {
            if (IsRealTime)
                RedrawLines();
        }

        private void RedrawLines()
        {
            int count = MarketSeries.Close.Count;

            int maxIndex1 = FindNextLocalExtremum(MarketSeries.High, count - 1, true);
            int maxIndex2 = FindNextLocalExtremum(MarketSeries.High, maxIndex1 - Period, true);

            int minIndex1 = FindNextLocalExtremum(MarketSeries.Low, count - 1, false);
            int minIndex2 = FindNextLocalExtremum(MarketSeries.Low, minIndex1 - Period, false);

            int startIndex = Math.Min(maxIndex2, minIndex2) - 100;
            int endIndex = count + 100;

            DrawTrendLine("high", startIndex, endIndex, maxIndex1, MarketSeries.High[maxIndex1], maxIndex2, MarketSeries.High[maxIndex2]);

            DrawTrendLine("low", startIndex, endIndex, minIndex1, MarketSeries.Low[minIndex1], minIndex2, MarketSeries.Low[minIndex2]);
        }

        private void DrawTrendLine(string lineName, int startIndex, int endIndex, int index1, double value1, int index2, double value2)
        {
            double gradient = (value2 - value1) / (index2 - index1);

            double startValue = value1 + (startIndex - index1) * gradient;
            double endValue = value1 + (endIndex - index1) * gradient;

            ChartObjects.DrawLine(lineName, startIndex, startValue, endIndex, endValue, Colors.Gray);
            ChartObjects.DrawLine(lineName + "_red", index1, value1, index2, value2, Colors.Red);
        }

        private int FindNextLocalExtremum(DataSeries series, int maxIndex, bool findMax)
        {
            for (int index = maxIndex; index >= 0; index--)
            {
                if (IsLocalExtremum(series, index, findMax))
                {
                    return index;
                }
            }
            return 0;
        }

        private bool IsLocalExtremum(DataSeries series, int index, bool findMax)
        {
            int end = Math.Min(index + Period, series.Count - 1);
            int start = Math.Max(index - Period, 0);

            double value = series[index];

            for (int i = start; i < end; i++)
            {
                if (findMax && value < series[i])
                    return false;

                if (!findMax && value > series[i])
                    return false;
            }
            return true;
        }
    }
}
