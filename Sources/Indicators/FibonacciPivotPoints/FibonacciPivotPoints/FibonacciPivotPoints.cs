//#reference: AverageTrueRange.algo


using System;
using cAlgo.API;
using cAlgo.API.Indicators;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AutoRescale = false)]
    public class FibonacciPivotPoints : Indicator
    {

        [Output("Daily Pivot", Color = Colors.Yellow)]
        public IndicatorDataSeries p { get; set; }
        [Output("Daily R1", Color = Colors.Green)]
        public IndicatorDataSeries r1 { get; set; }
        [Output("Daily R2", Color = Colors.Green)]
        public IndicatorDataSeries r2 { get; set; }
        [Output("Daily R3", Color = Colors.Green)]
        public IndicatorDataSeries r3 { get; set; }
        [Output("Daily R4", Color = Colors.Green)]
        public IndicatorDataSeries r4 { get; set; }
        [Output("Daily R5", Color = Colors.Green)]
        public IndicatorDataSeries r5 { get; set; }
        [Output("Daily R6", Color = Colors.Green)]
        public IndicatorDataSeries r6 { get; set; }
        [Output("Daily R7", Color = Colors.Green)]
        public IndicatorDataSeries r7 { get; set; }
        [Output("Daily R8", Color = Colors.Green)]
        public IndicatorDataSeries r8 { get; set; }
        [Output("Daily R9", Color = Colors.Green)]
        public IndicatorDataSeries r9 { get; set; }

        [Output("Daily S1", Color = Colors.Red)]
        public IndicatorDataSeries s1 { get; set; }
        [Output("Daily S2", Color = Colors.Red)]
        public IndicatorDataSeries s2 { get; set; }
        [Output("Daily S3", Color = Colors.Red)]
        public IndicatorDataSeries s3 { get; set; }
        [Output("Daily S4", Color = Colors.Red)]
        public IndicatorDataSeries s4 { get; set; }
        [Output("Daily S5", Color = Colors.Red)]
        public IndicatorDataSeries s5 { get; set; }
        [Output("Daily S6", Color = Colors.Red)]
        public IndicatorDataSeries s6 { get; set; }
        [Output("Daily S7", Color = Colors.Red)]
        public IndicatorDataSeries s7 { get; set; }
        [Output("Daily S8", Color = Colors.Red)]
        public IndicatorDataSeries s8 { get; set; }
        [Output("Daily S9", Color = Colors.Red)]
        public IndicatorDataSeries s9 { get; set; }




        [Parameter("Atr Period", DefaultValue = 100, MinValue = 1, MaxValue = 200)]
        public int atrPeriod { get; set; }
        [Parameter("Atr Multiplay", DefaultValue = 2, MinValue = 0, MaxValue = 5)]
        public int atrMultiplay { get; set; }


        private double dailyLow = double.MaxValue;
        private double dailyHigh = double.MinValue;
        private double dailyClose;
        private AverageTrueRange _averageTrueRange;



        private int nameCounter = 0;

        private string currentTimeFrameName = "";

        //public  event EventHandler<EventArgs<int>>  CalcFinished = delegate {} ;

        protected override void Initialize()
        {


            _averageTrueRange = Indicators.GetIndicator<AverageTrueRange>(atrPeriod);
        }




        public override void Calculate(int index)
        {


            string timeFrame = GetTimeFrameName();


            if (timeFrame != "0" && timeFrame != currentTimeFrameName)
                currentTimeFrameName = timeFrame;



            bool dailyCondition = currentTimeFrameName != "D1" && currentTimeFrameName != "W1" && currentTimeFrameName != "M1";

            //Print(currentTimeFrameName + ":" +dailyCondition);


            if (dailyCondition)
                calculateDailyPivots(index);




        }


        protected void calculateDailyPivots(int index)
        {
            DayOfWeek currentDay = MarketSeries.OpenTime[index].DayOfWeek;
            DayOfWeek previousDay = MarketSeries.OpenTime[index - 1].DayOfWeek;
            double atr = _averageTrueRange.Result[index] * atrMultiplay;
            bool cond1 = currentDay == previousDay || previousDay == DayOfWeek.Sunday;

            if (cond1)
            {

                if (dailyLow > MarketSeries.Low[index])
                    dailyLow = MarketSeries.Low[index];
                if (dailyHigh < MarketSeries.High[index])
                    dailyHigh = MarketSeries.High[index];

                p[index] = p[index - 1];

                s1[index] = s1[index - 1];
                s2[index] = s2[index - 1];
                s3[index] = s3[index - 1];
                s4[index] = s4[index - 1];
                s5[index] = s5[index - 1];
                s6[index] = s6[index - 1];
                s7[index] = s7[index - 1];
                s8[index] = s8[index - 1];
                s9[index] = s9[index - 1];

                r1[index] = r1[index - 1];
                r2[index] = r2[index - 1];
                r3[index] = r3[index - 1];
                r4[index] = r4[index - 1];
                r5[index] = r5[index - 1];
                r6[index] = r6[index - 1];
                r7[index] = r7[index - 1];
                r8[index] = r8[index - 1];
                r9[index] = r9[index - 1];





            }
            else if (!cond1)
            {

                dailyClose = MarketSeries.Close[index - 1];
                p[index] = (dailyHigh + dailyLow + dailyClose) / 3;
                r1[index] = p[index] + atr * 0.382;
                r2[index] = p[index] + atr * 0.618;
                r3[index] = p[index] + atr * 0.82;
                r4[index] = p[index] + atr * 1.382;
                r5[index] = p[index] + atr * 1.618;
                r6[index] = p[index] + atr * 2.236;
                r7[index] = p[index] + atr * 2.618;
                r8[index] = p[index] + atr * 3.0;
                r9[index] = p[index] + atr * 3.618;

                s1[index] = p[index] - atr * 0.382;
                s2[index] = p[index] - atr * 0.618;
                s3[index] = p[index] - atr * 0.82;
                s4[index] = p[index] - atr * 1.382;
                s5[index] = p[index] - atr * 1.618;
                s6[index] = p[index] - atr * 2.236;
                s7[index] = p[index] - atr * 2.618;
                s8[index] = p[index] - atr * 3.0;
                s9[index] = p[index] - atr * 3.618;


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
