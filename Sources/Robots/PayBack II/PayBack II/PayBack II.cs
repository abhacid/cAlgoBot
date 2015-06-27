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
//		PayBack modifie (28 juin 2014)
//		version 1.2014.7.1.19h
//		Author : https://www.facebook.com/ab.hacid
//
// -------------------------------------------------------------------------------
#endregion

#region cBot Parameters Comments
// -------------------------------------------------------------------------------
//
//	Utiliser : 
//			Volume				=	100000
//          SL					=	57 pips
//          TP					=	150 pips
//			commission			=	37.6 per Million
//			Spread fixe			=	1pip
//			Starting Capital	=	50000
//
//	Results :
//          sur GBPUSD en h1 entre le 1/1/2014 et 1/7/2014 a 19h30 gain de 9482 euros(+19%).
//			Net profit			=	9481.93
//			Ending Equity		=	10164.18 euros
//			Ratio de Sharpe		=	0.24
//			Ratio de Storino	=	0.55
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
using cAlgo;
using cAlgo.Lib;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class PayBackII : Robot
    {
        [Parameter("Initial Volume", DefaultValue = 100000, MinValue = 0)]
        public int InitialVolume { get; set; }
        [Parameter("Stop Loss", DefaultValue = 57)]
        public int StopLoss { get; set; }
        [Parameter("Take Profit", DefaultValue = 150)]
        public int TakeProfit { get; set; }

        const long microVolume = 1000;
        const string botLabel = "PB-II";


        protected override void OnStart()
        {
            Positions.Opened += OnPositionOpened;
            Positions.Closed += OnPositionClosed;

            relanceOrders();
        }

        private void relanceOrders()
        {
            manageOpen(TradeType.Buy, InitialVolume);
            manageOpen(TradeType.Sell, InitialVolume);
        }

        private void manageOpen(TradeType tradeType, long volume, string prefixLabel = botLabel)
        {
            int nVolumePartition = 10, part1 = 5, part2 = 3;
            long nVol = (long)Math.Floor((double)(volume / (microVolume * nVolumePartition)));
            long partialVolume = nVol * microVolume;

            var result1 = ExecuteMarketOrder(tradeType, Symbol, partialVolume * part1, prefixLabel + tradeType.ToString() + "-1");
            var result2 = ExecuteMarketOrder(tradeType, Symbol, partialVolume * part2, prefixLabel + tradeType.ToString() + "-2");
            var result3 = ExecuteMarketOrder(tradeType, Symbol, volume - (part1 + part2) * partialVolume, prefixLabel + tradeType.ToString() + "-3");
        }

        private void manageClose()
        {
            foreach (var position in Positions)
            {
                if (position.TakeProfit.HasValue)
                {
                    string labelType = position.Label.Substring(position.Label.Length - 1, 1);
                    double potentialGainPips = ((position.TradeType == TradeType.Buy) ? 1 : -1) * (position.TakeProfit.Value - position.EntryPrice) / Symbol.PipSize;
                    double potentialLosePips = ((position.TradeType == TradeType.Buy) ? 1 : -1) * (position.StopLoss.Value - position.EntryPrice) / Symbol.PipSize;
                    double percentGain = position.Pips / potentialGainPips;
                    double percentLose = -position.Pips / potentialLosePips;

                    if ((percentGain >= 0.43) && (labelType == "3"))
                        ClosePosition(position);

                    if ((percentGain >= 0.76) && (labelType == "2"))
                        ClosePosition(position);

                    if ((percentLose <= -0.33) && (labelType == "1"))
                        ClosePosition(position);

                    if ((percentLose <= -0.66) && (labelType == "2"))
                        ClosePosition(position);

                }
            }
        }


        protected void OnPositionOpened(PositionOpenedEventArgs args)
        {
            var position = args.Position;

            ModifyPosition(position, position.pipsToStopLoss(Symbol, StopLoss), position.pipsToTakeProfit(Symbol, TakeProfit));
        }

        protected void OnPositionClosed(PositionClosedEventArgs args)
        {
            Position position = args.Position;

            if (position.Pips < 0)
                manageOpen(position.inverseTradeType(), position.Volume, botLabel + "Mart-");

            if (Positions.Count == 0)
                relanceOrders();
        }


        protected override void OnTick()
        {
            manageClose();

        }
        protected override void OnError(Error error)
        {
            if (error.Code != ErrorCode.BadVolume)
            {
                Print("erreur : " + error.Code);
                Stop();
            }
        }



    }
}


