// -------------------------------------------------------------------------------
//
//    This code is a cAlgo API sample.
//
//    This cBot is intended to be used as a sample and does not guarantee any particular outcome or
//    profit of any kind. Use it at your own risk.
//
//    This cBot closes all profitable positions of the current account.
//
// -------------------------------------------------------------------------------

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot("Sample close profitable positions", TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SampleCloseProfitablePositionscBot : Robot
    {
        protected override void OnStart()
        {
            foreach (var position in Positions)
            {
                if (position.GrossProfit > 0)
                {
                    ClosePosition(position);
                }
            }
        }
    }
}
