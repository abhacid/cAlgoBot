using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Snake3 : Indicator
    {

        [Output("OpenDay", Color = Colors.Red, PlotType = PlotType.Line, Thickness = 1)]
        public IndicatorDataSeries OpenDay { get; set; }

        [Output("OpenWeek", Color = Colors.Green, PlotType = PlotType.Line, Thickness = 2)]
        public IndicatorDataSeries OpenWeek { get; set; }

        [Output("OpenMonth", Color = Colors.DodgerBlue, PlotType = PlotType.Line, Thickness = 3)]
        public IndicatorDataSeries OpenMonth { get; set; }



        public double openprice1 = 0;
        public double openprice2 = 0;
        public double openprice3 = 0;

        public override void Calculate(int index)
        {

            if (index < 1)
            {
                // If first bar is first bar of the day set open
                if (MarketSeries.OpenTime[index].TimeOfDay == TimeSpan.Zero)
                {
                    OpenWeek[index] = MarketSeries.Open[index];
                    OpenMonth[index] = MarketSeries.Open[index];
                    OpenDay[index] = MarketSeries.Open[index];
                    return;
                }
            }

            DateTime openTime = MarketSeries.OpenTime[index];
            DateTime lastOpenTime = MarketSeries.OpenTime[index - 1];
            const string objectName = "messageNA";

            if (!ApplicableTimeFrame(openTime, lastOpenTime))
            {
                // Display message that timeframe is N/A
                const string text = "TimeFrame Not Applicable. Choose a lower Timeframe";
                ChartObjects.DrawText(objectName, text, StaticPosition.TopLeft, Colors.Red);
                return;
            }

            // If TimeFrame chosen is applicable remove N/A message
            ChartObjects.RemoveObject(objectName);

            // Plot Daily Open and Close
            PlotDailyOpenClose(openTime, lastOpenTime, index);

            var name1 = "Dn";
            var text1 = "Day Open : " + openprice1.ToString() + " \nWeek Open: " + openprice2.ToString() + " \nMonth Open:" + openprice3.ToString();
            var staticPos = StaticPosition.TopRight;
            var color = Colors.Yellow;
            ChartObjects.DrawText(name1, text1, staticPos, color);

        }

        private bool ApplicableTimeFrame(DateTime openTime, DateTime lastOpenTime)
        {
            // minutes difference between bars
            var timeFrameMinutes = (int)(openTime - lastOpenTime).TotalMinutes;

            bool daily = timeFrameMinutes == 1440;
            bool weeklyOrGreater = timeFrameMinutes >= 7200;

            bool timeFrameNotApplicable = daily || weeklyOrGreater;

            if (timeFrameNotApplicable)
                return false;

            return true;
        }

        private void PlotDailyOpenClose(DateTime openTime, DateTime lastOpenTime, int index)
        {

            DateTime currentTime = MarketSeries.OpenTime[MarketSeries.OpenTime.Count - 1];
            DateTime previousTime = MarketSeries.OpenTime[MarketSeries.OpenTime.Count - 2];



            // Day change
            if (openTime.Day != lastOpenTime.Day)
            {
                // Plot Open
                OpenDay[index] = MarketSeries.Open[index];
                openprice1 = OpenDay[index];

            }
            // Same Day
            else
            {
                // Plot Open
                OpenDay[index] = OpenDay[index - 1];
                openprice1 = OpenDay[index];
            }




            // Week change
            if (currentTime.DayOfWeek == DayOfWeek.Monday && previousTime.DayOfWeek != DayOfWeek.Monday)
            {
                // Plot Open
                OpenWeek[index] = MarketSeries.Open[index];
                openprice2 = OpenWeek[index];
            }
            // Same Day
            else
            {
                // Plot Open
                OpenWeek[index] = OpenWeek[index - 1];
                openprice2 = OpenWeek[index];
            }

            // Month
            if (currentTime.Month == currentTime.Month && previousTime.Month != currentTime.Month)
            {
                // Plot Open
                OpenMonth[index] = MarketSeries.Open[index];
                openprice3 = OpenMonth[index];
            }
            // Same Day
            else
            {
                // Plot Open
                OpenMonth[index] = OpenMonth[index - 1];
                openprice3 = OpenMonth[index];
            }

            // Plot todays close
            DateTime today = DateTime.Now.Date;
            if (openTime.Date != today)
                return;

        }
    }
}
