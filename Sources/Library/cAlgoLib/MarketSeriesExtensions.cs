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
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Lib
{
	/// <summary>
	/// Méthodes d'extensions du type cAlgo.API.Internals.MarketSeries
	/// </summary>
	public static class MarketSeriesExtensions
	{
		/// <summary>
		/// Détermine le nombre de bougies des prix du marché
		/// </summary>
		/// <param name="marketSeries">L'instance de MarketSeries actuel</param>
		/// <param name="robot">instance of the current robot</param>
		/// <returns>nombre de bougies des prix du marché</returns>
		public static int Bars(this MarketSeries marketSeries)
		{
			return marketSeries.Open.Count;
		}

		/// <summary>
		/// Détermine si la bougie est haussière (Bullisk) ou baissière (Bearish)
		/// </summary>
		/// <param name="marketSeries">L'instance de MarketSeries actuel</param>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="index">index de la bougie à tester (ordre inversé)</param>
		/// <returns>true si la bougie est haussière, false sinon</returns>
		public static bool? isBullCandle(this MarketSeries marketSeries, int index)
		{
			int count = marketSeries.Bars();

			if (index >= 0 && index < count)
			{
				double open = marketSeries.Open[count - 1 - index];
				double close = marketSeries.Close[count - 1 - index];
				double median = marketSeries.Median[count - 1 - index];
				double variation;

				if (double.IsNaN(close))
					variation = median-open;
				else
					variation = close - open;

				if (variation > 0)
					return true;
				else
					if (variation < 0)
						return false;
					else
						return null;
			}
			else
				throw new ArgumentException(string.Format("Valeur de l'indice {0} en dehors des valeurs permises", index));
		}

		/// <summary>
		/// Détermine si la bougie est haussière (Bullisk) ou baissière (Bearish)
		/// </summary>
		/// <param name="marketSeries">L'instance de MarketSeries actuel</param>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="index">index de la bougie à tester (ordre inversé)</param>
		/// <returns>true si la bougie est baissière</returns>
		public static bool? isBearCandle(this MarketSeries marketSeries, int index)
		{
			bool? returnValue = marketSeries.isBullCandle(index);
			
			return (returnValue!=null) ? !returnValue : null;

		}

		/// <summary>
		/// Teste si une bougie est au dessus d'une limite
		/// </summary>
		/// <param name="marketSeries">L'instance de MarketSeries actuel</param>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="index">index de la bougie à tester (ordre inversé)</param>
		/// <param name="limit">limite horizontale à tester par rapport à la bougie</param>
		/// <returns>true si la bougie est au dessus de la limite, false sinon</returns>
		public static bool isCandleAbove(this MarketSeries marketSeries, int index, double limit)
		{
			int count = marketSeries.Bars();

			if (index >= 0 && index < count)
				return marketSeries.Low[count - 1 - index] >= limit;
			else 
				throw new ArgumentException(string.Format("Valeur de l'indice {0} en dehors des valeurs permises", index));


		}

		/// <summary>
		/// Teste si une bougie est en dessous d'une bougie
		/// </summary>
		/// <param name="marketSeries">L'instance de MarketSeries actuel</param>
		/// <param name="index">index de la bougie à tester (ordre inversé)</param>
		/// <param name="limit">limite horizontale à tester par rapport à la bougie</param>
		/// <returns>true si la bougie est au dessous de la limite, false sinon</returns>
		public static bool iscandleBelow(this MarketSeries marketSeries, int index, double limit)
		{
			int count = marketSeries.Bars();

			if (index >= 0 && index < count)
				return marketSeries.High[count - 1 - index] <= limit;
			else
				throw new ArgumentException(string.Format("Valeur de l'indice {0} en dehors des valeurs permises", index));


		}
		
		/// <summary>
		/// Vérifie si une bougie traverse une frontière
		/// </summary>
		/// <param name="marketSeries">L'instance de MarketSeries actuel</param>
		/// <param name="index">index de la bougie à tester (ordre inversé)</param>
		/// <param name="frontier">frontière horizontale à tester par rapport à la bougie</param>
		/// <returns>true si la bougie est sur la frontière, false sinon</returns>
		public static bool isCandleOver(this MarketSeries marketSeries, int index, double frontier)
		{
			int count = marketSeries.Bars();

			if (index >= 0 && index < count)
			{
				return frontier.between(marketSeries.Low[count - 1 - index], marketSeries.High[count - 1 - index]);

			}
			else
				throw new ArgumentException(string.Format("Valeur de l'indice {0} en dehors des valeurs permises", index));

		}

		/// <summary>
		/// Vérifie si une bougie est entre deux bornes basse et haute
		/// </summary>
		/// <param name="marketSeries">L'instance de MarketSeries actuel</param>
		/// <param name="index">index de la bougie à tester (ordre inversé)</param>
		/// <param name="dn">Borne inférieure horizontale</param>
		/// <param name="up">Borne supérieure horizontale</param>
		/// <returns>true si la bougie est entre les deux bornes, false sinon</returns>
		public static bool isCandleBetween(this MarketSeries marketSeries, int index, double dn, double up)
		{
			return isCandleAbove(marketSeries, index, dn) && iscandleBelow(marketSeries, index, up);

		}

		/// <summary>
		/// dispersion of a marketSeries
		/// </summary>
		/// <param name="marketSeries"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public static double volatility(this MarketSeries marketSeries, int period)
		{
			double maximum = marketSeries.High.Maximum(period);
			double minimum = marketSeries.Low.Minimum(period);

			return (maximum - minimum);
		}
		/// <summary>
		/// dispersion of a marketSeries in Pips
		/// </summary>
		/// <param name="marketSeries"></param>
		/// <param name="period"></param>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static double volatilityInPips(this MarketSeries marketSeries, int period, Symbol symbol)
		{
			return marketSeries.volatility(period) / symbol.PipSize;
		}

		/// <summary>
		/// Renvoie le nombre de bougies, selon l'instrument et timeframe courant du robot, terminees apres l'entree en position
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="robot">instance of the current robot</param>
		/// <param name="position">Used position</param>
		/// <returns>Nombre de bougies écoulée depuis l'entrée en position</returns>
		public static int barsAgo(this MarketSeries marketSeries, Position position)
		{
			for(var i = marketSeries.OpenTime.Count - 1; i >= 0; i--)
			{
				if(position.EntryTime > marketSeries.OpenTime[i])
					return marketSeries.OpenTime.Count - 1 - i;
			}
			return -1;
		}

		/// <summary>
		/// Determine the global candle interval time that contain this instant time
		/// </summary>
		/// <param name="series"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public static int GetIndexByDate(this MarketSeries series, DateTime time)
		{
			for(int i = series.Open.Count - 1; i > 0; i--)
			{
				if(time >= series.OpenTime[i])
					return i;
			}
			return -1;
		}
	}
}
