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
//		BreakoutII II
//		version 2.2014.7.23.1.
//		Author : https://www.facebook.com/ab.hacid
//
// -------------------------------------------------------------------------------
#endregion

#region cBot Parameters Comments
// GBPUSD, h1, Open, SL=57, TP=115, Volume=100k,, Periode=16, MAType=Wilder Smoothing, Deviation=2 donne entre 1/1/2014 et 23/7/2014 3562 euros.
#endregion

#region advertisement
// -------------------------------------------------------------------------------
//			Trading using leverage carries a high degree of risk to your capital, and it is possible to lose more than
//			your initial investment. Only speculate with money you can afford to lose.
// -------------------------------------------------------------------------------
#endregion


using System;
using System.Diagnostics;
using System.Reflection;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Lib;

namespace cAlgo.Robots
{
    /// <summary>
	/// https://github.com/abhacid/cAlgoBot/wiki/Breakout-avec-les-bandes-de-Bollinger
    /// </summary>
    [Robot("BreakOutII", TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public partial class BreakoutII : Robot
    {
        #region cBot Parameters
        [Parameter("Source")]
        // positionner a Open
        public DataSeries Source { get; set; }

        [Parameter("Stop Loss", DefaultValue = 57)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 115)]
        public int TakeProfit { get; set; }

        [Parameter("Volume", DefaultValue = 100000, MinValue = 0)]
        public int Volume { get; set; }

        [Parameter("Periods", DefaultValue = 16)]
        public int Periods { get; set; }

        [Parameter("MA Type", DefaultValue = 6)]
        //Wilder Smoothing
        public MovingAverageType MAType { get; set; }

        [Parameter("Deviation", DefaultValue = 2)]
        public int Deviation { get; set; }

        #endregion

		#region cBot variables
		private string _botName;
		private string _botVersion = Assembly.GetExecutingAssembly().FullName.Split(',')[1].Replace("Version=", "").Trim();

		// le label permet de s'y retrouver parmis toutes les instances possibles.
		private string _instanceLabel;

        private BollingerBands bb;
		#endregion

		protected override void OnStart()
        {
			_botName = ToString();
			_instanceLabel = string.Format("{0}-{1}-{2}-{3}", _botName, _botVersion, Symbol.Code, TimeFrame.ToString());

            Positions.Opened += OnPositionOpened;

            bb = Indicators.BollingerBands(Source, Periods, Deviation, MAType);
        }

        protected override void OnBar()
        {
			double middle = (bb.Top.LastValue + bb.Bottom.LastValue) / 2;

            // Cloture si les prix repassent le milieu de la bande de bollinger
			if(MarketSeries.isCandleOver(1, middle))
				this.closeAllPositions(_instanceLabel);

            TradeType? tradeType = signal();
				
			if(tradeType.HasValue)
			{
				if(tradeType.isBuy())
				{
					this.closeAllSellPositions(_instanceLabel);

					if(this.existBuyPositions(_instanceLabel))
						return;
				}
				else
				{
					this.closeAllBuyPositions(_instanceLabel);

					if(this.existSellPositions(_instanceLabel))
						return;

				}

				ExecuteMarketOrder(tradeType.Value, Symbol, Volume, _instanceLabel);
			}
        }

		private TradeType? signal()
		{
			TradeType? tradeType=null;
            double bbTop = bb.Top.LastValue;
            double bbBottom = bb.Bottom.LastValue;

            if (Symbol.Ask < bbTop && Symbol.Bid > bbBottom) // Nous sommes entre les bandes de Bollinger.
				return null;

			//achat en haut des BB, vente en bas
			if (MarketSeries.isCandleAbove(1, bbTop))
				tradeType = TradeType.Buy;
			else if (MarketSeries.iscandleBelow(1, bbBottom))
				tradeType = TradeType.Sell;

			return tradeType;
		}

        protected void OnPositionOpened(PositionOpenedEventArgs args)
        {
            Position position = args.Position;

            ModifyPosition(position, position.pipsToStopLoss(Symbol, StopLoss), position.pipsToTakeProfit(Symbol, TakeProfit));
        }

        protected override void OnStop()
        {
            base.OnStop();
            this.closeAllPositions(_instanceLabel);
        }


    }
}

