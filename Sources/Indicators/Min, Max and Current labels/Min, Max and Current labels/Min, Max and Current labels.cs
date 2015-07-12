// -------------------------------------------------------------------------------
//
//    This indicator can be attached to the source indicator. It shows its maximum, minimum and current values.
//    You can specify values sacle precision (digits number), text color and period to use when looking for max and min.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MaxMinCurrentLabels : Indicator
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Period", DefaultValue = 30, MinValue = 3)]
        public int Period { get; set; }

        [Parameter("Scale Precision", DefaultValue = 5)]
        public int ScalePrecision { get; set; }

        [Parameter("Text Color", DefaultValue = "Yellow")]
        public string TextColor { get; set; }

        private Colors color = Colors.Yellow;
        private string format;

        protected override void Initialize()
        {
            // Parse color from string, e.g. "Yellow", "Green", "Red". string must start with large letter, "Red" is valid, "red" - not.
            Enum.TryParse(TextColor, out color);

            // create string format based on scale precision, e.g "0.000" for scale precision = 3
            format = "0." + new string('0', ScalePrecision);

            UpdateLabels(true);
        }

        public override void Calculate(int index)
        {
            if (IsRealTime)
                UpdateLabels(false);
        }

        private void UpdateLabels(bool fastUpdate)
        {
            ChartObjects.RemoveAllObjects();

            int startIndex = fastUpdate ? Source.Count - 1000 : 0;
            int index;

            index = Source.Count - 2;
            while (index >= startIndex)
            {
                if (IsLocalExtremum(index, true))
                {
                    ChartObjects.DrawText("max_" + index, Source[index].ToString(format), index, Source[index], VerticalAlignment.Top, HorizontalAlignment.Center, color);
                    index = index - Period;
                }
                else
                    index--;
            }

            index = Source.Count - 2;
            while (index >= startIndex)
            {
                if (IsLocalExtremum(index, false))
                {
                    ChartObjects.DrawText("min_" + index, Source[index].ToString(format), index, Source[index], VerticalAlignment.Bottom, HorizontalAlignment.Center, color);
                    index = index - Period;
                }
                else
                    index--;
            }

            var lastIndex = Source.Count - 1;

            ChartObjects.DrawText("current", Source[lastIndex].ToString(format), lastIndex + 1, Source[lastIndex], VerticalAlignment.Center, HorizontalAlignment.Right, color);
        }

        private bool IsLocalExtremum(int index, bool findMax)
        {
            int end = Math.Min(index + Period, Source.Count - 1);
            int start = Math.Max(index - Period, 0);

            double value = Source[index];

            for (int i = start; i <= end; i++)
            {
                if (findMax && value < Source[i])
                    return false;

                if (!findMax && value > Source[i])
                    return false;
            }
            return true;
        }
    }
}
