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

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/cAlgoBot
#endregion

#region cBot Infos
// -------------------------------------------------------------------------------
//
//		PayBack I 
//		version 1.0.0.0.1.
//		Author : https://www.facebook.com/ab.hacid
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
using cAlgo.Lib;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class PayBackI : Robot
    {
        [Parameter("Volume", DefaultValue = 10000, MinValue = 0)]
        public int Volume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 20)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 20)]
        public int TakeProfit { get; set; }

        const string partialLabel = "PB-I";


        protected override void OnStart()
        {
            Positions.Opened += OnPositionOpened;

            ExecuteOrder(TradeType.Buy, Volume);
            ExecuteOrder(TradeType.Sell, Volume);

        }

        private void ExecuteOrder(TradeType tradeType, long volume, string prefixLabel = partialLabel)
        {
            int parties = 6, part1 = 2, part2 = 2;
			long partialVolume = Symbol.NormalizeVolume(volume/parties,RoundingMode.ToNearest);

            var result1 = ExecuteMarketOrder(tradeType, Symbol, partialVolume * part1, prefixLabel + "1");
            var result2 = ExecuteMarketOrder(tradeType, Symbol, partialVolume * part2, prefixLabel + "2");
            var result3 = ExecuteMarketOrder(tradeType, Symbol, volume - (part1 + part2) * partialVolume, prefixLabel + "3");
        }

        protected void OnPositionOpened(PositionOpenedEventArgs args)
        {
            var position = args.Position;

            double stopLoss = position.TradeType == TradeType.Buy ? position.EntryPrice - Symbol.PipSize * StopLoss : position.EntryPrice + Symbol.PipSize * StopLoss;
            double takeProfit = position.TradeType == TradeType.Buy ? position.EntryPrice + Symbol.PipSize * TakeProfit : position.EntryPrice - Symbol.PipSize * TakeProfit;

            ModifyPosition(position, stopLoss, takeProfit);
        }


        protected override void OnTick()
        {
            foreach (var position in Positions)
            {
                if (position.TakeProfit.HasValue)
                {
					int factor = (position.TradeType == TradeType.Buy).factor();
                    string labelType = position.Label.Substring(position.Label.Length - 1, 1);
                    double potentialGainPips = factor * (position.TakeProfit.Value - position.EntryPrice) / Symbol.PipSize;
                    double potentialLosePips = factor * (position.StopLoss.Value - position.EntryPrice) / Symbol.PipSize;
                    double percentGain = position.Pips / potentialGainPips;
                    double percentLose = position.Pips / potentialLosePips;



                    if ((percentGain >= 0.33) && (labelType == "1"))
                        ClosePosition(position);

                    if ((percentLose <= -0.33) && (labelType == "2"))
                        ClosePosition(position);

                    if ((percentGain >= 0.66) && (labelType == "2"))
                        ClosePosition(position);

                    if ((percentLose <= -0.66) && (labelType == "3"))
                        ClosePosition(position);
                }


            }

        }
        protected override void OnError(Error error)
        {
            if (error.Code != ErrorCode.BadVolume)
                Stop();
        }

        protected TradeType inverseTradeType(Position position)
        {

            return ((position.TradeType == TradeType.Sell) ? TradeType.Buy : TradeType.Sell);
        }

    }
}


