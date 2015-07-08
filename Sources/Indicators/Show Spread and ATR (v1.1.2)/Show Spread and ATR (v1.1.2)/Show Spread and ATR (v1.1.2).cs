using System;
using System.Text;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class atr : Indicator
    {
        private AverageTrueRange averageTrueRange;

        public StaticPosition corner_position;

        [Parameter("Show Spread", DefaultValue = 1)]
        public bool ShowSpread { get; set; }

        [Parameter("Show ATR", DefaultValue = 1)]
        public bool ShowATR { get; set; }

        [Parameter("Choose corner", DefaultValue = 1, MinValue = 1, MaxValue = 4)]
        public int corner { get; set; }

        [Parameter("ATR Periods", DefaultValue = 14)]
        public int Periods { get; set; }

        [Parameter("ATR Multiplier", DefaultValue = 1)]
        public double ATRMultiplier { get; set; }

        [Parameter("ATR MA Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MAType { get; set; }

        protected override void Initialize()
        {
            averageTrueRange = Indicators.AverageTrueRange(Periods, MAType);
        }

        public override void Calculate(int index)
        {

            switch (corner)
            {
                case 1:
                    corner_position = StaticPosition.TopLeft;
                    break;
                case 2:
                    corner_position = StaticPosition.TopRight;
                    break;
                case 3:
                    corner_position = StaticPosition.BottomLeft;
                    break;
                case 4:
                    corner_position = StaticPosition.BottomRight;
                    break;
            }

            double multiplier;
            if (MarketSeries.SymbolCode == "EURJPY" || MarketSeries.SymbolCode == "USDJPY" || MarketSeries.SymbolCode == "AUDJPY" || MarketSeries.SymbolCode == "CADJPY" || MarketSeries.SymbolCode == "CHFJPY" || MarketSeries.SymbolCode == "GBPJPY" || MarketSeries.SymbolCode == "NOKJPY" || MarketSeries.SymbolCode == "NZDJPY" || MarketSeries.SymbolCode == "SEKJPY" || MarketSeries.SymbolCode == "SGDJPY")
            {
                multiplier = 100;
            }
            else
            {
                multiplier = 10000;
            }


            StringBuilder LineBreak = new StringBuilder();
            LineBreak.AppendLine();

            StringBuilder LineBreak2 = new StringBuilder();
            LineBreak2.AppendLine();
            LineBreak2.AppendLine();


            if (ShowATR == true)
            {
                string ATR = "" + Math.Round(averageTrueRange.Result.LastValue * multiplier * ATRMultiplier, 1);
                ChartObjects.DrawText("show ATR", "ATR: " + ATR + " pips" + LineBreak2, corner_position);
            }


            if (ShowSpread == true)

                if (corner == 1 || corner == 2)
                {
                    string spread = "" + Math.Round(Symbol.Spread * multiplier, 1);
                    ChartObjects.DrawText("show Spread", LineBreak + "Spread: " + spread + " pips", corner_position);
                }

                else
                {
                    string spread = "" + Math.Round(Symbol.Spread * multiplier, 1);
                    ChartObjects.DrawText("show Spread", "Spread: " + spread + " pips", corner_position);
                }

        }

    }
}

//changelog:
//v1.1.2 (2014-12-01):
//deleted some uncalled code
//v1.1.1 (2014-12-01):
//fix choose indicator corner
//v1.1.0 (2014-11-28):
//+ Option to choose indicator corner
//v1.0.0 (2014-11-27):
//initial version
