using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AutoRescale = false, TimeZone = TimeZones.EEuropeStandardTime, AccessRights = AccessRights.None)]
    public class PivotPointsDemark : Indicator
    {
        public override void Calculate(int index)
        {
            DateTime today = MarketSeries.OpenTime[index].Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            
            // Adjust for Monday
            if (today.DayOfWeek == DayOfWeek.Monday)
            {
                yesterday = today.AddDays(-3);
            }

            double high = double.MaxValue;
            double low = double.MinValue;
            double close = 0;
            double open = 0;

            // previous days high, low & close
            for (int i = index; i > 2; i--)
            {               
                if (MarketSeries.OpenTime[i].Date < yesterday)
                    break;
                
                if (MarketSeries.OpenTime[i].Date == today && MarketSeries.OpenTime[i - 1].Date == yesterday)
                {
                    high = MarketSeries.High[i - 1];
                    low = MarketSeries.Low[i - 1];
                    close = MarketSeries.Close[i - 1];
                    open = MarketSeries.Open[i];
                    continue;
                }
                // Adjust for Monday
                if (MarketSeries.OpenTime[i].Date == today && MarketSeries.OpenTime[i - 2].Date == yesterday)
                {
                    high = MarketSeries.High[i - 2];
                    low = MarketSeries.Low[i - 2];
                    close = MarketSeries.Close[i - 2];
                    open = MarketSeries.Open[i];
                    continue;
                }

                high = Math.Max(high, MarketSeries.High[i]);
                low = Math.Min(low, MarketSeries.Low[i]);
            }

            // Calculate output
            double x;
            if (close < open)
                x = high + 2*low + close;
            else if (close > open)
                x = 2*high + low + close;
            else
                x = high + low + 2*close;

            double pivot = x/4;
            double r1 = x / 2 - low;
            double s1 = x / 2 - high;
            double r2 = pivot + (.618 * (high - low));
            double s2 = pivot - (.618 * (high - low));
            double r3 = pivot + (high - low);
            double s3 = pivot - (high - low);

            // Plot output
            ChartObjects.DrawLine("p" + today, today, pivot, tomorrow, pivot, Colors.Lime);

            ChartObjects.DrawLine("r1" + today, today, r1, tomorrow, r1, Colors.Blue);
            ChartObjects.DrawLine("s1" + today, today, s1, tomorrow, s1, Colors.Red);

            ChartObjects.DrawLine("r2" + today, today, r2, tomorrow, r2, Colors.Blue);
            ChartObjects.DrawLine("s2" + today, today, s2, tomorrow, s2, Colors.Red);

            ChartObjects.DrawLine("r3" + today, today, r3, tomorrow, r3, Colors.Blue);
            ChartObjects.DrawLine("s3" + today, today, s3, tomorrow, s3, Colors.Red);

        }
    }
}