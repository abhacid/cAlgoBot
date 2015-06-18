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
//		TrailCutII-II (18 juillet 2014)
//		version 2.2014.7.13h30
//		Author : https://www.facebook.com/ab.hacid
//
//	Robot Multi indicateurs avec seuil de declenchement (levelSignal), 
//	gerant un trail stop, 
//	une cloture anticipee des pertes (Cut Loss) 
//	une martingale selective, 
//	Possibilite d'achat ou de vente seulement,
//	Signaux sur Tick ou sur nouvelle barre
// -------------------------------------------------------------------------------
#endregion


#region cBot Parameters Comments
//
//	Utiliser : (parametres a modifier avant de tester : Symbol, Timeframe, WprSource)
//			Symbol							=	GBPUSD
//			TimeFrame						=	D1
//			Volume							=	100000
//
//			OnTick							=	Non						//	Declanchement des ordres sur chaque nouveau tick ou sur chaque nouvelle barre
//          Stop Loss						=	150 pips
//          Take Profit						=	1000 pips				
//			Cut Loss						=	Non						//	Coupe les pertes a 50%, 66% du StopLoss initial
//			Buy Only						=	Non						//	Execute seulement des ordres sur signaux d'achat
//			Sell Only						=	Non						//	Execute seulement des ordres sur signaux de vente

//			Martingale						=	Oui						//	En cas de perte inverse la position avec un facteur de 1.5*Volume initial
//
//			Trail Start						=	3000					//	Debut du mouvement du stopLoss
//			Trail Step						=	3						//	Pas du Mouvement de trailling
//			Trail Stop						=	29						//	Minimum du StopLoss
//
//			WPR Source						=	Open					
//			WPR Period						=   17
//			WPR Overbuy Ceil				=	-20						//	Seuil d'oversell
//			WPR Oversell Ceil				=	-80						//	Seuil d'overbuy
//			WPR Magic Number				=	2						//	Permet d'etendre le temps de detection du tradeType et cree plus de signaux (Magic)
//			WPR Min/Max Period				=	114						//	Periode pendant laquelle on calcule le minimum et le maximum pour detecter l'etendue du range
//			WPR Exceed MinMax				=	2						//	Decalage par rapport au Minimum et au Maximum pour cloturer les positions
//
//			MBFXLen							=	13
//			MBFX Filter						=	5
//			ZzDeph							=	12
//			ZzDeviation						=	5
//			ZzBackStep						=	3
//
//			commission						=	37.6 per Million
//			Spread fixe						=	1pip
//			Starting Capital				=	50000
//
//	Results :
//          Resultats			=	entre le 01/04/2011 et 18/7/2014 a 11:30 gain de 34534 euros(+69%).
//			Net profit			=	34534.34 euros
//			Ending Equity		=	34534.34 euros
//			Ratio de Sharpe		=	1.75 sur achats 2.23 sur ventes
//			Ratio de Storino	=	-    sur achats - sur ventes
//
// -------------------------------------------------------------------------------
//			Utiliser en trading reel a vos propres risques. 
//			l'effet de levier comportant un risque de perte supérieure au capital investi.
// -------------------------------------------------------------------------------
#endregion

#region advertisement
// -------------------------------------------------------------------------------
//			Trading using leverage carries a high degree of risk to your capital, and it is possible to lose more than
//			your initial investment. Only speculate with money you can afford to lose.
// -------------------------------------------------------------------------------
#endregion


using cAlgo;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;
using cAlgo.Lib;
using cAlgo.Strategies;

using System;
using System.Text;
using System.Collections.Generic;

namespace cAlgo.Robots
{
	[Robot("TrailCutII", TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
	public class TrailCutII : Robot
	{
		#region cBot Parameters

		[Parameter("William Percent Range", DefaultValue = true)]
		public bool IsWPRActif { get; set; }

		[Parameter("Zigzag Kwan MBFX Timing", DefaultValue = false)]
		public bool IsZZKwanActif { get; set; }

		[Parameter("Zigzag", DefaultValue = false)]
		public bool IsZZActif { get; set; }

		[Parameter("Double Candle", DefaultValue = false)]
		public bool IsDCActif { get; set; }

		[Parameter("Trend Magic", DefaultValue = false)]
		public bool IsTMActif { get; set; }

		[Parameter("Multi-Strategie Signal Ceil", DefaultValue = 1, MinValue = 1)]
		public int MultiStrategieSignalCeil { get; set; }

		[Parameter("Volume", DefaultValue = 100000, MinValue = 0)]
		public int InitialVolume { get; set; }

		[Parameter("Stop Loss", DefaultValue = 150)]
		public double StopLoss { get; set; }

		[Parameter("Take Profit", DefaultValue = 1000)]
		public double TakeProfit { get; set; }

		[Parameter("Period", DefaultValue = 1000)]
		public int Period { get; set; }

		[Parameter("OnTick", DefaultValue = false)]
		public bool IsOnTick { get; set; }

		[Parameter("Cut Loss", DefaultValue = false)]
		public bool CutLoss { get; set; }

		[Parameter("Buy Only", DefaultValue = false)]
		public bool BuyOnly { get; set; }

		[Parameter("Sell Only", DefaultValue = false)]
		public bool SellOnly { get; set; }

		[Parameter("Martingale", DefaultValue = false)]
		public bool Martingale { get; set; }

		[Parameter("Trail Start", DefaultValue = 3000, MinValue = 1)]
		public int TrailStart { get; set; }

		[Parameter("Trail Step", DefaultValue = 3, MinValue = 0)]
		public int TrailStep { get; set; }

		[Parameter("Trail Stop Min", DefaultValue = 29, MinValue = 0)]
		public int TrailStopMin { get; set; }

		[Parameter("WPR Source")]		// Placer a Open
		public DataSeries WprSource { get; set; }

		[Parameter("WPR Period", DefaultValue = 17, MinValue = 1)]
		public int WprPeriod { get; set; }

		[Parameter("WPR Overbuy Ceil", DefaultValue = -20, MinValue = -100, MaxValue = 0)]
		public int WprOverbuyCeil { get; set; }

		[Parameter("WPR Oversell Ceil", DefaultValue = -80, MinValue = -100, MaxValue = 0)]
		public int WprOversellCeil { get; set; }

		[Parameter("WPR Magic Number", DefaultValue = 2, MinValue = 0)]
		public int WprMagicNumber { get; set; }

		[Parameter("WPR Min/Max Period", DefaultValue = 114)]
		public int WprMinMaxPeriod { get; set; }

		[Parameter("WPR Exceed MinMax", DefaultValue = 2)]
		public int WprExceedMinMax { get; set; }

		[Parameter("MBFX Len", DefaultValue = 4, MinValue = 0)]
		public int MbfxLen { get; set; }

		[Parameter("MBFX Filter", DefaultValue = -1.0)]
		public double MbfxFilter { get; set; }

		[Parameter("ZigZagIndicator Depth",DefaultValue = 12)]
		public int ZzDepth { get; set; }

		[Parameter("ZigZagIndicator Deviation",DefaultValue = 5)]
		public int ZzDeviation { get; set; }

		[Parameter("ZigZagIndicator BackStep",DefaultValue = 3)]
		public int ZzBackStep { get; set; }

		[Parameter("Double Candle step",DefaultValue = 7)]
		public int DoubleCandleStep { get; set; }


		[Parameter("Trend Magic CCIPeriod", DefaultValue = 50)]
		public int TMCciPeriod { get; set; }

		[Parameter("Trend Magic ATRPeriod", DefaultValue = 5)]
		public int TMAtrPeriod { get; set; }

		#endregion

		#region cBot globals

		OrderParams initialOP;
		List<Strategy> strategies;

		#endregion

		#region cBot Events

		/// <summary>
		/// 
		/// </summary>
		protected override void OnStart()
		{
			base.OnStart();

			double slippage = 2;			// maximum slippage in point, if order execution imposes a higher slippage, the order is not executed.
			string botPrefix = "TCII";		// order prefix passed by the bot
			string comment = string.Format("{0}-{1} {2}", botPrefix, Symbol.Code, TimeFrame); 		// order label passed by the bot

			initialOP = new OrderParams(null, Symbol, InitialVolume, this.botName(), StopLoss, TakeProfit, slippage, comment, null, new List<double>() { 5, 3, 2 });

			Positions.Closed += OnPositionClosed;

			strategies = new List<Strategy>();

			if (IsZZKwanActif)
				strategies.Add(new ZigZagKwanStrategy(this,MbfxLen,MbfxFilter));

			if (IsWPRActif)
				strategies.Add(new WPRSStrategy(this, WprSource, WprPeriod, WprOverbuyCeil, WprOversellCeil, WprMagicNumber, WprMinMaxPeriod, WprExceedMinMax, IsOnTick));

			if (IsDCActif)
				strategies.Add(new DoubleCandleStrategy(this,Period,DoubleCandleStep));

			if (IsZZActif)
				strategies.Add(new ZigZagStrategy(this, ZzDepth,ZzDeviation,ZzBackStep));

			if (IsTMActif)
				strategies.Add(new TrendMagicStrategy(this,TMCciPeriod,TMAtrPeriod));
		}

     	/// <summary>
     	/// Controle des positions et relance.  
     	/// </summary>
		protected override void OnTick()
		{
			if (IsOnTick)
				controlRobot();
		}

		/// <summary>
		/// A Chaque nouvelle bougie on peut evaluer la possibilite d'achat ou de vente ou de neutralite
		/// </summary>
		protected override void OnBar()
		{
			if (!IsOnTick)
				controlRobot();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		protected void OnPositionClosed(PositionClosedEventArgs args)
		{
			Position position = args.Position;

			// Manage a selective Martingale.
			if (Martingale)
			{
				OrderParams op = this.martingale(position);
				op.Slippage = initialOP.Slippage;
				this.splitAndExecuteOrder(op);
			}
			
			Print(position.log(this, false));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="error"></param>
		protected override void OnError(Error error)
		{
			base.OnError(error);

			this.notifyError(error, new System.Net.Mail.MailAddress("ab.hacid@gmail.com"));
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnStop()
		{
			base.OnStop();
			this.closeAllPositions();
		}

		#endregion

		#region cBot Action

		/// <summary>
		/// Manage taking position
		/// </summary>
		private void controlRobot()
		{
			if (CutLoss)
				this.partialClose(initialOP.Label);

			foreach (Position position in Positions)
				position.trailStop(this, TrailStart, TrailStep, TrailStopMin);

			initialOP.TradeType = this.signal(strategies, MultiStrategieSignalCeil);

			if (initialOP.TradeType.HasValue)
				this.splitAndExecuteOrder(initialOP);

		}
		#endregion

	}
}


