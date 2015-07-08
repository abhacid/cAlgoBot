// -------------------------------------------------------------------------------------------------
//
//    This code is a cAlgo API sample.
//
//    This robot is intended to be used as a sample and does not guarantee any particular outcome or
//    profit of any kind. Use it at your own risk.
//
//    All changes to this file will be lost on next application start.
//    If you are going to modify this file please make a copy using the "Duplicate" command.
//
//    The "Sample RSI Range Robot" will create a buy order when the Relative Strength Index indicator crosses the  level 30, 
//    and a Sell order when the RSI indicator crosses the level 70. The order is closed be either a Stop Loss, defined in 
//    the "Stop Loss" parameter, or by the opposite RSI crossing signal (buy orders close when RSI crosses the 70 level 
//    and sell orders are closed when RSI crosses the 30 level). 
//
//    The robot can generate only one Buy or Sell order at any given time.
//
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class RSIRangeRobot : Robot
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Periods", DefaultValue = 14)]
        public int Periods { get; set; }

        [Parameter("Stop Loss (pips)", DefaultValue = 10, MinValue = 1)]
        public int StopLoss { get; set; }

        [Parameter("Volume", DefaultValue = 10000, MinValue = 1000)]
        public int Volume { get; set; }

        private RelativeStrengthIndex rsi;

        protected override void OnStart()
        {
            rsi = Indicators.RelativeStrengthIndex(Source, Periods);
        }

        protected override void OnTick()
        {
            if (rsi.Result.LastValue < 30)
            {
                Close(TradeType.Sell);
                Open(TradeType.Buy);
            }
            else if (rsi.Result.LastValue > 70)
            {
                Close(TradeType.Buy);
                Open(TradeType.Sell);
            }
        }

        private void Close(TradeType tradeType)
        {
            foreach (var position in Positions.FindAll("SampleRSI", Symbol, tradeType))
                ClosePosition(position);
        }

        private void Open(TradeType tradeType)
        {
            var position = Positions.Find("SampleRSI", Symbol, tradeType);

            if (position == null)
                ExecuteMarketOrder(tradeType, Symbol, Volume, "SampleRSI");
        }
    }
}
