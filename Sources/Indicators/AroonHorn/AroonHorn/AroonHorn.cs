using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class AroonHorn : Indicator
    {
        [Parameter(DefaultValue = 10)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 25)]
        public int Filter { get; set; }

        [Output("AroonOSCup", Color = Colors.Blue, IsHistogram = true)]
        public IndicatorDataSeries aroonoscup { get; set; }

        [Output("AroonOSCdown", Color = Colors.Red, IsHistogram = true)]
        public IndicatorDataSeries aroonoscdown { get; set; }

        [Output("AroonOSCGray", Color = Colors.Gray, IsHistogram = true)]
        public IndicatorDataSeries aroonoscgray { get; set; }

        [Output("AroonOSC", Color = Colors.Turquoise)]
        public IndicatorDataSeries aroonosc { get; set; }

        private Aroon aroonhorn;

        protected override void Initialize()
        {
            aroonhorn = Indicators.Aroon(Period);
        }

        public override void Calculate(int index)
        {
            double commander = 10 * (aroonhorn.Up[index] - aroonhorn.Down[index]) / Period;
            if ((commander >= 0) && (commander > Filter))
            {
                aroonoscup[index] = commander;
            }
            if ((commander < 0) && (commander < (-1) * Filter))
            {
                aroonoscdown[index] = commander;
            }
            if ((commander >= (-1) * Filter) && (commander <= Filter))
            {
                aroonoscgray[index] = commander;
            }
            aroonosc[index] = commander;
        }
    }
}