// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//    Please use the “Feedback” tab to provide us with your suggestions about cAlgo’s API.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Requests;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Clock : Indicator
    {
        protected override void Initialize()
        {
            // Initialize and create nested indicators

        }

        public override void Calculate(int index)
        {

            ChartObjects.DrawText("Clock", "Broker Time: " + MarketSeries.OpenTime[0].ToShortTimeString() + ":" + DateTime.Now.Second + "  Local Time: " + DateTime.Now.ToLongTimeString(), StaticPosition.TopCenter, Colors.Green);

        }
    }
}
