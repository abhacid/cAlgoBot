/*
 
Aroon Up = 100 x (25 - Days Since 25-day High)/25
Aroon Down = 100 x (25 - Days Since 25-day Low)/25
Aroon Oscillator = Aroon-Up  -  Aroon-Down

 */

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Levels(-100,0,100)]
    [Indicator(AccessRights = AccessRights.None)]
    public class AroonOscilator : Indicator
    {
        private Aroon _aroon;

        [Parameter(DefaultValue = 25)]
        public int Period { get; set; }

        [Output("Positive", PlotType = PlotType.Histogram)]
        public IndicatorDataSeries Positive { get; set; }

        [Output("Negative", PlotType = PlotType.Histogram, Color = Colors.Red)]
        public IndicatorDataSeries Negative { get; set; }

        protected override void Initialize()
        {
            _aroon = Indicators.Aroon(Period);
        }


        public override void Calculate(int index)
        {
            double diff = _aroon.Up[index] - _aroon.Down[index];
            Positive[index] = diff > 0 ? diff : 0;
            Negative[index] = diff < 0 ? diff : 0;
        }
    }
}