using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AutoRescale = false, AccessRights = AccessRights.None)]
    public class _500Pips : Indicator
    {
        public int LineBold;
        public Colors LineColor;

        [Parameter("Show 500PipsLevels", DefaultValue = 1)]
        public bool Set500Levels { get; set; }

        [Parameter("MinLevel", DefaultValue = 0, MinValue = 0)]
        public int MinLevel { get; set; }

        [Parameter("MaxLevel", DefaultValue = 200, MinValue = 2)]
        public int MaxLevel { get; set; }

        // line bold
        [Parameter("Line bold", DefaultValue = 5, MinValue = 1, MaxValue = 5)]
        public int L1 { get; set; }

        // ===== linr color
        [Parameter(DefaultValue = 1)]
        public bool SetBlue { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool SetRed { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool SetWhite { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool SetBlack { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool SetGreen { get; set; }


        public override void Calculate(int index)
        {

            LineBold = L1;

            if (SetBlue)
            {
                LineColor = Colors.DodgerBlue;
            }


            if (SetRed)
            {
                LineColor = Colors.Red;
            }
            if (SetWhite)
            {
                LineColor = Colors.White;
            }
            if (SetBlack)
            {
                LineColor = Colors.Black;
            }
            if (SetGreen)
            {
                LineColor = Colors.YellowGreen;
            }


            if (Set500Levels && MinLevel < MaxLevel)
            {
                for (int i = MinLevel; i < MaxLevel; i++)
                {
                    ChartObjects.DrawHorizontalLine("Level" + i, i * 500 * Symbol.PipSize, LineColor, LineBold, LineStyle.Solid);
                }
            }


        }
    }
}
