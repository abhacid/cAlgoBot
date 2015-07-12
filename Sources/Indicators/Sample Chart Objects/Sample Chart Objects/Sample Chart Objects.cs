using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class SampleChartObjects : Indicator
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Period", DefaultValue = 30, MinValue = 3)]
        public int Period { get; set; }

        [Parameter("Text Color", DefaultValue = "Yellow")]
        public string TextColor { get; set; }

        [Parameter("Line Color", DefaultValue = "White")]
        public string LineColor { get; set; }


        private Colors _colorText;
        private Colors _colorLine;

        protected override void Initialize()
        {
            // Parse color from string

            if (!Enum.TryParse(TextColor, out _colorText))
            {
                ChartObjects.DrawText("errorMsg1", "Invalid Color for Text", StaticPosition.TopLeft, Colors.Red);
                _colorText = Colors.Yellow;
            }

            if (!Enum.TryParse(LineColor, out _colorLine))
            {
                ChartObjects.DrawText("errorMsg2", "\nInvalid Color for Line", StaticPosition.TopLeft, Colors.Red);
                _colorLine = Colors.White;
            }
        }

        public override void Calculate(int index)
        {
            if (!IsLastBar)
                return;

            ChartObjects.DrawVerticalLine("vLine", index - Period, _colorLine);

            int maxIndex = index;
            double max = Source[index];

            for (int i = index - Period; i <= index; i++)
            {
                if (max >= Source[i])
                    continue;

                max = Source[i];
                maxIndex = i;

            }

            var text = "max " + max.ToString("0.0000");
            var top = VerticalAlignment.Top;
            var center = HorizontalAlignment.Center;

            ChartObjects.DrawText("max", text, maxIndex, max + 0.0002, top, center, _colorText);
            ChartObjects.DrawLine("line", maxIndex, max, index, Source[index], _colorLine);

        }
    }
}
