using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Levels(-20, -50, -80)]
    [Indicator(ScalePrecision = 2, AccessRights = AccessRights.None)]
    public class WPRIndicator : Indicator
    {
        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Output("Main", Color = Colors.CornflowerBlue)]
        public IndicatorDataSeries Result { get; set; }


        public override void Calculate(int index)
        {
            double max = MarketSeries.High.Maximum(Period);
            double min = MarketSeries.Low.Minimum(Period);
            double close = MarketSeries.Close[index];

            if ((max - min) > 0)
                Result[index] = -100 * (max - close) / (max - min);
            else
                Result[index] = 0.0;

        }
    }
}
