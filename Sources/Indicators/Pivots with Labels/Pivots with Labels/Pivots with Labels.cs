using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Pivots : Indicator
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

        private double _high;
        private double _low;
        private bool _dailyPivots;
        private bool _weeklyPivots;
        private Timeframe _currentTimeFrame = Timeframe.na;
        private IndicatorDataSeries _p;
        private IndicatorDataSeries _r1;
        private IndicatorDataSeries _s1;
        private IndicatorDataSeries _r2;
        private IndicatorDataSeries _s2;
        private IndicatorDataSeries _r3;
        private IndicatorDataSeries _s3;
        private bool _isNewPeriod;

        private bool _showLabels;
        private bool _showPivots;

        #region Output

        [Output("Pivot", LineStyle = LineStyle.Lines, Color = Colors.Blue, Thickness = 2)]
        public IndicatorDataSeries Pivot { get; set; }

        [Output("R1", LineStyle = LineStyle.Lines, Color = Colors.Green, Thickness = 2)]
        public IndicatorDataSeries R1 { get; set; }

        [Output("R2", LineStyle = LineStyle.Lines, Color = Colors.Green, Thickness = 2)]
        public IndicatorDataSeries R2 { get; set; }

        [Output("R3", LineStyle = LineStyle.Lines, Color = Colors.Green, Thickness = 2)]
        public IndicatorDataSeries R3 { get; set; }


        [Output("S1", LineStyle = LineStyle.Lines, Color = Colors.Red, Thickness = 2)]
        public IndicatorDataSeries S1 { get; set; }

        [Output("S2", LineStyle = LineStyle.Lines, Color = Colors.Red, Thickness = 2)]
        public IndicatorDataSeries S2 { get; set; }

        [Output("S3", LineStyle = LineStyle.Lines, Color = Colors.Red, Thickness = 2)]
        public IndicatorDataSeries S3 { get; set; }


        #endregion

        #region Input Parameters

        [Parameter("Draw Labels", DefaultValue = 1, MinValue = 0, MaxValue = 1)]
        public int ShowLabels { get; set; }

        [Parameter("Draw Pivots", DefaultValue = 1, MinValue = 0, MaxValue = 1)]
        public int ShowPivots { get; set; }

        [Parameter("Daily", DefaultValue = 1, MinValue = 0, MaxValue = 1)]
        public int DailyPivots { get; set; }

        [Parameter("Weekly", DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int WeeklyPivots { get; set; }


        #endregion

        /// <summary>
        /// indicator initialization function
        /// </summary>
        protected override void Initialize()
        {
            _dailyPivots = DailyPivots == 1 && WeeklyPivots == 0;
            _weeklyPivots = DailyPivots == 0 && WeeklyPivots == 1;
            _showLabels = ShowLabels == 1;
            _showPivots = ShowPivots == 1;

            _p = CreateDataSeries();
            _r1 = CreateDataSeries();
            _s1 = CreateDataSeries();
            _r2 = CreateDataSeries();
            _s2 = CreateDataSeries();
            _r3 = CreateDataSeries();
            _s3 = CreateDataSeries();
        }

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="index"></param>
        public override void Calculate(int index)
        {
            if (!(_showPivots || _showLabels))
            {
                ChartObjects.DrawText("msgShow", "Set \"Draw Labels\" or \"Draw Pivots\" to 1", StaticPosition.TopLeft, Colors.Red);
                return;
            }
            ChartObjects.RemoveObject("msgShow");
            if (index <= 1)
            {
                _p[index] = 0;
                return;
            }

            if (GetTimeFrame(index) == Timeframe.na && _currentTimeFrame == Timeframe.na)
            {
                _p[index] = 0;
                return;
            }

            if (_currentTimeFrame == Timeframe.na)
                _currentTimeFrame = GetTimeFrame(index);

            bool daily = _currentTimeFrame < Timeframe.D1 && _dailyPivots;
            bool weekly = _currentTimeFrame < Timeframe.W1 && _weeklyPivots;

            if (!(daily || weekly))
            {
                ChartObjects.DrawText("msg", "Incorrect Timeframe selection", StaticPosition.TopLeft, Colors.Red);
                return;
            }

            ChartObjects.RemoveObject("msg");

            CalculatePivots(index, daily, weekly);

            if (ShowPivots == 1)
                DrawPivots(index);


        }

        /// <summary>
        /// Calculate pivots
        /// </summary>
        /// <param name="index"></param>
        /// <param name="daily"></param>
        /// <param name="weekly"></param>
        private void CalculatePivots(int index, bool daily, bool weekly)
        {
            int currentDay = MarketSeries.OpenTime[index].Day;
            int previousDay = MarketSeries.OpenTime[index - 1].Day;

            DayOfWeek currentDayWk = MarketSeries.OpenTime[index].DayOfWeek;

            bool sameDay = currentDay == previousDay;

            _isNewPeriod = _p[index - 1] == _p[index - 2] && !sameDay && (daily && currentDayWk != DayOfWeek.Saturday || weekly && currentDayWk == DayOfWeek.Monday);

            if (_isNewPeriod)
            {
                // Calculate new values
                double close = MarketSeries.Close[index - 1];

                _p[index] = (_high + _low + close) / 3;

                _r1[index] = 2 * _p[index] - _low;
                _s1[index] = 2 * _p[index] - _high;

                _r2[index] = _p[index] + (_high - _low);
                _s2[index] = _p[index] - (_high - _low);

                _r3[index] = _high + 2 * (_p[index] - _low);
                _s3[index] = _low - 2 * (_high - _p[index]);

                _high = double.MinValue;
                _low = double.MaxValue;

                if (ShowLabels == 1)
                    DrawLabels(index);

            }
            else
            {
                _p[index] = _p[index - 1];
                _r1[index] = _r1[index - 1];
                _s1[index] = _s1[index - 1];

                _r2[index] = _r2[index - 1];
                _s2[index] = _s2[index - 1];

                _r3[index] = _r3[index - 1];
                _s3[index] = _s3[index - 1];

                if (MarketSeries.High[index] > _high)
                    _high = MarketSeries.High[index];
                if (MarketSeries.Low[index] < _low)
                    _low = MarketSeries.Low[index];
            }
        }

        /// <summary>
        /// Display Pivots
        /// </summary>
        /// <param name="index"></param>
        private void DrawPivots(int index)
        {
            Pivot[index] = _p[index];
            R1[index] = _r1[index];
            S1[index] = _s1[index];

            R2[index] = _r2[index];
            S2[index] = _s2[index];

            R3[index] = _r3[index];
            S3[index] = _s3[index];
        }

        /// <summary>
        /// Display pivot value at index position
        /// </summary>
        /// <param name="index"></param>
        private void DrawLabels(int index)
        {
            int xValue = index;
            double yValue = Math.Round(_p[index], Symbol.Digits);
            string objectName = string.Format("P {0}", yValue);
            ChartObjects.DrawText(objectName, objectName, xValue, yValue, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Blue);

            yValue = Math.Round(_r1[index], Symbol.Digits);
            objectName = string.Format("R1 {0}", yValue);
            ChartObjects.DrawText(objectName, objectName, xValue, yValue, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Green);

            yValue = Math.Round(_s1[index], Symbol.Digits);
            objectName = string.Format("S1 {0}", yValue);
            ChartObjects.DrawText(objectName, objectName, xValue, yValue, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Red);

            yValue = Math.Round(_r2[index], Symbol.Digits);
            objectName = string.Format("R2 {0}", yValue);
            ChartObjects.DrawText(objectName, objectName, xValue, yValue, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Green);

            yValue = Math.Round(_s2[index], Symbol.Digits);
            objectName = string.Format("S2 {0}", yValue);
            ChartObjects.DrawText(objectName, objectName, xValue, yValue, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Red);

            yValue = Math.Round(_r3[index], Symbol.Digits);
            objectName = string.Format("R3 {0}", yValue);
            ChartObjects.DrawText(objectName, objectName, xValue, yValue, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Green);

            yValue = Math.Round(_s3[index], Symbol.Digits);
            objectName = string.Format("S3 {0}", yValue);
            ChartObjects.DrawText(objectName, objectName, xValue, yValue, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Red);


        }

        /// <summary>
        /// Get the current chart timeframe
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
            var totalMin = (int)barTimeDiff.TotalMinutes;


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
