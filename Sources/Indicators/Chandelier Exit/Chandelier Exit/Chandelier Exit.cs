using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ChandelierExit : Indicator
    {
        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 3)]
        public double AtrMultiplier { get; set; }

        [Output("LongExit", PlotType = PlotType.Points, Color = Colors.Blue, Thickness = 3)]
        public IndicatorDataSeries LongExit { get; set; }

        [Output("ShortExit", PlotType = PlotType.Points, Color = Colors.Red, Thickness = 3)]
        public IndicatorDataSeries ShortExit { get; set; }


        private AverageTrueRange _atr;

        protected override void Initialize()
        {
            _atr = Indicators.AverageTrueRange(Period, MovingAverageType.Exponential);
        }

        public override void Calculate(int index)
        {
            var highestHigh = MarketSeries.High.Maximum(Period);
            var lowestLow = MarketSeries.Low.Minimum(Period);
            var adjustedAtr = _atr.Result[index] * AtrMultiplier;

            var longExit = highestHigh - adjustedAtr;
            var shortExit = lowestLow + adjustedAtr;

            if (!double.IsNaN(LongExit[index - 1]))
            {
                if (MarketSeries.Close[index - 1] < LongExit[index - 1])
                    ShortExit[index] = shortExit;
                else
                    LongExit[index] = longExit;
            }
            else
            {
                if (MarketSeries.Close[index - 1] > ShortExit[index - 1])
                    LongExit[index] = longExit;
                else
                    ShortExit[index] = shortExit;
            }
        }
    }
}
