// -------------------------------------------------------------------------------
//   
//    This cBot is intended to be used a safety Net based on Equity Protection 
//    and does not guarantee any particular outcome or profit of any kind. 
//    Use it at your own risk. This cBot does not Open Positions. 
//
//    FOREX EQUITY PROTECTION http://redrhinofx.com/forex-equity-protection/
//    Protection your risk capital and start trading more successfully
//
//    This cBot closes all open positions once your are losing money.
//
//    To prevent losing money while trading forex, 
//    -- you should reduce your exposure by trading with appropiate lotsize
//    -- you should be patient and only enter when your system is valid
//    -- you should only add to losing positions when you have positive equity
//    -- you should limit your losses by setting a MaxDrawdown Percentage
// -------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot("Close Losing Positions", TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ForexEquityProtection : Robot
    {

        [Parameter("Equity Protection (percentage)", DefaultValue = 1.0, MinValue = 0.1)]
        public double MaxDrawdown { get; set; }

        [Parameter("Show Text", DefaultValue = true)]
        public bool showText { get; set; }

        [Parameter("Choose Text Corner", DefaultValue = 1, MinValue = 1, MaxValue = 4)]
        public int corner { get; set; }

        [Parameter("Maintain Equity(reduce Exposure)", DefaultValue = false)]
        public bool ReduceDrawdown { get; set; }

        public StaticPosition corner_position;

        protected override void OnStart()
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

        }
        protected override void OnTick()
        {
            foreach (var position in Positions)
            {
                // 0.01 = old version
                //1.0 / 100 = new version
                double dd = Account.Balance - Account.Equity;
                double max = Account.Balance * (MaxDrawdown / 100);
                if (showText)
                {
                    GetMaxDrawDown();
                }
                // only close orders to maintain an equity above the Max DD level
                if (dd > max && ReduceDrawdown == true)
                {
                    // close position One at a time 
                    ClosePosition(position);
                }
                // Close all orders immediately
                if (dd > max && ReduceDrawdown == false)
                {
                    foreach (var openedPosition in Positions)
                    {
                        ClosePositionAsync(openedPosition);

                    }

                }


            }
        }
        private double peak;
        private double mDrawdown;
        private List<double> drawdown = new List<double>();
        private void GetMaxDrawDown()
        {
            peak = Math.Max(peak, Account.Balance);
            drawdown.Add((peak - Account.Balance) / peak * 100);
            drawdown.Sort();
            mDrawdown = drawdown[drawdown.Count - 1];
            ChartObjects.DrawText("show DD", "MaxDrawdown: " + Math.Round(mDrawdown, 2) + " Percent", corner_position);

        }
    }
}
