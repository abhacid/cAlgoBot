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

using cAlgo.API;
using cAlgo.API.Internals;
using System;
using System.Text;
using System.Net.Mail;
using System.Linq;
using System.Collections.Generic;
using cAlgo.Strategies;
using System.Threading;

namespace cAlgo.Lib
{
	/// <summary>
	/// Méthodes d'extensions du type cAlgo.API.Robot
	/// </summary>
	public static class RobotExtensions
	{
		/// <summary>
		/// Obtient le nom du robot
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <returns>Le nom du type dérivé de Robot et définissant une nouvelle instance de Robot</returns>
		public static string botName(this Robot robot)
		{
			return robot.ToString();
		}

		/// <summary>
		/// Calcule le volume maximum a engager en nombre de Lot en fonction du risque maximum
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="risk">le pourcentage de risque (perte) maximun accepté</param>
		/// <param name="stopLossPips">Le stop Loss en PIPS nécessaire à la position à prendre</param>
		/// <returns></returns>
		public static double moneyManagement(this Robot robot, double risk, double stopLossPips)
		{

			if (stopLossPips <=0)
				throw new System.ArgumentException(String.Format("the 'stopLossPips' : {0} parameter must be positive and not null", stopLossPips));
			else
			{
				double moneyToInvestInDepositCurrency = robot.Account.Balance * risk/ (double)100;
				double moneyToInvestInQuoteCurrency = moneyToInvestInDepositCurrency * robot.Symbol.Mid();
				double volumeToInvestInQuoteCurrency = moneyToInvestInQuoteCurrency / (stopLossPips * robot.Symbol.PipSize);

				return volumeToInvestInQuoteCurrency;
			}
		}

		/// <summary>
		/// Existe t-il au moins une position du type précisé
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="tradeType">type Buy ou Sell</param>
		/// <returns>true si le robot actuel possède au moins une position du type tradeType, false sinon</returns>
		public static bool existPositions(this Robot robot, TradeType tradeType, string label = "")
		{
			return (robot.Positions.Find(label, robot.Symbol, tradeType) != null);
		}


		/// <summary>
		/// Existe t-il au moins une position à l'achat active
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <returns>true si le robot actuel possède au moins une position à l'achat, false sinon</returns>
		public static bool existBuyPositions(this Robot robot, string label = "")
		{
			return robot.existPositions(TradeType.Buy,label);
		}

		/// <summary>
		/// Existe t-il au moins une position à la vente active
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <returns>true si le robot actuel possède au moins une position à la vente, false sinon</returns>
		public static bool existSellPositions(this Robot robot, string label = "")
		{
			return robot.existPositions(TradeType.Sell, label); 
		}

		/// <summary>
		/// Existe t-il au moins une position à la vente active et au moins une à l'achat
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <returns>true si le robot actuel possède au moins une position à l'achat, et au moins une à la vente,false sinon</returns>
		public static bool existBuyAndSellPositions(this Robot robot, string label = "")
		{
			return robot.existBuyPositions(label) && robot.existSellPositions(label);
		}

		/// <summary>
		/// Y a t-il aucune position active
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <returns>true Positions.count==0, false sinon</returns>
		public static bool isNoPositions(this Robot robot, string label = "")
		{
			return !(robot.existBuyPositions(label)) && !(robot.existSellPositions(label)); 
		}

		/// <summary>
		/// Clôture une position, et gère un message d'erreur en cas d'insuccès
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="position">La position à clôturer</param>
		public static TradeResult closePosition(this Robot robot, Position position, double volume)
		{

			TradeResult result;

			if(volume == 0)
				result = robot.ClosePosition(position);
			else
				result = robot.ClosePosition(position, robot.Symbol.NormalizeVolume(volume,RoundingMode.ToNearest));

			return result;
		}

		/// <summary>
		/// Clôture une position, et gère un message d'erreur en cas d'insuccès
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="position">La position à clôturer</param>
		public static void closePosition(this Robot robot, Position position)
		{
			var result = robot.ClosePosition(position);

			if (!result.IsSuccessful)
				robot.Print("error : {0}", result.Error);
		}
		
		/// <summary>
		/// Cloture toute les positions de type tradeType
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="tradeType">Le type de trade à clôturer</param>
		public static void closeAllPositions(this Robot robot, TradeType tradeType, string label = "")
		{
			foreach (Position position in robot.Positions.FindAll(label, robot.Symbol, tradeType))
				robot.closePosition(position);

		}

		/// <summary>
		/// Cloture toute les positions de type Buy
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		public static void closeAllBuyPositions(this Robot robot, string label = "")
		{
			closeAllPositions(robot, TradeType.Buy, label);
		}

		/// <summary>
		/// Cloture toute les positions de type Sell
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		public static void closeAllSellPositions(this Robot robot, string label = "")
		{
			closeAllPositions(robot, TradeType.Sell, label);
		}

		/// <summary>
		/// Clôture toutes les positions du robot
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		public static void closeAllPositions(this Robot robot, string label = "")
		{
			robot.closeAllBuyPositions(label);
			robot.closeAllSellPositions(label);
		}

		/// <summary>
		/// Cancel all pending orders of type 'tradeType'
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="tradeType">Tradetype to cancel</param>
		public static void cancelAllPendingOrders(this Robot robot, TradeType tradeType, string label = "")
		{
			foreach (PendingOrder order in robot.PendingOrders)
				if (order.Label == label && order.SymbolCode == robot.Symbol.Code && order.TradeType==tradeType)
                    robot.CancelPendingOrderAsync(order);
		}

		/// <summary>
		/// Cancel all buy pending orders
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		public static void cancelAllPendingBuyOrders(this Robot robot, string label = "")
		{
			cancelAllPendingOrders(robot, TradeType.Buy, label);
		}

		/// <summary>
		/// Cancel all sell pending orders
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		public static void cancelAllPendingSellOrders(this Robot robot, string label = "")
		{
			cancelAllPendingOrders(robot, TradeType.Sell, label);
		}

		/// <summary>
		/// Cancel all pending orders
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		public static void cancelAllPendingOrders(this Robot robot, string label = "")
		{
			robot.cancelAllPendingBuyOrders(label);
			robot.cancelAllPendingSellOrders(label);
		}

		/// <summary>
		/// Notify a message at mail address
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="headMessage"></param>
		/// <param name="message"></param>
		/// <param name="mailAddress"></param>
		public static void notifyMessage(this Robot robot, string headMessage, string message, MailAddress mailAddress)
		{
			robot.Notifications.SendEmail("cAlgoBot@cAlgoLib.com", mailAddress.Address, headMessage, message);
		}

		/// <summary>
		///	send email to with the balance, profit etc
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="error"></param>
		public static void notifyError(this Robot robot, Error error, MailAddress mailAddress)
		{
			string errorText = robot.errorString(error);

			if (error.Code == ErrorCode.NoMoney || error.Code == ErrorCode.TechnicalError)
				robot.notifyMessage("Robot Error : ", errorText, mailAddress);
			else
				if (error.Code == ErrorCode.MarketClosed)
					robot.notifyMessage("End of week report : ", errorText, mailAddress);

		}

		/// <summary>
		/// transform code error to String error.
		/// </summary>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="error"></param>
		/// <returns></returns>
		public static string errorString(this Robot robot, Error error)
		{
			string errorText = "";

			switch (error.Code)
			{
				case ErrorCode.BadVolume: errorText = "Bad volume";
					break;
				case ErrorCode.TechnicalError: errorText = "Technical Error";
					break;
				case ErrorCode.NoMoney: errorText = "No Money";
					break;
				case ErrorCode.Disconnected: errorText = "Disconnected";
					break;
				case ErrorCode.MarketClosed: errorText = "Market Closed";
					break;
			}


			if (error.Code == ErrorCode.BadVolume || error.Code == ErrorCode.Disconnected)
				errorText="Error:" + errorText;

			if (error.Code == ErrorCode.MarketClosed)
			{
				StringBuilder report = new StringBuilder("End of trading week report for the week of:");
				report.Append(DateTime.Now);
				report.Append("\n");
				report.Append("Account Balance:");
				report.Append(robot.Account.Balance);
				report.Append("\n");

				return report.ToString();
			}

			return errorText;
		}

		/// <summary>
		/// Close losses positions as the saying goes jogging leave earnings, fenced losses.
		/// </summary>
		public static void partialClose(this Robot robot, string label = "")
		{
			foreach (var position in robot.Positions.FindAll(label, robot.Symbol))
			{
				if (position.TakeProfit.HasValue && position.StopLoss.HasValue)
				{
					string ident = position.Comment.Substring(position.Comment.Length - 1, 1);
					double? percentLoss = position.percentLoss();

					if (percentLoss.HasValue && (((percentLoss <= -0.33) && (ident == "1")) || ((percentLoss <= -0.66) && (ident == "2"))))
						robot.closePosition(position);

				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tradeType"></param>
		/// <param name="volume"></param>
		/// <param name="label"></param>
		public static void executeOrder(this Robot robot, OrderParams op)
		{
			if (op==null)
				throw new System.ArgumentException(String.Format("parameter 'op' must be non null", op));
			if (op.Volume <= 0)
				throw new System.ArgumentException(String.Format("parameter 'op.Volume' must be strictly positive", op.Volume));
			if (!op.TradeType.HasValue)
				throw new System.ArgumentException(String.Format("parameter 'op.TradeType' must have a value", op.TradeType));


			// it is necessary that the volume is a multiple of "microvolume".
			long v = robot.Symbol.NormalizeVolume(op.Volume.Value, RoundingMode.ToNearest);

			if (v > 0)
			{
				var result = robot.ExecuteMarketOrder(op.TradeType.Value, op.Symbol, v, op.Label, op.StopLoss, op.TakeProfit, op.Slippage, op.Comment);
				if (!result.IsSuccessful)
					robot.Print("error : {0}, {1}", result.Error, v);
			}
		}

		/// <summary>
		/// cutting an order in n orders such that the sum of the volume equal initial volume.
		/// </summary>
		/// <param name="tradeType"></param>
		/// <param name="volume"></param>
		/// <param name="prefixLabel"></param>
		public static void splitAndExecuteOrder(this Robot robot, OrderParams op)
		{
			if (op == null)
				throw new System.ArgumentException(String.Format("parameter 'op' must be non null", op));
			if (op.Volume <= 0)
				throw new System.ArgumentException(String.Format("parameter 'op.Volume' must be strictly positive", op.Volume));

			double sum = op.Parties.Sum(x => Math.Abs(x));
			OrderParams partialOP = new OrderParams(op);
			List<double> l = new List<double>(op.Parties);
			l.Sort();

			for(int i=l.Count-1; i>=0; i--)
			{
				partialOP.Volume = op.Volume.Value * l[i] / sum;
				partialOP.Comment = string.Format("{0}-{1}",partialOP.Comment,i);

				robot.executeOrder(partialOP);
			}

		}

		/// <summary>
		/// Return a buy, sell or neutral tradeType. 
		/// </summary>
		/// <param name="ceilSignal"></param>
		/// <returns>null : no tradeType (neutral), Buy or Sell</returns>
		public static TradeType? signal(this Robot robot, List<Strategy> strategies, int? ceilSignal=null)
		{
			int ceil;
			if (ceilSignal.HasValue)
				ceil=ceilSignal.Value;
			else
				ceil = strategies.Count;

			int signal = 0;
			foreach (Strategy strategy in strategies)
				signal += strategy.signal().factor();

			if (signal >= ceil)
				return TradeType.Buy;
			else
				if (signal <= -ceil)
					return TradeType.Sell;

			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="robot"></param>
		public static OrderParams martingale(this Robot robot, Position position, bool inversePosition=true, double martingaleCoeff=1.5)
		{
			if ((position != null) && position.Pips < 0)
			{
				OrderParams op = new OrderParams(position);
				op.Comment = string.Format("{0}-{1}", position.Comment, "Mart");
				if (inversePosition)
					op.TradeType = position.TradeType.inverseTradeType();
				op.Volume = position.Volume * martingaleCoeff;

				return op;
			}
			else
				return null;
		}

	}
}
