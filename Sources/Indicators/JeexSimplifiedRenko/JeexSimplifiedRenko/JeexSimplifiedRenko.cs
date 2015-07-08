// -------------------------------------------------------------------------------
//
//      SimplifiedRenko is property of Jeex.Eu - victor@jeex.eu 
//      Current Version 1.0.0
//
//
// -------------------------------------------------------------------------------
using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class JeexSimplifiedRenko : Indicator
    {
        [Parameter("Renko Pips", DefaultValue = 10)]
        public int pips { get; set; }

        [Output("Renko UP", Color = Colors.DodgerBlue, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries rUP { get; set; }

        [Output("Renko DOWN", Color = Colors.Tomato, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries rDOWN { get; set; }

        private double vorige;
        private int vorigeBar;
        private Colors kleur;

        protected override void Initialize()
        {
            // nothing to do here
            vorigeBar = 0;
            vorige = 0;
        }

        public override void Calculate(int index)
        {
            int nuBar = MarketSeries.Close.Count - 1;
            double c = MarketSeries.Close[index] / Symbol.PipSize;
            double nu = (c - (c % pips)) * Symbol.PipSize;

            if (nu > vorige)
            {
                // stijging van minimaal pips
                rUP[index] = nu;
                rDOWN[index] = nu - (pips * Symbol.PipSize);
                vvorig(nu, nuBar);
            }
            else if (nu < vorige)
            {
                // daling van minimaal pips
                rUP[index] = nu + (pips * Symbol.PipSize);
                rDOWN[index] = nu;
                kleur = Colors.Tomato;
                vvorig(nu, nuBar);
            }
            else
            {
                // zelfde
                rUP[index] = rUP[index - 1];
                rDOWN[index] = rDOWN[index - 1];
            }
        }

        private bool vvorig(double nieuweWaarde, int bar)
        {
            if (bar > vorigeBar)
            {
                vorige = nieuweWaarde;
                vorigeBar = bar;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
