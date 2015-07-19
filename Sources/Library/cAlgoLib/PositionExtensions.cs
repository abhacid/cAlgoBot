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

using System;
using System.Text;
using System.Linq;

using cAlgo;
using cAlgo.API;
using cAlgo.API.Internals;


namespace cAlgo.Lib
{
	/// <summary>
	/// Méthodes d'extensions du type cAlgo.API.Position
	/// </summary>
	public static class PositionExtensions
	{
		/// <summary>
		/// Vérifie si une variable de type Position est en cours ou à déjà été clôturée
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>true si la position est active, false sinon</returns>
		public static bool isAlive(this Position position, Positions positions)
		{
			var request = from p in positions select p.Id==position.Id ;

			return request!=null;

		}
		/// <summary>
		/// Est-ce une position d'achat
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>true si achat, false sinon</returns>
		public static bool isBuy(this Position position)
		{

			return position !=null && TradeType.Buy == position.TradeType;
		}

		/// <summary>
		/// Est-ce une position de vente
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>true si vente, false sinon</returns>
		public static bool isSell(this Position position)
		{
			return position !=null && TradeType.Sell == position.TradeType;
		}

		/// <summary>
		/// Détermine si un stop loss à été placé.
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>renvoie true si un stop loss à été plcé, false sinon</returns>
		public static bool hasStop(this Position position)
		{
			return position.StopLoss.HasValue;
		}

		/// <summary>
		/// Détermine si un take profit à été placé.
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>renvoie true si un take profit à été plcé, false sinon</returns>
		public static bool hasTakeProfit(this Position position)
		{
			return position.TakeProfit.HasValue;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public static int factor(this Position position)
		{
			TradeType tradeType = position.TradeType;

			return tradeType.factor();
		}

		/// <summary>
		/// Calcule le stop loss en valeur en fonction du stop loss en pips
		/// </summary>
		/// <param name="position">Used position</param>
		/// <param name="SLPips">Stop loss en pips</param>
		/// <returns>le stop loss en valeur</returns>
		public static double? pipsToStopLoss(this Position position, Symbol symbol, int SLPips)
		{
			return position.EntryPrice - position.factor() * symbol.PipSize * SLPips;
		}

		/// <summary>
		///	Calcule le take profit en valeur en fonction du take profit en pips
		/// </summary>
		/// <param name="position">Used position</param>
		/// <param name="TPPips">le take profit en pips</param>
		/// <returns>le take profit en valeur</returns>
		public static double? pipsToTakeProfit(this Position position, Symbol symbol, int TPPips)
		{

			return position.EntryPrice + position.factor() * symbol.PipSize * TPPips;

		}

		/// <summary>
		/// Transforme une valeur en terme de prix en pips
		/// </summary>
		/// <param name="position">Used position</param>
		/// <param name="number">valeur à transformer</param>
		/// <returns>la valeur en pips de number pour position</returns>
		public static double? valueToPips(this Position position, Symbol symbol, double? number)
		{
			if (number.HasValue)
				return (number - position.EntryPrice) / (position.factor() * symbol.PipSize);
			else
				return null;

		}

		/// <summary>
		/// Transforme le stop loss en pips
		/// </summary>
		/// <param name="position">Used position</param>
		/// <param name="value"></param>
		/// <returns>le stop loss en pips</returns>
		public static double? stopLossToPips(this Position position, Symbol symbol)
		{
			return valueToPips(position, symbol, position.StopLoss);
		}

		/// <summary>
		/// Transforme le take profit en pips
		/// </summary>
		/// <param name="position">Used position</param>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="value"></param>
		/// <returns>le take profit en pips</returns>
		public static double? takeProfitToPips(this Position position, Symbol symbol)
		{
			return valueToPips(position, symbol, position.TakeProfit);
		}

		/// <summary>
		/// retourne l'inverse du type de trade
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>cAlgo.API.TradeType</returns>
		public static TradeType inverseTradeType(this Position position)
		{
			return (position.isBuy() ? TradeType.Sell : TradeType.Buy);
		}

		/// <summary>
		/// Calcule le gain potentiel maximum de la position en fonction du take profit
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>le gain potentiel</returns>
		public static double? potentialProfit(this Position position)
		{
			if (position.TakeProfit.HasValue)
				return (position.isBuy() ? position.TakeProfit.Value - position.EntryPrice : position.EntryPrice - position.TakeProfit.Value) * position.Volume;
			else
				return null;

		}

		/// <summary>
		/// Calcule la perte maximale possible en fonction du stop loss
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>la perte maximum</returns>
		public static double? potentialLoss(this Position position)
		{
			if (position.StopLoss.HasValue)
				return (position.isBuy() ? position.EntryPrice - position.StopLoss.Value : position.StopLoss.Value - position.EntryPrice) * position.Volume;
			else
				return null;

		}

		/// <summary>
		/// Calcule en % le profit relativement au potentiel de gain maximum
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>% de profit</returns>
		public static double? percentProfit(this Position position)
		{
			double? potentialProfit = position.potentialProfit();

			if (potentialProfit.HasValue && potentialProfit.Value !=0)
				return position.GrossProfit / position.potentialProfit();
			else
				return null;
		}

		/// <summary>
		/// Calcule en % la perte relativement au potentiel de perte maximum
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>% de perte</returns>
		public static double? percentLoss(this Position position)
		{
			double? potentialLoss = position.potentialLoss();

			if (potentialLoss.HasValue && potentialLoss.Value != 0)
				return position.GrossProfit / position.potentialLoss();
			else
				return null;
		}

		/// <summary>
		/// Infos sur la position
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>Infos sur la position</returns>
		public static string infos(this Position position, Symbol symbol)
		{
			StringBuilder logMessage = new StringBuilder();

			logMessage.AppendFormat("Symbol: {0}, {1}, Gain: {0} Pips", symbol.Code, position.TradeType, position.valueToPips(symbol,position.GrossProfit));

			return logMessage.ToString();
		}

		/// <summary>
		/// Complete information on an open or closed position
		/// </summary>
		/// <param name="position">Used position</param>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="withLabels">Ajoute des labels si égal à true</param>
		/// <returns>Complete infos on the Used position</returns>
		public static string log(this Position position, Robot robot, bool withLabels=false)
		{
			StringBuilder logMessage = new StringBuilder();
			string logFormat;

			if (withLabels)
				logFormat = "Symbol: {0}, Id: {1}, Year: {2}, Month: {3}, Day: {4}, DayOfWeek: {5}, EntryTime: {6}, Volume: {7}, TradeType: {8}, EntryPrice: {9}, StopLoss: {10}, TakeProfit: {11}, GrossProfit: {12}, NetProfit: {13}, Equity: {14}, Balance: {15}";
			else
				logFormat = "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}";



			logMessage.AppendFormat(logFormat,	robot.Symbol.Code,
												position.Id,
												robot.Server.Time.Year,
												robot.Server.Time.Month,
												robot.Server.Time.Day,
												robot.Server.Time.DayOfWeek,
												position.EntryTime,
												position.Volume,
												position.TradeType,
												position.EntryPrice,
												position.StopLoss,
												position.TakeProfit,
												position.GrossProfit,
												position.NetProfit,
												robot.Account.Equity,
												robot.Account.Balance);
			if (position.isAlive(robot.Positions))
			{
				string format;
				if (withLabels)
					format = ",ExitTime: {0}, ClosePrice {1}";
				else
					format = ",{0}, {1}";

					logMessage.AppendFormat(format,robot.Server.Time,robot.Symbol.Ask);
			}

			return logMessage.ToString();
		}


		/// <summary>
		/// Manage Trail Stop
		/// </summary>
		/// <param name="position">Used position</param>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="trailStart">start of the stoploss movement</param>
		/// <param name="trailStop">steps stoploss</param>
		/// <param name="trailStopMin">Minimal StopLoss</param>
		/// <param name="isModifyPosition">change the stoploss position or not ?</param>
		/// <returns>The new stoploss</returns>
		public static double? trailStop(this  Position position, Robot robot, double trailStart, double trailStop, bool isModifyPosition=true)
		{
			double? newStopLoss = position.StopLoss;	
			
			if (position.Pips > trailStart)
			{
				double actualPrice = position.isBuy() ? robot.Symbol.Bid : robot.Symbol.Ask;
				int factor = position.factor();

				if((actualPrice - newStopLoss) * factor > trailStop * robot.Symbol.PipSize)
				{
					newStopLoss += factor * trailStop * robot.Symbol.PipSize;

					if (isModifyPosition && newStopLoss != position.StopLoss)
						robot.ModifyPosition(position, newStopLoss, position.TakeProfit.Value);
				}
			}

			return newStopLoss;
		}
	}
}
