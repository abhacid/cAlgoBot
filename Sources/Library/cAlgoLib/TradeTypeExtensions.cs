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

namespace cAlgo.Lib
{
	/// <summary>
	/// Méthodes d'extensions du type cAlgo.API.TradeType
	/// </summary>
	public static class TradeTypeExtensions
	{
		/// <summary>
		/// Est-ce une position d'achat
		/// </summary>
		/// <param name="tradeType">le type de trade utilisé</param>
		/// <returns>true si achat, false sinon</returns>
		public static bool isBuy(this TradeType? tradeType)
		{
			if (tradeType.HasValue)
				return TradeType.Buy == tradeType;

			return false;
		}

		/// <summary>
		/// Est-ce une position d'achat
		/// </summary>
		/// <param name="tradeType">le type de trade utilisé</param>
		/// <returns>true si achat, false sinon</returns>
		public static bool isBuy(this TradeType tradeType)
		{
			return TradeType.Buy == tradeType;
		}
		/// <summary>
		/// Est-ce une position de vente
		/// </summary>
		/// <param name="tradeType">le type de trade utilisé</param>
		/// <returns>true si vente, false sinon</returns>
		public static bool isSell(this TradeType? tradeType)
		{
			if (tradeType.HasValue)
				return TradeType.Sell == tradeType;

			return false;
		}

		/// <summary>
		/// Est-ce une position de vente
		/// </summary>
		/// <param name="tradeType">le type de trade utilisé</param>
		/// <returns>true si vente, false sinon</returns>
		public static bool isSell(this TradeType tradeType)
		{
			return TradeType.Sell == tradeType;
		}

		/// <summary>
		/// Est-ce une position de vente
		/// </summary>
		/// <param name="tradeType">le type de trade utilisé</param>
		/// <returns>true tradeType n'a pas de valeur, false sinon</returns>
		public static bool isNothing(this TradeType? tradeType)
		{
			return !(tradeType.HasValue);
		}


		/// <summary>
		/// Transforme TradeType.Buy en 1, TradeType.Sell en -1, null en 0
		/// </summary>
		/// <param name="tradeType">le type de trade utilisé</param>
		/// <returns>1 si achat, -1 si vente</returns>
		public static int factor(this TradeType? tradeType)
		{
			if (tradeType.HasValue)
				return tradeType.Value.factor();
			else
				return 0;
		}

		/// <summary>
		/// Renvoie 1 si c'est une position longue, -1 si c'est une position courte
		/// </summary>
		/// <param name="tradeType">le type de trade utilisé</param>
		/// <returns>1 si achat, -1 si vente</returns>
		public static int factor(this TradeType tradeType)
		{
			return (tradeType.isBuy()).factor();
		}

		/// <summary>
		/// Inverse TradeType
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>cAlgo.API.TradeType</returns>
		public static TradeType inverseTradeType(this TradeType tradeType)
		{
			return (tradeType.isBuy() ? TradeType.Sell : TradeType.Buy);
		}

		/// <summary>
		/// Inverse TradeType?
		/// </summary>
		/// <param name="position">Used position</param>
		/// <returns>cAlgo.API.TradeType?</returns>
		public static TradeType? inverseTradeType(this TradeType? tradeType)
		{
			if (tradeType.HasValue)
				return tradeType.Value.inverseTradeType();
			else
				return null;
		}

	}
}
