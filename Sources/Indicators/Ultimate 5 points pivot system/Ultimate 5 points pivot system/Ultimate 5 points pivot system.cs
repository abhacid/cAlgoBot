
// -------------------------------------------------------------------------------
//
//     
//    	This is an idicator to display daily, weekly and monthly pivot points according to the 5 points pivot system
//		This indicator was developed by Hichem MHAMED : mr.hichem@gmail.com
//		Please contact for feedback or feature requests.
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AutoRescale = false, AccessRights = AccessRights.None)]
    public class Ultimate5pointspivotsystem : Indicator
    {

        [Output("Daily Pivot", Color = Colors.Yellow)]
        public IndicatorDataSeries p { get; set; }
        [Output("Daily R1", Color = Colors.Green)]
        public IndicatorDataSeries r1 { get; set; }
        [Output("Daily R2", Color = Colors.Green)]
        public IndicatorDataSeries r2 { get; set; }
        [Output("Daily R3", Color = Colors.Green)]
        public IndicatorDataSeries r3 { get; set; }
        [Output("Daily S1", Color = Colors.Red)]
        public IndicatorDataSeries s1 { get; set; }
        [Output("Daily S2", Color = Colors.Red)]
        public IndicatorDataSeries s2 { get; set; }
        [Output("Daily S3", Color = Colors.Red)]
        public IndicatorDataSeries s3 { get; set; }

        [Output("Weekly Pivot", Color = Colors.AliceBlue)]
        public IndicatorDataSeries wp { get; set; }
        [Output("Weekly R1", Color = Colors.Brown)]
        public IndicatorDataSeries wr1 { get; set; }
        [Output("Weekly R2", Color = Colors.Brown)]
        public IndicatorDataSeries wr2 { get; set; }
        [Output("Weekly R3", Color = Colors.Brown)]
        public IndicatorDataSeries wr3 { get; set; }
        [Output("Weekly S1", Color = Colors.Purple)]
        public IndicatorDataSeries ws1 { get; set; }
        [Output("Weekly S2", Color = Colors.Purple)]
        public IndicatorDataSeries ws2 { get; set; }
        [Output("Weekly S3", Color = Colors.Purple)]
        public IndicatorDataSeries ws3 { get; set; }

        [Output("Monthly P", Color = Colors.Pink)]
        public IndicatorDataSeries mp { get; set; }
        [Output("Monthly R1", Color = Colors.Orange)]
        public IndicatorDataSeries mr1 { get; set; }
        [Output("Monthly R2", Color = Colors.Orange)]
        public IndicatorDataSeries mr2 { get; set; }
        [Output("Monthly R3", Color = Colors.Orange)]
        public IndicatorDataSeries mr3 { get; set; }
        [Output("Monthly S1", Color = Colors.Turquoise)]
        public IndicatorDataSeries ms1 { get; set; }
        [Output("Monthly S2", Color = Colors.Turquoise)]
        public IndicatorDataSeries ms2 { get; set; }
        [Output("Monthly S3", Color = Colors.Turquoise)]
        public IndicatorDataSeries ms3 { get; set; }



        [Parameter("Show daily pivots", DefaultValue = 1, MinValue = 0, MaxValue = 1)]
        public int ShowDailyPivots { get; set; }

        [Parameter("Show weekly pivots", DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int ShowWeeklyPivots { get; set; }

        [Parameter("Show monthly pivots", DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int ShowMonthlyPivots { get; set; }

        private double dailyLow = double.MaxValue;
        private double dailyHigh = double.MinValue;
        private double dailyClose;


        private double weeklyLow = double.MaxValue;
        private double weeklyHigh = double.MinValue;
        private double weeklyClose;

        private double monthlyLow = double.MaxValue;
        private double monthlyHigh = double.MinValue;
        private double monthlyClose;


        private int nameCounter = 0;

        private string currentTimeFrameName = "";

        //public  event EventHandler<EventArgs<int>>  CalcFinished = delegate {} ;

        protected override void Initialize()
        {


            // Initialize and create nested indicators
        }




        public override void Calculate(int index)
        {


            string timeFrame = GetTimeFrameName();

            if (timeFrame != "0" && timeFrame != currentTimeFrameName)
                currentTimeFrameName = timeFrame;



            bool dailyCondition = ShowDailyPivots == 1 && currentTimeFrameName != "D1" && currentTimeFrameName != "W1" && currentTimeFrameName != "M1";
            bool weeklyCondition = ShowWeeklyPivots == 1 && currentTimeFrameName != "W1" && currentTimeFrameName != "M1";
            bool monthlyCondition = ShowMonthlyPivots == 1 && currentTimeFrameName != "M1";

            //Print(currentTimeFrameName + ":" +dailyCondition);


            if (dailyCondition)
                calculateDailyPivots(index);
            if (weeklyCondition)
                calculateWeeklyPivots(index);
            if (monthlyCondition)
                calculateMonthlyPivots(index);




        }

        private void calculateMonthlyPivots(int index)
        {
            int currentDay = MarketSeries.OpenTime[index].Day;
            int previousDay = MarketSeries.OpenTime[index - 1].Day;
            if (currentDay != previousDay && currentDay == 1)
            {

                monthlyClose = MarketSeries.Close[index - 1];
                mp[index] = (monthlyHigh + monthlyLow + monthlyClose) / 3;
                mr1[index] = 2 * mp[index] - monthlyLow;
                mr2[index] = mp[index] + monthlyHigh - monthlyLow;
                ms1[index] = 2 * mp[index] - monthlyHigh;
                ms2[index] = mp[index] - monthlyHigh + monthlyLow;



                monthlyLow = double.MaxValue;
                monthlyHigh = double.MinValue;
            }
            else
            {
                if (monthlyLow > MarketSeries.Low[index])
                    monthlyLow = MarketSeries.Low[index];
                if (monthlyHigh < MarketSeries.High[index])
                    monthlyHigh = MarketSeries.High[index];

                mp[index] = mp[index - 1];
                mr1[index] = mr1[index - 1];
                ms1[index] = ms1[index - 1];

                mr2[index] = mr2[index - 1];
                ms2[index] = ms2[index - 1];

                mr3[index] = mr3[index - 1];
                ms3[index] = ms3[index - 1];
            }

        }

        private void calculateWeeklyPivots(int index)
        {
            DayOfWeek currentDay = MarketSeries.OpenTime[index].DayOfWeek;
            DayOfWeek previousDay = MarketSeries.OpenTime[index - 1].DayOfWeek;
            if (currentDay != previousDay && previousDay == DayOfWeek.Friday)
            {

                weeklyClose = MarketSeries.Close[index - 1];
                wp[index] = (weeklyHigh + weeklyLow + weeklyClose) / 3;
                wr1[index] = 2 * wp[index] - weeklyLow;
                wr2[index] = wp[index] + weeklyHigh - weeklyLow;
                ws1[index] = 2 * wp[index] - weeklyHigh;
                ws2[index] = wp[index] - weeklyHigh + weeklyLow;



                weeklyLow = double.MaxValue;
                weeklyHigh = double.MinValue;
            }
            else
            {
                if (weeklyLow > MarketSeries.Low[index])
                    weeklyLow = MarketSeries.Low[index];
                if (weeklyHigh < MarketSeries.High[index])
                    weeklyHigh = MarketSeries.High[index];

                wp[index] = wp[index - 1];
                wr1[index] = wr1[index - 1];
                ws1[index] = ws1[index - 1];

                wr2[index] = wr2[index - 1];
                ws2[index] = ws2[index - 1];

                wr3[index] = wr3[index - 1];
                ws3[index] = ws3[index - 1];
            }
        }

        protected void calculateDailyPivots(int index)
        {
            DayOfWeek currentDay = MarketSeries.OpenTime[index].DayOfWeek;
            DayOfWeek previousDay = MarketSeries.OpenTime[index - 1].DayOfWeek;

            bool cond1 = currentDay == previousDay || previousDay == DayOfWeek.Sunday;

            if (cond1)
            {

                if (dailyLow > MarketSeries.Low[index])
                    dailyLow = MarketSeries.Low[index];
                if (dailyHigh < MarketSeries.High[index])
                    dailyHigh = MarketSeries.High[index];

                p[index] = p[index - 1];
                r1[index] = r1[index - 1];
                s1[index] = s1[index - 1];

                r2[index] = r2[index - 1];
                s2[index] = s2[index - 1];

                r3[index] = r3[index - 1];
                s3[index] = s3[index - 1];


            }
            else if (!cond1)
            {

                dailyClose = MarketSeries.Close[index - 1];
                p[index] = (dailyHigh + dailyLow + dailyClose) / 3;
                r1[index] = 2 * p[index] - dailyLow;
                r2[index] = p[index] + dailyHigh - dailyLow;
                s1[index] = 2 * p[index] - dailyHigh;
                s2[index] = p[index] - dailyHigh + dailyLow;



                dailyLow = double.MaxValue;
                dailyHigh = double.MinValue;

            }
        }

        /// <summary>
        /// Get the time span between two consecutive bars OpenTime
        /// </summary>
        private TimeSpan GetTimeFrame()
        {
            if (MarketSeries.Close.Count > 0)
            {
                int currentIndex = MarketSeries.Close.Count - 1;
                DateTime currentOpenTime = MarketSeries.OpenTime[currentIndex];
                DateTime previousOpenTime = MarketSeries.OpenTime[currentIndex - 1];

                TimeSpan timeFrame = currentOpenTime - previousOpenTime;

                if (currentOpenTime.DayOfWeek == DayOfWeek.Monday && previousOpenTime.DayOfWeek != DayOfWeek.Monday)
                {
                    currentOpenTime = previousOpenTime;
                    previousOpenTime = MarketSeries.OpenTime[currentIndex - 2];
                    timeFrame = currentOpenTime - previousOpenTime;
                }

                return timeFrame;
            }
            // if bars are not available
            return TimeSpan.Zero;
        }

        /// <summary>
        /// Get the name representation of the timeframe used
        /// </summary>
        /// <param name="timeFrame">Time span between two consecutive bars OpenTime</param>
        /// <returns>The name representation of the TimeFrame</returns>
        private string GetTimeFrameName()
        {
            TimeSpan timeFrame = GetTimeFrame();
            int totalMin = (int)timeFrame.TotalMinutes;
            string timeFrameName;

            if (totalMin > 10080)
                timeFrameName = "M1";
            else
            {
                switch (totalMin)
                {
                    case 1:
                        timeFrameName = "m1";
                        break;
                    case 2:
                        timeFrameName = "m2";
                        break;
                    case 3:
                        timeFrameName = "m3";
                        break;
                    case 4:
                        timeFrameName = "m4";
                        break;
                    case 5:
                        timeFrameName = "m5";
                        break;
                    case 10:
                        timeFrameName = "m10";
                        break;
                    case 15:
                        timeFrameName = "m15";
                        break;
                    case 30:
                        timeFrameName = "m30";
                        break;
                    case 60:
                        timeFrameName = "h1";
                        break;
                    case 240:
                        timeFrameName = "h4";
                        break;
                    case 720:
                        timeFrameName = "h12";
                        break;
                    case 1440:
                        timeFrameName = "D1";
                        break;
                    case 10080:
                        timeFrameName = "W1";
                        break;
                    default:
                        timeFrameName = "0";
                        break;

                }
            }

            return timeFrameName;
        }



    }
}
