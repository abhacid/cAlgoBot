// -----------------------------------------------------------------------------------------------
//
//    This is an example of how to plot the Daily Open and Close on a TimeFrame lower than Daily 
//
// -----------------------------------------------------------------------------------------------

using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class DailyOpenClose : Indicator
    {
        [Output("Open", Color = Colors.Wheat, PlotType = PlotType.Points)]
        public IndicatorDataSeries Open { get; set; }

        [Output("Close", Color = Colors.Blue, PlotType = PlotType.Points)]
        public IndicatorDataSeries Close { get; set; }

        public override void Calculate(int index)
        {
            if (index < 1)
            {
                // If first bar is first bar of the day set open
                if (MarketSeries.OpenTime[index].TimeOfDay == TimeSpan.Zero)
                    Open[index] = MarketSeries.Open[index];
                return;
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
            double close;
            int i;

            // Day change
            if (openTime.Day != lastOpenTime.Day)
            {
                // Plot Open
                Open[index] = MarketSeries.Open[index];

                // Plot previous day close                
                close = MarketSeries.Close[index - 1];
                i = index - 1;
                while (MarketSeries.OpenTime[i].Day == lastOpenTime.Day)
                {
                    Close[i] = close;
                    i--;
                }
            }
            // Same Day
            else
            {
                // Plot Open 
                Open[index] = Open[index - 1];
            }

            // Plot todays close 
            DateTime today = DateTime.Now.Date;
            if (openTime.Date != today)
                return;

            close = MarketSeries.Close[index];
            i = index;
            while (MarketSeries.OpenTime[i].Date == today)
            {
                Close[i] = close;
                i--;
            }
        }
    }
}
