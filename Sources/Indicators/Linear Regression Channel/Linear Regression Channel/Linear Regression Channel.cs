using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class LinearRegressionChannel : Indicator
    {
        [Parameter(DefaultValue = 200)]
        public int Bars { get; set; }

        [Parameter(DefaultValue = "Yellow")]
        public string Color { get; set; }

        [Parameter(DefaultValue = 1.0)]
        public double LineThickness { get; set; }

        [Parameter("Center", DefaultValue = true)]
        public bool ShowCenter { get; set; }

        [Parameter("Channel", DefaultValue = true)]
        public bool ShowChannel { get; set; }

        [Parameter("Standard deviation", DefaultValue = true)]
        public bool ShowDeviantion { get; set; }


        private Colors color;

        protected override void Initialize()
        {
            // Parse color from string, e.g. "Yellow", "Green", "Red". string must start with large letter, "Red" is valid, "red" - not.
            if (!Enum.TryParse(Color, out color))
                color = Colors.Yellow;
        }

        public override void Calculate(int index)
        {
            if (IsLastBar)
                LinearRegression(MarketSeries.Close);
        }

        private void LinearRegression(DataSeries series)
        {
            // Linear regresion

            double sum_x = 0, sum_x2 = 0, sum_y = 0, sum_xy = 0;

            int start = series.Count - Bars;
            int end = series.Count - 1;

            for (int i = start; i <= end; i++)
            {
                sum_x += 1.0 * i;
                sum_x2 += 1.0 * i * i;
                sum_y += series[i];
                sum_xy += series[i] * i;
            }

            double a = (Bars * sum_xy - sum_x * sum_y) / (Bars * sum_x2 - sum_x * sum_x);
            double b = (sum_y - a * sum_x) / Bars;


            // Calculate maximum and standard devaitions

            double maxDeviation = 0;
            double sumDevation = 0;

            for (int i = start; i <= end; i++)
            {
                double price = a * i + b;
                maxDeviation = Math.Max(Math.Abs(series[i] - price), maxDeviation);
                sumDevation += Math.Pow(series[i] - price, 2.0);
            }

            double stdDeviation = Math.Sqrt(sumDevation / Bars);

            // draw in future
            end += 20;

            double pr1 = a * start + b;
            double pr2 = a * end + b;

            if (ShowCenter)
            {
                ChartObjects.DrawLine("center", start, pr1, end, pr2, color, LineThickness, LineStyle.Lines);
            }

            if (ShowChannel)
            {
                ChartObjects.DrawLine("top", start, pr1 + maxDeviation, end, pr2 + maxDeviation, color, LineThickness, LineStyle.Solid);
                ChartObjects.DrawLine("bottom", start, pr1 - maxDeviation, end, pr2 - maxDeviation, color, LineThickness, LineStyle.Solid);
            }

            if (ShowDeviantion)
            {
                ChartObjects.DrawLine("dev-top", start, pr1 + stdDeviation, end, pr2 + stdDeviation, color, LineThickness, LineStyle.DotsVeryRare);
                ChartObjects.DrawLine("dev-bottom", start, pr1 - stdDeviation, end, pr2 - stdDeviation, color, LineThickness, LineStyle.DotsVeryRare);
            }
        }

    }
}
