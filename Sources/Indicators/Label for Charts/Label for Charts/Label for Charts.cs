using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class LabelforCharts : Indicator
    {
//-----------------------------------------------------------------            
        private Position position;
//-----------------------------------------------------------------     
        public override void Calculate(int index)
        {

            string Code = "\t" + Convert.ToString(Symbol.Code);
            string Ask = "\n\t" + Convert.ToString(Symbol.Ask);
            string Bid = "\n\n\t" + Convert.ToString(Symbol.Bid);
            string Spread = "\n\n\n\t" + Convert.ToString(Convert.ToSingle(Symbol.Spread) * 10000);
            string Digits = "\n\n\n\n\t" + Convert.ToString(Symbol.Digits);
            string PipSize = "\n\n\n\n\n\t" + Convert.ToString(Symbol.PipSize);

            ChartObjects.DrawText("Labels", "Code:" + "\n" + "Ask:" + "\n" + "Bid:" + "\n" + "Spread:" + "\n" + "Digits:" + "\n" + "PipSize:" + "\n", StaticPosition.TopLeft, Colors.Yellow);

            ChartObjects.DrawText("Code", Code, StaticPosition.TopLeft, Colors.LightBlue);
            ChartObjects.DrawText("Ask", Ask, StaticPosition.TopLeft, Colors.LightBlue);
            ChartObjects.DrawText("Bid", Bid, StaticPosition.TopLeft, Colors.LightBlue);
            ChartObjects.DrawText("Spread", Spread, StaticPosition.TopLeft, Colors.LightBlue);
            ChartObjects.DrawText("Digits", Digits, StaticPosition.TopLeft, Colors.LightBlue);
            ChartObjects.DrawText("PipSize", PipSize, StaticPosition.TopLeft, Colors.LightBlue);

        }
    }
}
