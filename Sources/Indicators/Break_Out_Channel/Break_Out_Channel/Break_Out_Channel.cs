using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class Break_Out_Channel : Indicator
    {
//-----------------------------------------------------------------     
        [Output("Up", Color = Colors.DarkCyan, PlotType = PlotType.Line, LineStyle = LineStyle.LinesDots, Thickness = 1)]
        public IndicatorDataSeries Up { get; set; }

        [Output("Down", Color = Colors.DarkCyan, PlotType = PlotType.Line, LineStyle = LineStyle.LinesDots, Thickness = 1)]
        public IndicatorDataSeries Down { get; set; }

        [Output("Mid", Color = Colors.DarkViolet, PlotType = PlotType.Line, LineStyle = LineStyle.LinesDots, Thickness = 1)]
        public IndicatorDataSeries Mid { get; set; }
//----------------------------------------------------------------- 
        public override void Calculate(int index)
        {

            if (index < 5)
                return;

            if (MarketSeries.Close[index] < Up[index - 1] & MarketSeries.Close[index] > Down[index - 1])
            {
                Up[index] = Up[index - 1];
                Down[index] = Down[index - 1];
            }
            else
            {
                Up[index] = MarketSeries.High[index];
                Down[index] = MarketSeries.Low[index];
            }

            Mid[index] = (Up[index] + Down[index]) / 2;
        }
    }
}
