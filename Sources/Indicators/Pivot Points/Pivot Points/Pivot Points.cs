using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.EasternStandardTime, AccessRights = AccessRights.None)]
    public class PivotPoints : Indicator
    {
        private DateTime _previousPeriodStartTime;
        private int _previousPeriodStartIndex;
        private TimeFrame PivotTimeFrame;
        private VerticalAlignment vAlignment = VerticalAlignment.Top;
        private HorizontalAlignment hAlignment = HorizontalAlignment.Right;

        Colors pivotColor = Colors.White;
        Colors supportColor = Colors.Red;
        Colors resistanceColor = Colors.Green;

        [Parameter("Show Labels", DefaultValue = true)]
        public bool ShowLabels { get; set; }

        [Parameter("Pivot Color", DefaultValue = "White")]
        public string PivotColor { get; set; }
        [Parameter("Support Color", DefaultValue = "Red")]
        public string SupportColor { get; set; }
        [Parameter("Resistance Color", DefaultValue = "Green")]
        public string ResistanceColor { get; set; }


        protected override void Initialize()
        {
            if (TimeFrame <= TimeFrame.Hour)
                PivotTimeFrame = TimeFrame.Daily;
            else if (TimeFrame < TimeFrame.Daily)
            {
                PivotTimeFrame = TimeFrame.Weekly;
            }
            else
                PivotTimeFrame = TimeFrame.Monthly;


            Enum.TryParse(PivotColor, out pivotColor);
            Enum.TryParse(SupportColor, out supportColor);
            Enum.TryParse(ResistanceColor, out resistanceColor);

        }

        private DateTime GetStartOfPeriod(DateTime dateTime)
        {
            return CutToOpenByNewYork(dateTime, PivotTimeFrame);
        }

        private DateTime GetEndOfPeriod(DateTime dateTime)
        {
            if (PivotTimeFrame == TimeFrame.Monthly)
            {
                return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1);
            }

            return AddPeriod(CutToOpenByNewYork(dateTime, PivotTimeFrame), PivotTimeFrame);
        }

        public override void Calculate(int index)
        {
            var currentPeriodStartTime = GetStartOfPeriod(MarketSeries.OpenTime[index]);
            if (currentPeriodStartTime == _previousPeriodStartTime)
                return;

            if (index > 0)
                CalculatePivots(_previousPeriodStartTime, _previousPeriodStartIndex, currentPeriodStartTime, index);

            _previousPeriodStartTime = currentPeriodStartTime;
            _previousPeriodStartIndex = index;
        }

        private void CalculatePivots(DateTime startTime, int startIndex, DateTime startTimeOfNextPeriod, int index)
        {
            var high = MarketSeries.High[startIndex];
            var low = MarketSeries.Low[startIndex];
            var close = MarketSeries.Close[startIndex];
            var i = startIndex + 1;

            while (GetStartOfPeriod(MarketSeries.OpenTime[i]) == startTime && i < MarketSeries.Close.Count)
            {
                high = Math.Max(high, MarketSeries.High[i]);
                low = Math.Min(low, MarketSeries.Low[i]);
                close = MarketSeries.Close[i];

                i++;
            }

            var pivotStartTime = startTimeOfNextPeriod;
            var pivotEndTime = GetEndOfPeriod(startTimeOfNextPeriod);

            var pivot = (high + low + close) / 3;

            var r1 = 2 * pivot - low;
            var s1 = 2 * pivot - high;

            var r2 = pivot + high - low;
            var s2 = pivot - high + low;

            var r3 = high + 2 * (pivot - low);
            var s3 = low - 2 * (high - pivot);

            ChartObjects.DrawLine("pivot " + startIndex, pivotStartTime, pivot, pivotEndTime, pivot, Colors.White);
            ChartObjects.DrawLine("r1 " + startIndex, pivotStartTime, r1, pivotEndTime, r1, Colors.Green);
            ChartObjects.DrawLine("r2 " + startIndex, pivotStartTime, r2, pivotEndTime, r2, Colors.Green);
            ChartObjects.DrawLine("r3 " + startIndex, pivotStartTime, r3, pivotEndTime, r3, Colors.Green);
            ChartObjects.DrawLine("s1 " + startIndex, pivotStartTime, s1, pivotEndTime, s1, Colors.Red);
            ChartObjects.DrawLine("s2 " + startIndex, pivotStartTime, s2, pivotEndTime, s2, Colors.Red);
            ChartObjects.DrawLine("s3 " + startIndex, pivotStartTime, s3, pivotEndTime, s3, Colors.Red);

            if (!ShowLabels)
                return;

            ChartObjects.DrawText("Lpivot " + startIndex, "P", index, pivot, vAlignment, hAlignment, pivotColor);
            ChartObjects.DrawText("Lr1 " + startIndex, "R1", index, r1, vAlignment, hAlignment, resistanceColor);
            ChartObjects.DrawText("Lr2 " + startIndex, "R2", index, r2, vAlignment, hAlignment, resistanceColor);
            ChartObjects.DrawText("Lr3 " + startIndex, "R3", index, r3, vAlignment, hAlignment, resistanceColor);
            ChartObjects.DrawText("Ls1 " + startIndex, "S1", index, s1, vAlignment, hAlignment, supportColor);
            ChartObjects.DrawText("Ls2 " + startIndex, "S2", index, s2, vAlignment, hAlignment, supportColor);
            ChartObjects.DrawText("Ls3 " + startIndex, "S3", index, s3, vAlignment, hAlignment, supportColor);


        }


        private static DateTime CutToOpenByNewYork(DateTime date, TimeFrame timeFrame)
        {
            if (timeFrame == TimeFrame.Daily)
            {
                var hourShift = (date.Hour + 24 - 17) % 24;
                return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0, DateTimeKind.Unspecified).AddHours(-hourShift);
            }

            if (timeFrame == TimeFrame.Weekly)
                return GetStartOfTheWeek(date);

            if (timeFrame == TimeFrame.Monthly)
            {
                return new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Unspecified);
            }

            throw new ArgumentException(string.Format("Unknown timeframe: {0}", timeFrame), "timeFrame");
        }

        private static DateTime GetStartOfTheWeek(DateTime dateTime)
        {
            return dateTime.Date.AddDays((double)DayOfWeek.Sunday - (double)dateTime.Date.DayOfWeek).AddHours(-7);
        }


        public DateTime AddPeriod(DateTime dateTime, TimeFrame timeFrame)
        {
            if (timeFrame == TimeFrame.Daily)
            {
                return dateTime.AddDays(1);
            }
            if (timeFrame == TimeFrame.Weekly)
            {
                return dateTime.AddDays(7);
            }
            if (timeFrame == TimeFrame.Monthly)
                return dateTime.AddMonths(1);

            throw new ArgumentException(string.Format("Unknown timeframe: {0}", timeFrame), "timeFrame");
        }

    }

//    static internal class DateTimeExtencions
//    {
//    }
}
