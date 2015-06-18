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
//		Argunes (5 Aout 2014)
//		version 1.2014.8.5.20h00
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

#region cBot Parameters Comments
// -------------------------------------------------------------------------------
//	
//			Symbol				=	EURUSD
//			TimeFrame			=	4h	
//			Stop Loss			=	53
//			Take Profit			=	151
//
//	Results :
//          Results				=	entre le 01/01/2014 et 7/8/2014 a 15h00 gain de 1265 euros(+%).
//			Net profit			=	1264.90 euros
//			Ending Equity		=	1264.90 euros
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
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Lib;
using cAlgo.Strategies;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Argunes : Robot
    {
		#region cBot Parameters
		[Parameter("Volume", DefaultValue = 100000, MinValue = 0)]
		public int InitialVolume { get; set; }

		[Parameter("Stop Loss", DefaultValue = 150)]
		public int StopLoss { get; set; }

		[Parameter("Take Profit", DefaultValue = 1000)]
		public int TakeProfit { get; set; }

		#endregion

		#region cBot variables
		OrderParams initialOP;
		List<Strategy> strategies;
		#endregion

		/// <summary>
		/// 
		/// </summary>
		protected override void OnStart()
        {
			base.OnStart();

			double slippage = 2;			// maximum slippage in point, if order execution imposes a higher slippage, the order is not executed.
			string botPrefix = "Argunes";	// order prefix passed by the bot
			string positionComment = string.Format("{0}-{1} {2}", botPrefix, Symbol.Code, TimeFrame); ;				// order label passed by the bot
			initialOP = new OrderParams(null, Symbol, InitialVolume, this.botName(), StopLoss, TakeProfit, slippage, positionComment, null, new List<double>() { 5, 3, 2 });
			
			strategies = new List<Strategy>();
			strategies.Add(new ArgunesStrategy(this));
        }

		/// <summary>
		/// 
		/// </summary>
        protected override void OnTick()
        {

        }

		protected override void OnBar()
		{
			base.OnBar();

			controlRobot();

		}

		/// <summary>
		/// 
		/// </summary>
        protected override void OnStop()
        {
			base.OnStop();
			this.closeAllPositions();
        }

		/// <summary>
		/// Manage taking position.
		/// </summary>
		private void controlRobot()
		{
			TradeType? tradeType = this.signal(strategies);

			if (tradeType.HasValue)
			{
				initialOP.TradeType = tradeType.Value;
				initialOP.Volume = InitialVolume;

				this.splitAndExecuteOrder(initialOP);
			}
				
		}
    }
}
