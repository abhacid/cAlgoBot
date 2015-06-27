#region Licence
//The MIT License (MIT)
//Copyright (c) 2014 abdallah HACID, https://www.facebook.com/ab.hacid

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//and associated documentation files (the "Software"), to deal in the Software without restriction,
//including without limitation the rights to use, copy, modify, merge, publish, distribute,
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
//is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or
//substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
//BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Project Hosting for Open Source Software on Codeplex : https://calgobots.codeplex.com/
#endregion

#region cBot Infos
// -------------------------------------------------------------------------------
//
//		PayBack
//
//	create two trades: buying and selling
//	when a trade is negative, you win on the other.
//	the martingale (non-random) reimburse the losing trades.
//	that the market goes up or down you generate profits.
//	Attention to the margin
//	adjust the stop loss and take profit depending on the size of your wallet and your leverage
//
//	good luck
//
//	tradermatrix (Marc)
//
// -------------------------------------------------------------------------------
#endregion

#region advertisement
// -------------------------------------------------------------------------------
//			Trading using leverage carries a high degree of risk to your capital, and it is possible to lose more than
//			your initial investment. Only speculate with money you can afford to lose.
// -------------------------------------------------------------------------------
#endregion


using System;
using cAlgo.API;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class PayBack : Robot
    {
        [Parameter("Initial Volume", DefaultValue = 10000, MinValue = 0)]
        public int InitialVolume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 20)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 20)]
        public int TakeProfit { get; set; }

        private Position position;


        protected override void OnStart()
        {
            ExecuteOrder(InitialVolume, TradeType.Sell);
            ExecuteOrder(InitialVolume, TradeType.Buy);
        }

        private void ExecuteOrder(int volume, TradeType tradeType)
        {
            ExecuteMarketOrder(tradeType, Symbol, volume);
        }

        protected override void OnPositionOpened(Position openedPosition)
        {
            position = openedPosition;
            ModifyPosition(openedPosition, GetAbsoluteStopLoss(openedPosition, StopLoss), GetAbsoluteTakeProfit(openedPosition, TakeProfit));
        }

        protected override void OnPositionClosed(Position closedPosition)
        {
            if (closedPosition.GrossProfit > 0)
            {
                ExecuteOrder(InitialVolume, closedPosition.TradeType);
            }
            else
            {
                ExecuteOrder((int)closedPosition.Volume * 2, closedPosition.TradeType);
            }
        }
        protected override void OnError(Error error)
        {
            if (error.Code == ErrorCode.BadVolume)
                Stop();
        }

        private double GetAbsoluteStopLoss(Position position, int stopLossInPips)
        {
            return position.TradeType == TradeType.Buy ? position.EntryPrice - Symbol.PipSize * stopLossInPips : position.EntryPrice + Symbol.PipSize * stopLossInPips;
        }

        private double GetAbsoluteTakeProfit(Position position, int takeProfitInPips)
        {
            return position.TradeType == TradeType.Buy ? position.EntryPrice + Symbol.PipSize * takeProfitInPips : position.EntryPrice - Symbol.PipSize * takeProfitInPips;
        }
    }
}


