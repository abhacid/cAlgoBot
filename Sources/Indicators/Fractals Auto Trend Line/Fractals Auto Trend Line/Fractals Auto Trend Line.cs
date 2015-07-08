using System;
using cAlgo.API;
using cAlgo.API.Internals;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class FractalsAutoTrendLine : Indicator
    {
        [Parameter(DefaultValue = 48, MinValue = 3)]
        public int Calc_Max_Bars_For_TL { get; set; }

        [Parameter(DefaultValue = 4, MinValue = 3)]
        public int Period { get; set; }

        [Output("Up Fractal", Color = Colors.Red, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries UpFractal { get; set; }

        [Output("Down Fractal", Color = Colors.Blue, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries DownFractal { get; set; }

        //High Line
        private int index_high_1 = 0;
        private int index_high_2 = 0;

        //Low Line
        private int index_low_1 = 0;
        private int index_low_2 = 0;

        public override void Calculate(int index)
        {
            index_high_1 = 0;
            index_high_2 = 0;
            index_low_1 = 0;
            index_low_2 = 0;

            for (int calc = (MarketSeries.Close.Count - 1); calc > (MarketSeries.Close.Count - Calc_Max_Bars_For_TL); calc--)
            {
                DrawUpFractal(calc);
                DrawDownFractal(calc);
            }
            int count = MarketSeries.Close.Count;

            int maxIndex1 = index_high_1;
            int maxIndex2 = index_high_2;

            int minIndex1 = index_low_1;
            int minIndex2 = index_low_2;

            int startIndex = Math.Min(maxIndex2, minIndex2) - 100;
            int endIndex = count + 100;

            if (index_high_1 > 0 && index_high_2 > 0)
            {
                DrawTrendLine("Down Trend", startIndex, endIndex, maxIndex1, MarketSeries.High[maxIndex1], maxIndex2, MarketSeries.High[maxIndex2], true);
            }
            if (index_low_1 > 0 && index_low_2 > 0)
            {
                DrawTrendLine("Up Trend", startIndex, endIndex, minIndex1, MarketSeries.Low[minIndex1], minIndex2, MarketSeries.Low[minIndex2], false);
            }

            return;

        }

        private void DrawTrendLine(string lineName, int startIndex, int endIndex, int index1, double value1, int index2, double value2, bool color_up_trend)
        {
            double gradient = (value2 - value1) / (index2 - index1);

            double startValue = value1 + (startIndex - index1) * gradient;
            double endValue = value1 + (endIndex - index1) * gradient;
            if (color_up_trend == true)
            {
                ChartObjects.DrawLine(lineName, index2, value2, endIndex, endValue, Colors.Red, 2, LineStyle.Solid);
            }
            else
            {
                ChartObjects.DrawLine(lineName, index2, value2, endIndex, endValue, Colors.Blue, 2, LineStyle.Solid);
            }
        }

        private void DrawUpFractal(int index)
        {
            //int period = Period % 2 == 0 ? Period - 1 : Period;
            int period = Period;
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
            if (up == true)
            {
                if (index_high_1 == 0)
                {
                    index_high_1 = middleIndex;
                }
                else
                {
                    if (index_high_2 == 0 && MarketSeries.High[index_high_1] < MarketSeries.High[middleIndex])
                    {
                        index_high_2 = middleIndex;
                    }
                }
                UpFractal[middleIndex] = middleValue;
            }
        }

        private void DrawDownFractal(int index)
        {
            //int period = Period % 2 == 0 ? Period - 1 : Period;
            int period = Period;
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
            if (down == true)
            {
                if (index_low_1 == 0)
                {
                    index_low_1 = middleIndex;
                }
                else
                {
                    if (index_low_2 == 0 && MarketSeries.Low[index_low_1] > MarketSeries.Low[middleIndex])
                    {
                        index_low_2 = middleIndex;
                    }
                }
                DownFractal[middleIndex] = middleValue;
            }
        }


    }
}
