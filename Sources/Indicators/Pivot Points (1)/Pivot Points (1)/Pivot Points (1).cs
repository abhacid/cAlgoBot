using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class PivotPoints : Indicator
    {
        private enum Timeframe
        {
            m1,
            m2,
            m3,
            m4,
            m5,
            m10,
            m15,
            m30,
            h1,
            h4,
            h12,
            D1,
            W1,
            M1,
            na
        }

        private double _close;
        private double _high;
        private double _low;
        private bool _showDailyPivots;
        private bool _showWeeklyPivots;
        private bool _showMonthlyPivots;
        private Timeframe _currentTimeFrame = Timeframe.na;


        #region Output

        [Output("Pivot", LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries Pivot { get; set; }

        [Output("R1", LineStyle = LineStyle.Lines, Color = Colors.Blue)]
        public IndicatorDataSeries R1 { get; set; }

        [Output("R2", LineStyle = LineStyle.Lines, Color = Colors.Blue)]
        public IndicatorDataSeries R2 { get; set; }

        [Output("R3", LineStyle = LineStyle.Lines, Color = Colors.Blue)]
        public IndicatorDataSeries R3 { get; set; }


        [Output("S1", LineStyle = LineStyle.Lines, Color = Colors.Red)]
        public IndicatorDataSeries S1 { get; set; }

        [Output("S2", LineStyle = LineStyle.Lines, Color = Colors.Red)]
        public IndicatorDataSeries S2 { get; set; }

        [Output("S3", PlotType = PlotType.Line, LineStyle = LineStyle.Lines, Color = Colors.Red)]
        public IndicatorDataSeries S3 { get; set; }


        #endregion

        #region Input Parameters

        [Parameter("Number of Pivots", DefaultValue = 3, MinValue = 1, MaxValue = 3)]        
        public int NoPiv { get; set; }

        [Parameter("Daily", DefaultValue = 1, MinValue = 0, MaxValue = 1)]
        public int DailyPivots { get; set; }

        [Parameter("Weekly", DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int WeeklyPivots { get; set; }

        [Parameter("Monthly", DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int MonthlyPivots { get; set; }


        #endregion

        protected override void Initialize()
        {
            _showDailyPivots = DailyPivots == 1 && WeeklyPivots == 0 && MonthlyPivots == 0;
            _showWeeklyPivots = DailyPivots == 0 && WeeklyPivots == 1 && MonthlyPivots == 0;
            _showMonthlyPivots = DailyPivots == 0 && WeeklyPivots == 0 && MonthlyPivots == 1;
        }

        public override void Calculate(int index)
        {
            if(index <=1)
            {
                Pivot[index] = 0;
                return;
            }

            if (_currentTimeFrame == Timeframe.na)
                _currentTimeFrame = GetTimeFrame(index);
            if (_currentTimeFrame == Timeframe.na)
            {
                Pivot[index] = 0;
                return;
            }
            bool daily = _currentTimeFrame < Timeframe.D1 && _showDailyPivots;
            bool weekly = _currentTimeFrame < Timeframe.W1 && _showWeeklyPivots;
            bool monthly = _currentTimeFrame < Timeframe.M1 && _showMonthlyPivots;

            if (!(daily || weekly || monthly))
            {
                ChartObjects.DrawText("msg", "Incorrect Timeframe selection", StaticPosition.TopLeft, Colors.Red);
                return;
            }

            ChartObjects.RemoveAllObjects();
            int currentDay = MarketSeries.OpenTime[index].Day;
            int previousDay = MarketSeries.OpenTime[index - 1].Day;            

            DayOfWeek currentDayWk = MarketSeries.OpenTime[index].DayOfWeek;

            bool sameDay = currentDay == previousDay;
            bool calculateValues = Pivot[index - 1] == Pivot[index - 2] && !sameDay &&
                                   (daily && currentDayWk != DayOfWeek.Saturday
                                    || weekly && currentDayWk == DayOfWeek.Monday
                                    || monthly && currentDay == 1);
            
            if (!calculateValues) 
            {                
                // Same values for the day
                Pivot[index] = Pivot[index - 1];
                R1[index] = R1[index - 1];
                S1[index] = S1[index - 1];

                // Display additional pivots according to user input
                if (NoPiv >= 2)
                {
                    R2[index] = R2[index - 1];
                    S2[index] = S2[index - 1];
                }
                if (NoPiv >= 3)
                {
                    R3[index] = R3[index - 1];
                    S3[index] = S3[index - 1];
                }

                // Set high & low for next cycle
                if (MarketSeries.High[index] > _high)
                    _high = MarketSeries.High[index];
                if (MarketSeries.Low[index] < _low)
                    _low = MarketSeries.Low[index];
            }
            else //if (calculateValues)
            {                
                _close = MarketSeries.Close[index - 1];
                CalculatePivots(index);

                // reset high & low
                _high = double.MinValue;
                _low = double.MaxValue;
            }
        }
        
        private void CalculatePivots(int index)
        {
            // Calculate output            
            Pivot[index] = (_high + _low + _close)/3;
            R1[index] = 2*Pivot[index] - _low;
            S1[index] = 2*Pivot[index] - _high;

            // Display additional pivots according to input
            if (NoPiv >= 2)
            {
                R2[index] = Pivot[index] + (_high - _low);
                S2[index] = Pivot[index] - (_high - _low);
            }
            if (NoPiv >= 3)
            {
                R3[index] = _high + 2*(Pivot[index] - _low);
                S3[index] = _low - 2*(_high - Pivot[index]);
            }            
        }

        /// <summary>
        /// Get the current Timeframe
        /// </summary>
        /// <param name="index"> </param>
        /// <returns></returns>
        private Timeframe GetTimeFrame(int index)
        {
            
            DateTime currentOpenTime = MarketSeries.OpenTime[index];
            DateTime previousOpenTime = MarketSeries.OpenTime[index - 1];
            
            if (currentOpenTime.Day == 1 && previousOpenTime.Day == 1 && currentOpenTime.Month != previousOpenTime.Month)
                return Timeframe.M1;

            if (currentOpenTime.DayOfWeek == DayOfWeek.Monday && previousOpenTime.DayOfWeek != DayOfWeek.Monday)
            {
                currentOpenTime = previousOpenTime;
                previousOpenTime = MarketSeries.OpenTime[index - 2];                
            }

            TimeSpan barTimeDiff = currentOpenTime - previousOpenTime;
            var totalMin = (int) barTimeDiff.TotalMinutes;


            switch (totalMin)
            {
                case 1:
                    return Timeframe.m1;
                case 2:
                    return Timeframe.m2;
                case 3:
                    return Timeframe.m3;
                case 4:
                    return Timeframe.m4;
                case 5:
                    return Timeframe.m5;
                case 10:
                    return Timeframe.m10;
                case 15:
                    return Timeframe.m15;
                case 30:
                    return Timeframe.m30;
                case 60:
                    return Timeframe.h1;
                case 240:
                    return Timeframe.h4;
                case 720:
                    return Timeframe.h12;
                case 1440:
                    return Timeframe.D1;
                case 10080:
                    return Timeframe.W1;
                default:
                    return Timeframe.na;
            }
        }
    }
}
