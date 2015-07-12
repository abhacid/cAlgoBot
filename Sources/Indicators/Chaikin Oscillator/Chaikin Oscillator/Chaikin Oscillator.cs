// -------------------------------------------------------------------------------
//
//      The Chaikin oscillator is formed by subtracting a 10-day exponential moving average 
//      from a 3-day exponential moving average of the accumulation/distribution index.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Levels(0.0)]
    [Indicator("Chaikin Oscillator", IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ChaikinOscillator : Indicator
    {
        private IndicatorDataSeries accDist;
        private ExponentialMovingAverage ema10;
        private ExponentialMovingAverage ema3;
        private MarketSeries marketSeriesDaily;


        [Output("Chaikin Oscillator")]
        public IndicatorDataSeries Result { get; set; }


        protected override void Initialize()
        {
            // Initialize and create nested indicators
            accDist = CreateDataSeries();
            ema3 = Indicators.ExponentialMovingAverage(accDist, 3);
            ema10 = Indicators.ExponentialMovingAverage(accDist, 10);

            marketSeriesDaily = MarketData.GetSeries(TimeFrame.Daily);



        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index

            if (index < 10)
            {
                accDist[index] = 0;
                return;
            }

            if (TimeFrame != TimeFrame.Daily)
            {
                DisplayTFMessage();

                CalculateAccumulationDistribution(marketSeriesDaily, index);
                DisplayInCorrectIndex(index);

                return;
            }

            // Timeframe Daily
            CalculateAccumulationDistribution(MarketSeries, index);

            Result[index] = ema3.Result[index] - ema10.Result[index];

        }


        private void DisplayTFMessage()
        {
            const string errorMsg = "Please choose Daily Timeframe";
            ChartObjects.DrawText("ErrorMessage", errorMsg, StaticPosition.TopLeft, Colors.Red);
        }


        private void CalculateAccumulationDistribution(MarketSeries series, int index)
        {
            double close = series.Close[index];
            double low = series.Low[index];
            double high = series.High[index];
            double volume = marketSeriesDaily.TickVolume[index];

            if (!high.Equals(low))
            {
                double clv = ((close - low) - (high - close)) / (high - low);
                accDist[index] = accDist[index - 1] + volume * clv;
            }
            else
                accDist[index] = accDist[index - 1];


        }

        /// <summary>
        /// Display in correct index
        /// </summary>
        /// <param name="index"></param>
        private void DisplayInCorrectIndex(int index)
        {

            var indexDaily = GetIndexByDate(marketSeriesDaily, MarketSeries.OpenTime[index]);
            Print(index);
            Print(MarketSeries.OpenTime[index]);
            Print(indexDaily);

            if (indexDaily != -1)
                Result[index] = ema3.Result[indexDaily] - ema10.Result[indexDaily];
        }

        private int GetIndexByDate(MarketSeries series, DateTime time)
        {
            for (int i = series.Close.Count - 1; i > 0; i--)
            {
                if (time == series.OpenTime[i])
                    return i;
            }
            return -1;
        }

    }
}
