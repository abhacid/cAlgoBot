using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class DrawMinMaxSpread : Indicator
    {
        private double maxSpread = 0;
        private double minSpread = 1000;

        public override void Calculate(int index)
        {
            if (IsLastBar)
                DisplaySpreadOnChart();
        }

        private void DisplaySpreadOnChart()
        {
            var spread = Math.Round(Symbol.Spread / Symbol.PipSize, 2);
            if (spread > maxSpread)
                maxSpread = spread;
            else if (spread < minSpread)
                minSpread = spread;
            string minText = string.Format("{0}", minSpread);
            string maxText = string.Format("{0}", maxSpread);
            ChartObjects.DrawText("minmaxSpread", "\t" + "Min/Max Spread: " + minSpread + "/" + maxSpread, StaticPosition.TopCenter, Colors.LightSalmon);
        }
    }
}
