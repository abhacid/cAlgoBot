// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//    Please use the “Feedback” tab to provide us with your suggestions about cAlgo’s API.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class TrailingStop : Robot
    {

        [Parameter("Tralling Stop (pips)", DefaultValue = 20.0)]
        public int StopLoss { get; set; }

        [Parameter("Inital Stop Loss (pips)", DefaultValue = 10.0)]
        public int init_StopLoss { get; set; }

        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;

            foreach (var position in Account.Positions)
            {
                if (Symbol.Code != position.SymbolCode) continue;
                //set initial sotploss
                if (position.TradeType == TradeType.Sell && position.StopLoss == null)
                    Trade.ModifyPosition(position, position.EntryPrice + init_StopLoss * Symbol.PipSize, null);
                if (position.TradeType == TradeType.Buy && position.StopLoss == null)
                    Trade.ModifyPosition(position, position.EntryPrice - init_StopLoss * Symbol.PipSize, null);


                //tralling stop
                if (position.GrossProfit <= 0) continue;

                if (position.TradeType == TradeType.Sell)
                {
                    double profit1 = position.GrossProfit + position.Commissions -
                                     StopLoss * 10 * position.Volume / 100000.0;

                    if (profit1 > 0.0 && position.TradeType == TradeType.Sell)
                    {
                        double? stopLossPrice = Symbol.Bid + StopLoss * Symbol.PipSize;
                        if (StopLoss != 0 && stopLossPrice < position.StopLoss &&
                            stopLossPrice < position.EntryPrice)
                        {
                            if (stopLossPrice - Symbol.Bid > 0)
                            {
                                Trade.ModifyPosition(position, stopLossPrice, position.TakeProfit);
                            }
                        }
                    }
                }
                else
                {
                    double profit2 = position.GrossProfit + position.Commissions -
                                     StopLoss * 10 * position.Volume / 100000.0;

                    if (profit2 > 0.0 && position.TradeType == TradeType.Buy)
                    {
                        double? stopLossPrice = Symbol.Ask - StopLoss * Symbol.PipSize;

                        if (StopLoss != 0 && stopLossPrice > position.StopLoss &&
                            stopLossPrice > position.EntryPrice)
                            if (stopLossPrice - Symbol.Ask < 0)
                                Trade.ModifyPosition(position, stopLossPrice, position.TakeProfit);
                    }
                }
            }
        }
    }
}