using cAlgo.API;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Fractals:Indicator
    {
        [Parameter(DefaultValue = 5, MinValue = 5)]
        public int Period { get; set; }

        [Output("Up Fractal", Color = Colors.Red, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries UpFractal { get; set; }

        [Output("Down Fractal", Color = Colors.Blue, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries DownFractal { get; set; }

        
        
        
        public override void Calculate(int index)
        {
            if (index < Period)
                return;

            DrawUpFractal(index);
            DrawDownFractal(index);

        }

        private void DrawUpFractal(int index)
        {
            int period = Period % 2 == 0 ? Period - 1 : Period;
            int middleIndex = index - period / 2;
            double middleValue = MarketSeries.High[middleIndex];

            bool up = true;

            for (int i = 0; i < period; i++)
            {
                if (middleValue < MarketSeries.High[index - i])
                {
                    up = false;
                    break;
                }
            }
            if (up)
                UpFractal[middleIndex] = middleValue;

        }

        private void DrawDownFractal(int index)
        {
            int period = Period % 2 == 0 ? Period - 1 : Period;
            int middleIndex = index - period / 2;
            double middleValue = MarketSeries.Low[middleIndex];
            bool down = true;

            for (int i = 0; i < period; i++)
            {
                if (middleValue > MarketSeries.Low[index - i])
                {
                    down = false;
                    break;
                }
            }
            if (down)
                DownFractal[middleIndex] = middleValue;

        }


    }
}
