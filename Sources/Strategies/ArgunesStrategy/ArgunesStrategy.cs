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

#region Strategy Infos
// -------------------------------------------------------------------------------
//
//		Author : https://www.facebook.com/ab.hacid
//
//	Argunes : Forex Factory jan 2009
//
//	Hi Andi,
//
//	Thanks for offering help. I'll share a part of my trading system with you and if possible i would like to have an indicator showing my buy/sell signals.
//
//	First part of my trading system includes moving averages.
//	Here are the values,
//	5 EMA (close),
//	8 SMA (open),
//	21 EMA (close),
//	55 EMA (close)
//	200 EMA (close).
//
//	My signals are these;
//		1. when 21 EMA is over 55 EMA; buy when 5 EMA crosses up 8 SMA,
//		2. when 21 EMA is under 55 EMA; sell when 5 EMA crosses down 8 SMA.
//
//	I also look at MACD and Stothastics for confirmation but moving average crossovers are enought for now.
//
//	I'd be glad if you send indicators to my email; argunes@hotmail.com .
//	If you have any questions about my strategy please don't hesitate to ask.
// -------------------------------------------------------------------------------
#endregion

using cAlgo;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Lib;

namespace cAlgo.Strategies
{
	public class ArgunesStrategy : Strategy
    {
		ExponentialMovingAverage ema5;	// Close
		SimpleMovingAverage sma8;		// Open
		ExponentialMovingAverage ema21;	// Close
		ExponentialMovingAverage ema55;	// Close

		public ArgunesStrategy(Robot robot) : base(robot)
		{
			Initialize();
		}

		protected override void Initialize()
		{
			ema5 = Robot.Indicators.ExponentialMovingAverage(Robot.MarketSeries.Close, 5);
			sma8 = Robot.Indicators.SimpleMovingAverage(Robot.MarketSeries.Open, 8);
			ema21 = Robot.Indicators.ExponentialMovingAverage(Robot.MarketSeries.Close, 21);
			ema55 = Robot.Indicators.ExponentialMovingAverage(Robot.MarketSeries.Close, 55);
		}

		/// <summary>
		/// Strategy :  when 21 EMA is over 55 EMA; buy when 5 EMA crosses up 8 SMA and
		/// when 21 EMA is under 55 EMA; sell when 5 EMA crosses down 8 SMA.
		/// </summary>
		/// <returns></returns>
		public override TradeType? signal()
		{
			if (!Robot.existBuyPositions() && (ema21.Result.LastValue > ema55.Result.LastValue) && (ema5.Result.HasCrossedAbove(sma8.Result, 0)))
			{
				Robot.closeAllSellPositions();
				return TradeType.Buy;
			}

			if (!Robot.existSellPositions() && (ema21.Result.LastValue < ema55.Result.LastValue) && (ema5.Result.HasCrossedBelow(sma8.Result, 0)))
			{
				Robot.closeAllBuyPositions();
				return TradeType.Sell;
			}

			return null;
		}
    }
}
