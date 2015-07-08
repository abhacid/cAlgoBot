using System;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator("Average Daily Range", ScalePrecision = 5, TimeZone = TimeZones.EEuropeStandardTime, AccessRights = AccessRights.None)]
    public class AverageDailyRange : Indicator
    {
        private MarketSeries dailySeries;

        [Parameter("Period", DefaultValue = 100)]
        public int Length { get; set; }

        [Output("Main", PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            dailySeries = MarketData.GetSeries(Symbol, TimeFrame.Daily);
        }

        public override void Calculate(int index)
        {

            int dailyIndex = GetIndexByDate(dailySeries, MarketSeries.OpenTime[index]);
            if (dailyIndex > 0)
                Result[index] = AverageRange(dailySeries, dailyIndex);
            else
            {
                dailyIndex = GetClosestIndexByDate(dailySeries, MarketSeries.OpenTime[index]);
                Result[index] = AverageRange(dailySeries, dailyIndex);

            }
        }


        private double AverageRange(MarketSeries marketSeries, int index)
        {
            double sum = 0;

            for (int i = index - Length; i <= index; i++)
            {
                double high = marketSeries.High[i];
                double low = marketSeries.Low[i];
                sum += (high - low);
            }

            return sum / Length;

        }

        private int GetIndexByDate(MarketSeries series, DateTime time)
        {
            var lastBar = series.Close.Count - 1;
            for (int i = lastBar; i > 0; i--)
            {
                if (time == series.OpenTime[i])
                    return i;
            }
            return -1;
        }

        private int GetClosestIndexByDate(MarketSeries series, DateTime time)
        {
            var lastIndex = series.Close.Count - 1;

            if (time >= series.OpenTime[lastIndex])
                return lastIndex;

            var timeDifference = time.Subtract(series.OpenTime[0]);

            int index = 0;

            for (int i = 0; i < lastIndex - 1; i++)
            {
                if (time < series.OpenTime[i])
                    break;
                var currDiff = time.Subtract(series.OpenTime[i]);

                if (currDiff < timeDifference)
                {
                    timeDifference = currDiff;
                    index = i;
                }

            }

            return index;
        }


    }
}
