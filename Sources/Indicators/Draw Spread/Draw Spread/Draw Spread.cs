using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class DrawSpread : Indicator
    {
        public override void Calculate(int index)
        {
            if (IsLastBar)
                DisplaySpreadOnChart();
        }

        private void DisplaySpreadOnChart()
        {
            var spread = Math.Round(Symbol.Spread / Symbol.PipSize, 2);
            string text = string.Format("{0}", spread);
            ChartObjects.DrawText("Label", "Spread:", StaticPosition.TopLeft, Colors.Yellow);
            ChartObjects.DrawText("spread", "\t" + text, StaticPosition.TopLeft, Colors.White);
        }
    }
}
