using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class GAPO : Indicator
    {
        [Parameter("Period", DefaultValue = 10)]
        public int Period { get; set; }

        [Output("GAPO")]
        public IndicatorDataSeries Result { get; set; }

        public override void Calculate(int index)
        {
            Result[index] = Math.Log(High(index) - Low(index)) / Math.Log(Period);
        }

        /// <summary>
        /// Retrieve the Highest Price of the past Period
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private double High(int index)
        {
            double high = 0.0;

            for (int i = index - Period; i < index; i++)
            {
                if (MarketSeries.High[i] > high)
                    high = MarketSeries.High[i];
            }

            return high;
        }

        /// <summary>
        /// Retrieve the Lowest Price of the past Period
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private double Low(int index)
        {
            double low = MarketSeries.Low[index];

            for (int i = index - Period; i < index; i++)
            {
                if (MarketSeries.Low[i] < low)
                    low = MarketSeries.Low[i];
            }

            return low;
        }
    }
}
