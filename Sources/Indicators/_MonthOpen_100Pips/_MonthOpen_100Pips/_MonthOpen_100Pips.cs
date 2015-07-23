using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MonthSnake : Indicator
    {

        private MovingAverage expo;

        [Output("Open", Color = Colors.YellowGreen, PlotType = PlotType.Line, Thickness = 5)]
        public IndicatorDataSeries Open { get; set; }

        [Parameter("MA", DefaultValue = 50)]
        public int EnvelopePeriod { get; set; }

        [Parameter("MAType", DefaultValue = 5)]
        public MovingAverageType matype { get; set; }

        [Output("Main", Color = Colors.Yellow, LineStyle = LineStyle.LinesDots, Thickness = 1)]
        public IndicatorDataSeries EnvelopeMain { get; set; }

        [Parameter("Show 100PipsLevels", DefaultValue = 1)]
        public bool Set100Levels { get; set; }

        [Parameter("MinLevel", DefaultValue = 0, MinValue = 0)]
        public int MinLevel { get; set; }

        [Parameter("MaxLevel", DefaultValue = 200, MinValue = 2)]
        public int MaxLevel { get; set; }


        public double openprice = 0;

        protected override void Initialize()
        {

            expo = Indicators.MovingAverage(MarketSeries.Close, EnvelopePeriod, matype);

        }

        public override void Calculate(int index)
        {

            EnvelopeMain[index] = expo.Result[index];

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

            double Pips = 0;
            if (Symbol.Ask > openprice)
                Pips = (Symbol.Ask - openprice) / Symbol.PipSize;

            if (Symbol.Ask < openprice)
                Pips = (openprice - Symbol.Ask) / Symbol.PipSize;

            double Profit = (Pips / 100) * 1000;


            var name1 = "Open";
            var text1 = "Open: " + openprice.ToString() + " Ask: " + Symbol.Ask + "\n Pips: " + (int)Pips + "\n Month Open Profit (1 Lot): " + (int)Profit + " USD";
            var staticPos = StaticPosition.TopRight;
            var color = Colors.Yellow;
            ChartObjects.DrawText(name1, text1, staticPos, color);



            var name12 = "Pips";
            var text12 = "Pips: " + (int)Pips + "\n Month Open Profit (1.0 Lot): " + (int)Profit + " USD (Initial Deposit 10000$)" + "\n Month Open Profit (0.10 Lot): " + (int)Profit / 10 + " USD (Initial Deposit 1000$)" + "\n Month Open Profit (0.01 Lot): " + (int)Profit / 100 + " USD (Initial Deposit 100$)  \n Profit = (Pips / 100) * 1000USD for 1Lot(100000) ";
            var staticPos12 = StaticPosition.BottomRight;
            var color12 = Colors.YellowGreen;
            ChartObjects.DrawText(name12, text12, staticPos12, color12);


            if (Set100Levels && MinLevel < MaxLevel)
            {
                for (int i = MinLevel; i < MaxLevel; i++)
                {
                    ChartObjects.DrawHorizontalLine("Level" + i, i * 100 * Symbol.PipSize, Colors.DodgerBlue, 2, LineStyle.Solid);
                }
            }


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

            int index1 = MarketSeries.OpenTime.Count - 1;

            // Day change
            //if (openTime.Day != lastOpenTime.Day)


            if (currentTime.Month == currentTime.Month && previousTime.Month != currentTime.Month)
            {
                // Plot Open
                Open[index] = MarketSeries.Open[index];
                openprice = MarketSeries.Open[index];
            }
            // Same Day
            else
            {
                // Plot Open
                Open[index] = Open[index - 1];
                //openprice = MarketSeries.Open[index];
            }

            // Plot todays close
            DateTime today = DateTime.Now.Date;
            if (openTime.Date != today)
                return;
        }
    }
}
