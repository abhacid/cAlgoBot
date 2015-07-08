// -------------------------------------------------------------------------------------------------
//
//    This code is a cAlgo API sample.
//
//    This cBot is intended to be used as a sample and does not guarantee any particular outcome or
//    profit of any kind. Use it at your own risk.
//
//    The "Sample Breakout cBot" will check the difference in pips between the Upper Bollinger Band and the Lower Bollinger Band 
//    and compare it against the "Band Height" parameter specified by the user.  If the height  is lower than the number of pips 
//    specified, the market is considered to be consolidating, and the first candlestick to cross the upper or lower band will 
//    generate a buy or sell signal. The user can specify the number of periods that the market should be consolidating in the 
//    "Consolidation Periods" parameter. The position is closed by a Stop Loss or Take Profit.  
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
    public class SampleBreakoutcBot : Robot
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Band Height (pips)", DefaultValue = 40.0, MinValue = 0)]
        public double BandHeightPips { get; set; }

        [Parameter("Stop Loss (pips)", DefaultValue = 20, MinValue = 1)]
        public int StopLossInPips { get; set; }

        [Parameter("Take Profit (pips)", DefaultValue = 40, MinValue = 1)]
        public int TakeProfitInPips { get; set; }

        [Parameter("Quantity (Lots)", DefaultValue = 1, MinValue = 0.01, Step = 0.01)]
        public double Quantity { get; set; }

        [Parameter("Bollinger Bands Deviations", DefaultValue = 2)]
        public double Deviations { get; set; }

        [Parameter("Bollinger Bands Periods", DefaultValue = 20)]
        public int Periods { get; set; }

        [Parameter("Bollinger Bands MA Type")]
        public MovingAverageType MAType { get; set; }

        [Parameter("Consolidation Periods", DefaultValue = 2)]
        public int ConsolidationPeriods { get; set; }

        BollingerBands bollingerBands;
        string label = "Sample Breakout cBot";
        int consolidation;

        protected override void OnStart()
        {
            bollingerBands = Indicators.BollingerBands(Source, Periods, Deviations, MAType);
        }

        protected override void OnBar()
        {

            var top = bollingerBands.Top.Last(1);
            var bottom = bollingerBands.Bottom.Last(1);

            if (top - bottom <= BandHeightPips * Symbol.PipSize)
            {
                consolidation = consolidation + 1;
            }
            else
            {
                consolidation = 0;
            }

            if (consolidation >= ConsolidationPeriods)
            {
                var volumeInUnits = Symbol.QuantityToVolume(Quantity);
                if (Symbol.Ask > top)
                {
                    ExecuteMarketOrder(TradeType.Buy, Symbol, volumeInUnits, label, StopLossInPips, TakeProfitInPips);

                    consolidation = 0;
                }
                else if (Symbol.Bid < bottom)
                {
                    ExecuteMarketOrder(TradeType.Sell, Symbol, volumeInUnits, label, StopLossInPips, TakeProfitInPips);

                    consolidation = 0;
                }
            }
        }
    }
}
