using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class RoundNumbers : Indicator
    {
        [Parameter(DefaultValue = 100)]
        public int StepPips { get; set; }

        protected override void Initialize()
        {
            double max = MarketSeries.High.Maximum(MarketSeries.High.Count);
            double min = MarketSeries.Low.Minimum(MarketSeries.Low.Count);

            double step = Symbol.PipSize*StepPips;
            double start = Math.Floor(min/step)*step;

            for (double level = start; level <= max + step; level += step)
            {
                ChartObjects.DrawHorizontalLine("line_" + level, level, Colors.Gray);
            }
        }

        public override void Calculate(int index)
        {
        }
    }
}