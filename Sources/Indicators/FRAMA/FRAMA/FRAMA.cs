//
//FRAMA by John Ehlers, Nonlinear Moving Average that adapts with help of the Hurst exponent
//
using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class FRAMA : Indicator
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("Period", DefaultValue = 16)]
        public int period { get; set; }

        [Parameter("Deviation", DefaultValue = 1)]
        public int deviation { get; set; }

        [Output("FRAMA", Color = Colors.Blue)]
        public IndicatorDataSeries frama { get; set; }

        [Output("UpperBand")]
        public IndicatorDataSeries upperband { get; set; }

        [Output("LowerBand")]
        public IndicatorDataSeries lowerband { get; set; }

        private double dimension;
        private double alpha;
        private double finalAlpha;
        private double upperBand;
        private double lowerBand;
        private double highLowRange3;

        private int halfPeriod;

        private IndicatorDataSeries halfPeriodHigh;
        private IndicatorDataSeries halfPeriodLow;
        private IndicatorDataSeries filter;
        private IndicatorDataSeries highLowRange1;
        private IndicatorDataSeries highLowRange2;

        private StandardDeviation stddev;

        protected override void Initialize()
        {
            halfPeriodHigh = CreateDataSeries();
            halfPeriodLow = CreateDataSeries();
            filter = CreateDataSeries();
            highLowRange1 = CreateDataSeries();
            highLowRange2 = CreateDataSeries();
            stddev = Indicators.StandardDeviation(Source, period, MovingAverageType.Simple);
        }

        public override void Calculate(int index)
        {
            if (index < period)
                return;

            halfPeriod = (int)period / 2;

            double highest = MarketSeries.High[index];
            double lowest = MarketSeries.Low[index];
            double highesthalf = MarketSeries.High[index];
            double lowesthalf = MarketSeries.Low[index];
            for (int i = 0; i <= period; i++)
            {
                if (MarketSeries.High[index - i] > highest)
                {
                    highest = MarketSeries.High[index - i];
                }
                if (MarketSeries.Low[index - i] < lowest)
                {
                    lowest = MarketSeries.Low[index - i];
                }
            }
            for (int i = 0; i <= halfPeriod; i++)
            {
                if (MarketSeries.High[index - i] > highesthalf)
                {
                    highesthalf = MarketSeries.High[index - i];
                }
                if (MarketSeries.Low[index - i] < lowesthalf)
                {
                    lowesthalf = MarketSeries.Low[index - i];
                }
            }
            highLowRange1[index] = (highest - lowest) / period;

            highLowRange2[index] = (highesthalf - lowesthalf) / halfPeriod;

            halfPeriodHigh[index] = MarketSeries.High[halfPeriod];
            halfPeriodLow[index] = MarketSeries.Low[halfPeriod];



            double highesthalfperiod = MarketSeries.High[index - halfPeriod];
            double lowesthalfperiod = MarketSeries.Low[index - halfPeriod];

            for (int i = 0; i <= halfPeriod; i++)
            {
                if (MarketSeries.High[index - halfPeriod - i] > highesthalfperiod)
                {
                    highesthalfperiod = MarketSeries.High[index - halfPeriod - i];
                }
                if (MarketSeries.Low[index - halfPeriod - i] < lowesthalfperiod)
                {
                    lowesthalfperiod = MarketSeries.Low[index - halfPeriod - i];
                }
            }
            highLowRange3 = (highesthalfperiod - lowesthalfperiod) / halfPeriod;
            if ((highLowRange1[index] > 0) && (highLowRange2[index] > 0) && (highLowRange3 > 0))
                dimension = (Math.Log(highLowRange3 + highLowRange2[index]) - Math.Log(highLowRange1[index])) / Math.Log(2);

            alpha = Math.Exp(-4.6 * (dimension - 1));

            if (alpha < 0.01)
                finalAlpha = 0.01;
            else
                finalAlpha = alpha;

            if (alpha > 1)
                finalAlpha = 1;
            else
                finalAlpha = alpha;

            if (!double.IsNaN(filter[index - 1]))
                filter[index] = finalAlpha * Source[index] + ((1 - finalAlpha) * filter[index - 1]);
            else
            {
                filter[index] = finalAlpha * Source[index] + (1 - finalAlpha);
            }
            upperband[index] = filter[index] + (deviation * stddev.Result[index]);
            lowerband[index] = filter[index] - (deviation * stddev.Result[index]);

            frama[index] = filter[index];
        }
    }
}
