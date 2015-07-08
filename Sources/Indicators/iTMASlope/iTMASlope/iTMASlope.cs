//#reference: AverageTrueRange.algo

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false)]
    public class iTMASlope : Indicator
    {
        private AverageTrueRange _averageTrueRange;
        private WeightedMovingAverage _weightedMovingAverage;
        private RelativeStrengthIndex _relativeStrengthIndex;

        [Parameter(DefaultValue = 2)]
        public int rsiPeriod { get; set; }

        [Output("TMA", Color = Colors.Gray, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries TMA { get; set; }

        [Output("RSI", Color = Colors.Blue)]
        public IndicatorDataSeries RSI { get; set; }

        protected override void Initialize()
        {
            _weightedMovingAverage = Indicators.WeightedMovingAverage(MarketSeries.Close, 21);
            _averageTrueRange = Indicators.GetIndicator<AverageTrueRange>(100);
            _relativeStrengthIndex = Indicators.RelativeStrengthIndex(TMA, rsiPeriod);
        }

        double calcPrevTrue(int index)
        {
            double dblSum = MarketSeries.Close[index - 1] * 21;
            double dblSumw = 21;
            int jnx, knx;

            dblSum += MarketSeries.Close[index] * 20;
            dblSumw += 20;

            for (jnx = 1,knx = 20; jnx <= 20; jnx++,knx--)
            {
                dblSum += MarketSeries.Close[index - 1 - jnx] * knx;
                dblSumw += knx;
            }
            return (dblSum / dblSumw);
        }


        public override void Calculate(int index)
        {
            double dblTma, dblPrev;
            double atr = _averageTrueRange.Result[index - 10] / 10;
            double gadblSlope = 0.0;

            if (atr != 0)
            {
                dblTma = _weightedMovingAverage.Result[index];
                dblPrev = calcPrevTrue(index);
                gadblSlope = (dblTma - dblPrev) / atr;
            }

            TMA[index] = gadblSlope;

            RSI[index] = _relativeStrengthIndex.Result[index];
        }
    }
}
