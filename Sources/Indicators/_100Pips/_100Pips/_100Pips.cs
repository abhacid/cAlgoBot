using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AutoRescale = false, AccessRights = AccessRights.None)]
    public class EnvelopeChannels : Indicator
    {
        public int LineBold;
        public Colors LineColor;

        [Parameter("Show 100PipsLevels", DefaultValue = 1)]
        public bool Set100Levels { get; set; }

        [Parameter("Show 50PipsLevels", DefaultValue = 0)]
        public bool Set50Levels { get; set; }

        [Parameter("Show 25PipsLevels", DefaultValue = 0)]
        public bool Set25Levels { get; set; }

        [Parameter("MinLevel", DefaultValue = 0, MinValue = 0)]
        public int MinLevel { get; set; }

        [Parameter("MaxLevel", DefaultValue = 200, MinValue = 2)]
        public int MaxLevel { get; set; }

        // line bold
        [Parameter("Line bold", DefaultValue = 2, MinValue = 1, MaxValue = 5)]
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


            if (Set100Levels && MinLevel < MaxLevel)
            {
                for (int i = MinLevel; i < MaxLevel; i++)
                {
                    ChartObjects.DrawHorizontalLine("Level" + i, i * 100 * Symbol.PipSize, LineColor, LineBold, LineStyle.Solid);
                }
            }

            if (Set25Levels && MinLevel < MaxLevel)
            {
                for (int i = MinLevel; i < MaxLevel; i++)
                {
                    ChartObjects.DrawHorizontalLine("Lh1" + i, i * 100 * Symbol.PipSize + 25 * Symbol.PipSize, Colors.Red, 1, LineStyle.DotsRare);
                    ChartObjects.DrawHorizontalLine("Lh3" + i, i * 100 * Symbol.PipSize + 75 * Symbol.PipSize, Colors.Red, 1, LineStyle.DotsRare);
                }
            }

            if (Set50Levels && MinLevel < MaxLevel)
            {
                for (int i = MinLevel; i < MaxLevel; i++)
                {
                    ChartObjects.DrawHorizontalLine("Lh2" + i, i * 100 * Symbol.PipSize + 50 * Symbol.PipSize, Colors.YellowGreen, 1, LineStyle.Lines);
                }
            }



        }
    }
}
