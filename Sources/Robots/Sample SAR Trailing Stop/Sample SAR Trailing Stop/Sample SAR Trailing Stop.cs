// -------------------------------------------------------------------------------------------------
//
//    This code is a cAlgo API sample.
//
//    This cBot is intended to be used as a sample and does not guarantee any particular outcome or
//    profit of any kind. Use it at your own risk
//
//    The "Sample SAR Trailing Stop" will create a market Buy order if the parabolic SAR of the previous bar is 
//    below the candlestick. A Sell order will be created if the parabolic SAR of the previous bar is above the candlestick.  
//    The order's volume is specified in the "Volume" parameter. The order will have a trailing stop defined by the 
//    previous periods' Parabolic SAR levels. The user can change the Parabolic SAR settings by adjusting the "MinAF" 
//    and "MaxAF" parameters.
//
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SampleSARTrailingStop : Robot
    {
        [Parameter("Min AF", DefaultValue = 0.02, MinValue = 0)]
        public double MinAF { get; set; }

        [Parameter("Max AF", DefaultValue = 0.2, MinValue = 0)]
        public double MaxAF { get; set; }

        [Parameter("Quantity (Lots)", DefaultValue = 1, MinValue = 0.01, Step = 0.01)]
        public double Quantity { get; set; }

        private ParabolicSAR parabolicSAR;

        protected override void OnStart()
        {
            parabolicSAR = Indicators.ParabolicSAR(MinAF, MaxAF);
            var tradeType = parabolicSAR.Result.LastValue < Symbol.Bid ? TradeType.Buy : TradeType.Sell;
            Print("Trade type is {0}, Parabolic SAR is {1}, Bid is {2}", tradeType, parabolicSAR.Result.LastValue, Symbol.Bid);

            var volumeInUnits = Symbol.QuantityToVolume(Quantity);
            ExecuteMarketOrder(tradeType, Symbol, volumeInUnits, "PSAR TrailingStops");
        }

        protected override void OnTick()
        {
            var position = Positions.Find("PSAR TrailingStops", Symbol);

            if (position == null)
            {
                Stop();
            }
            else
            {
                double newStopLoss = parabolicSAR.Result.LastValue;
                bool isProtected = position.StopLoss.HasValue;

                if (position.TradeType == TradeType.Buy && isProtected)
                {
                    if (newStopLoss > Symbol.Bid)
                        return;
                    if (newStopLoss - position.StopLoss < Symbol.TickSize)
                        return;
                }

                if (position.TradeType == TradeType.Sell && isProtected)
                {
                    if (newStopLoss < Symbol.Bid)
                        return;
                    if (position.StopLoss - newStopLoss < Symbol.TickSize)
                        return;
                }

                ModifyPosition(position, newStopLoss, null);
            }
        }
    }
}
