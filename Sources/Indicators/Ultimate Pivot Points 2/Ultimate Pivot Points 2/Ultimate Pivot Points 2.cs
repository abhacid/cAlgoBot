
// -------------------------------------------------------------------------------
//      Based on Ultimate 5 points pivot system
//      by hichem
//      [http://ctdn.com/algos/indicators/show/211]
//      The Padding Output Colors and line style should be the same as the regular 
//      corresponding ones (the name is the same plus "Pad"
//      Currently only for Less than H12 Timeframe
//      Check back for updates.
//      If you will use this as a reference indicator note that the last values of each line
//      are a different IndicatorDataSeries - e.g. p and P (this was to accomplish a discontinued line)
//      
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AutoRescale = false, AccessRights = AccessRights.None)]
    public class UltimatePivotPoints2 : Indicator
    {
        #region output
        [Output("Daily Pivot", Color = Colors.Yellow, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries p { get; set; }

        [Output("Daily R1", Color = Colors.Green, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries r1 { get; set; }

        [Output("Daily R2", Color = Colors.Green, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries r2 { get; set; }

        [Output("Daily R3", Color = Colors.Green, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries r3 { get; set; }

        [Output("Daily S1", Color = Colors.Red, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries s1 { get; set; }

        [Output("Daily S2", Color = Colors.Red, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries s2 { get; set; }

        [Output("Daily S3", Color = Colors.Red, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries s3 { get; set; }

        #endregion

        #region padding


        [Output("Daily Pivot Pad", Color = Colors.Yellow, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries P { get; set; }

        [Output("Daily R1 Pad", Color = Colors.Green, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries R1 { get; set; }

        [Output("Daily R2 Pad", Color = Colors.Green, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries R2 { get; set; }

        [Output("Daily R3 Pad", Color = Colors.Green, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries R3 { get; set; }

        [Output("Daily S1 Pad", Color = Colors.Red, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries S1 { get; set; }

        [Output("Daily S2 Pad", Color = Colors.Red, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries S2 { get; set; }

        [Output("Daily S3 Pad", Color = Colors.Red, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries S3 { get; set; }

        #endregion



        private double dailyLow = double.MaxValue;
        private double dailyHigh = double.MinValue;
        private double dailyClose;


        public override void Calculate(int index)
        {
            if (index == 0)
                return;


            bool dailyCondition = TimeFrame < TimeFrame.Hour12;


            if (dailyCondition)
                calculateDailyPivots(index);
            else
            {
                ChartObjects.DrawText("message", "Choose  TF < H 12", StaticPosition.TopCenter, Colors.Red);
            }


            if (MarketSeries.OpenTime[index].DayOfWeek == DayOfWeek.Saturday || MarketSeries.OpenTime[index].DayOfWeek == DayOfWeek.Sunday)
            {
                // overwrite weekend values
                p[index] = double.NaN;
                r1[index] = double.NaN;
                r2[index] = double.NaN;
                r3[index] = double.NaN;
                s1[index] = double.NaN;
                s2[index] = double.NaN;
                s3[index] = double.NaN;

            }

        }

        protected void calculateDailyPivots(int index)
        {
            DayOfWeek currentDay = MarketSeries.OpenTime[index].DayOfWeek;
            DayOfWeek previousDay = MarketSeries.OpenTime[index - 1].DayOfWeek;

            bool cond1 = currentDay == previousDay;

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
            else
            {
                dailyClose = MarketSeries.Close[index - 1];

                p[index] = (dailyHigh + dailyLow + dailyClose) / 3;

                r1[index] = 2 * p[index] - dailyLow;
                s1[index] = 2 * p[index] - dailyHigh;

                r2[index] = p[index] + dailyHigh - dailyLow;
                s2[index] = p[index] - dailyHigh + dailyLow;

                r3[index] = dailyHigh + 2 * (p[index] - dailyLow);
                s3[index] = dailyLow - 2 * (dailyHigh - p[index]);

                dailyLow = double.MaxValue;
                dailyHigh = double.MinValue;

                // Make Discontinuous

                // Pad last values
                P[index] = P[index - 1] = P[index - 2] = p[index - 1];

                R1[index] = R1[index - 1] = R1[index - 2] = r1[index - 1];
                R2[index] = R2[index - 1] = R2[index - 2] = r2[index - 1];
                R3[index] = R3[index - 1] = R3[index - 2] = r3[index - 1];

                S1[index] = S1[index - 1] = S1[index - 2] = s1[index - 1];
                S2[index] = S2[index - 1] = S2[index - 2] = s2[index - 1];
                S3[index] = S3[index - 1] = S3[index - 2] = s3[index - 1];


                // overwrite last value
                p[index - 1] = double.NaN;

                r1[index - 1] = double.NaN;
                r2[index - 1] = double.NaN;
                r3[index - 1] = double.NaN;

                s1[index - 1] = double.NaN;
                s2[index - 1] = double.NaN;
                s3[index - 1] = double.NaN;


            }
        }

    }

}
