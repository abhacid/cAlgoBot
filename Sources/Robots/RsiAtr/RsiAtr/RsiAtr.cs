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
//		RsiAtr (4 Aout 2014)
//		version 1.2014.8.4.21h00
//		Author : https://www.facebook.com/ab.hacid
//
// -------------------------------------------------------------------------------
#endregion

#region cBot Parameters Comments
//
// Robot using the indicators RSI and ATR
//
//	
//			Symbol							=	GBPUSD
//			TimeFrame						=	H4
//			Volume							=	100000
//          Stop Loss						=	51 pips
//          Take Profit						=	153 pips				
//
//			RSI Source						=	Close					
//			RSI Period						=   17
//			RSI Overbuy Ceil				=	70						//	Seuil d'oversell
//			RSI Oversell Ceil				=	30						//	Seuil d'overbuy
//			RSI Min/Max Period				=	70						//	Period during which calculates the minimum and maximum to detect the extent of the range
//			RSI Exceed MinMax				=	3						//	Lag with the Minimum and Maximum to clore positions
//
//			ATR Period						=	20
//			ATR MAType						=	Wilder Smoothing
//
//	Results :
//          Results				=	entre le 01/01/2014 et 4/8/2014 a 21h00 gain de 6961 euros(+14%).
//			Net profit			=	6960.79 euros
//			Ending Equity		=	6993.51 euros
//
// -------------------------------------------------------------------------------
//			Use in real trading at your own risk. leverage with a risk of loss greater than the capital invested.
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
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Lib;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC)]
    public class RsiAtr : Robot
    {
        #region cBot parameters
        [Parameter("Volume", DefaultValue = 100000, MinValue = 0)]
        public int Volume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 51)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 153)]
        public int TakeProfit { get; set; }

        [Parameter("RSI Source")]
        public DataSeries RsiSource { get; set; }
        // Close
        [Parameter("RSI Period", DefaultValue = 17, MinValue = 1)]
        public int RsiPeriod { get; set; }

        [Parameter("RSI Overbuy Ceil", DefaultValue = 65, MinValue = 0, MaxValue = 100)]
        public int rsiOverbuyCeil { get; set; }

        [Parameter("RSI Oversell Ceil", DefaultValue = 30, MinValue = 0, MaxValue = 100)]
        public int rsiOversellCeil { get; set; }

        [Parameter("RSI Exceed Ceil", DefaultValue = 3, MinValue = 0, MaxValue = 50)]
        public int rsiExceedCeil { get; set; }

        [Parameter("RSI MimMax Period", DefaultValue = 70, MinValue = 1)]
        public int rsiMinMaxPeriod { get; set; }

        [Parameter("ATR Period", DefaultValue = 20, MinValue = 1)]
        public int AtrPeriod { get; set; }

        [Parameter("ATR MAType", DefaultValue = 6)]
        public MovingAverageType AtrMaType { get; set; }
        // Wilder Smoothing
        #endregion

        private RelativeStrengthIndex rsi;
        private PipsATRIndicator pipsATR;

        // Prefix commands the robot passes
        private const string botPrefix = "RSI-ATR";
        // Label orders the robot passes
        private string botLabel;
        double minPipsATR;
        double maxPipsATR;
        double ceilSignalPipsATR;
        double minRSI;
        double maxRSI;

        protected override void OnStart()
        {
            botLabel = string.Format("{0}-{1} {2}", botPrefix, Symbol.Code, TimeFrame);
            rsi = Indicators.RelativeStrengthIndex(RsiSource, RsiPeriod);
            pipsATR = Indicators.GetIndicator<PipsATRIndicator>(TimeFrame, AtrPeriod, AtrMaType);

            minPipsATR = pipsATR.Result.Minimum(pipsATR.Result.Count);
            maxPipsATR = pipsATR.Result.Maximum(pipsATR.Result.Count);

        }

        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;

            minPipsATR = Math.Min(minPipsATR, pipsATR.Result.LastValue);
            maxPipsATR = Math.Max(maxPipsATR, pipsATR.Result.LastValue);
            minRSI = rsi.Result.Minimum(rsiMinMaxPeriod);
            maxRSI = rsi.Result.Maximum(rsiMinMaxPeriod);
            ceilSignalPipsATR = minPipsATR + (maxPipsATR - minPipsATR) / 3;

            if (rsi.Result.LastValue < minRSI + rsiExceedCeil)
                this.closeAllSellPositions();
            else if (rsi.Result.LastValue > maxRSI - rsiExceedCeil)
                this.closeAllBuyPositions();

            // Do nothing if daily ATR > Max allowed
            if (pipsATR.Result.LastValue <= ceilSignalPipsATR)
            {
                if ((!(this.existBuyPositions())) && rsi.Result.HasCrossedAbove(rsiOversellCeil, 0))
                {
                    this.closeAllSellPositions();
                    ExecuteMarketOrder(TradeType.Buy, Symbol, Volume, this.botName(), StopLoss, TakeProfit);
                }
                else if (!(this.existSellPositions()) && rsi.Result.HasCrossedBelow(rsiOverbuyCeil, 0))
                {
                    this.closeAllBuyPositions();
                    ExecuteMarketOrder(TradeType.Sell, Symbol, Volume, this.botName(), StopLoss, TakeProfit);
                }
            }
        }

    }
}
