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
using cAlgo.API.Internals;
using cAlgo.API;

namespace cAlgo.Lib
{
	/// <summary>
	/// Méthodes d'extensions du type cAlgo.API.DataSeries
	/// </summary>
	public static class DataSeriesExtensions
	{
		/// <summary>
		/// Recherche la valeur d'un élément de dataSeries de valeur différente de NaN
		/// </summary>
		/// <param name="dataSeries">La série de donnée utilisée</param>
		/// <param name="shift">le nombre-1 de valeurs différentes de NaN se trouvant après la valeur retournée</param>
		/// <returns>La valeur correspondante au iPos élément de dataSeries différent de NaN</returns>
		public static double lastRealValue(this DataSeries dataSeries, int shift)
		{
			for (var i = dataSeries.Count - 1; i >= 0; i--)
			{
				if (!double.IsNaN(dataSeries[i]))
				{
					if (shift == 0)
						return dataSeries[i];
					shift--;
				}
			}
			return double.NaN;
		}

		/// <summary>
		/// Recherche l'index d'un élément de dataSeries de valeur différente de NaN
		/// </summary>
		/// <param name="dataSeries">La série de donnée utilisée</param>
		/// <param name="iPos">le nombre-1 de valeur différentes de NaN se trouvant après la valeur retournée</param>
		/// <returns>L'index correspondante au iPos élément de dataSeries différent de NaN</returns>
		public static int lastRealIndex(this DataSeries dataSeries, int nFromEnd)
		{
			for (var i = dataSeries.Count - 1; i >= 0; i--)
			{
				if (!double.IsNaN(dataSeries[i]))
				{
					if (nFromEnd == 0)
						return i;
					nFromEnd--;
				}
			}
			return -1;
		}

		/// <summary>
		///	performs a transform function to each item in the list from index 'from' to index 'to'
		/// </summary>
		/// <param name="dataseries">DataSeries used</param>
		/// <param name="outIndicatorDataseries">returned IndicatorDataseries</param>
		/// <param name="f">boolean function</param>
		/// <param name="from">start index</param>
		/// <param name="to">end index</param>
		/// <returns>the calculated values</returns>
		public static IndicatorDataSeries map(this DataSeries dataseries, ref IndicatorDataSeries outIndicatorDataseries, Func<double, double> f, int from = 0, int? to = null)
		{
			if (!to.HasValue)
				to = dataseries.Count - 1;

			if (!dataseries.isValidInterval(from, to))
				throw new System.ArgumentException(String.Format("the 'from' : {0} parameter must be lower than 'to' : {1} and from above 0 and to below Count : {3}", from, to, dataseries.Count));

			double result = dataseries[from];

			for (int i = from + 1; i <= to.Value; i++)
				outIndicatorDataseries[i] = f(dataseries[i]);

			return outIndicatorDataseries;
		}

		/// <summary>
		/// Allows clipping negative values ​​in a marketSeries.
		/// </summary>
		/// <param name="dataseries">DataSeries used</param>
		/// <param name="outIndicatorDataseries">IndicatorDataSeries to be changed</param>
		/// <returns>pos(indicatorDataSeries)</returns>
		public static IndicatorDataSeries pos(this DataSeries dataseries, ref IndicatorDataSeries outIndicatorDataseries)
		{
			return dataseries.map(ref outIndicatorDataseries, _ => { if (_ >= 0) return _; else return double.NaN; });
		}

		/// <summary>
		/// Allows clipping positive values ​​in a marketSeries.
		/// </summary>
		/// <param name="dataseries">DataSeries used</param>
		/// <param name="outIndicatorDataseries">IndicatorDataSeries returned</param>
		/// <returns>neg(indicatorDataSeries)</returns>
		public static IndicatorDataSeries neg(this DataSeries dataseries, ref IndicatorDataSeries outIndicatorDataseries)
		{
			return dataseries.map(ref outIndicatorDataseries, _ => { if (_ <= 0) return _; else return double.NaN; });
		}

		public static T fold<T>(this DataSeries firstDataSeries, DataSeries secondDataSeries, Func<T, double, double, T> f, T i0, int shift = 0, int from = 0, int? to = null)
		{
			if (secondDataSeries == null)
				secondDataSeries = firstDataSeries;

			if (!to.HasValue)
				to = Math.Min(firstDataSeries.Count, secondDataSeries.Count) - 1;

			if (!firstDataSeries.isValidInterval(from, to) || !secondDataSeries.isValidInterval(from, to))
				throw new System.ArgumentException(String.Format("the 'from' : {0} parameter must be lower than 'to' : {1} and 'from' above 0 and 'to' below Count : {3}", from, to, firstDataSeries.Count));

			T acc = i0;

			for (int i = from; i <= to.Value - shift; i++)
			{
				acc = f(acc, firstDataSeries[i], secondDataSeries[i + shift]);
			}

			return acc;
		}

		/// <summary>
		/// The fold method traverses the list by applying an operator to "compress" into a single value.
		/// </summary>
		/// <param name="dataseries">DataSeries used</param>
		/// <param name="f">binary function</param>
		/// <param name="i0"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns>the iterative calculated value</returns>
		public static T fold<T>(this DataSeries dataseries, Func<T , double, T> f, T i0, int from=0, int? to=null)
		{
			return dataseries.fold(dataseries, (a, b, c) => f(a, b), i0, 0, from, to);
		}
		

		public static T fold<T>(this DataSeries firstDataSeries, Func<T, double, double, T> f, T i0, int shift = 0, int from = 0, int? to = null)
		{
			return firstDataSeries.fold(firstDataSeries, f, i0, shift, from, to);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="dataseries"></param>
		/// <param name="f"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static double sumf(this DataSeries dataseries, Func<double, double> f, int from=0, int? to=null)
		{
			return dataseries.fold((acc, value) => acc + f(value), (double)0, from, to);
		}

		/// <summary>
		///	performs a Boolean OR function to each item between 'from' and 'to' in the list
		/// </summary>
		/// <param name="dataseries">DataSeries used</param>
		/// <param name="from">start index</param>
		/// <param name="to">end index</param>
		/// <returns>return true if one returned function value return true</returns>
		public static bool orFilter(this DataSeries dataseries, Func<double, bool> f, int from = 0, int? to = null)
		{
			if (!to.HasValue)
				to = dataseries.Count - 1;

			if (!dataseries.isValidInterval(from,to))
				throw new System.ArgumentException(String.Format("the 'from' : {0} parameter must be lower than 'to' : {1} and from above 0 and to below Count : {3}",from, to,dataseries.Count));

			bool result = false;

			int i = from;
			do
			{
				if (result = result || f(dataseries[i]))
					return true;
			}
			while (!result && i++ < to.Value);

			return result;
		}

		/// <summary>
		///	performs a Boolean AND function to each item between 'from' and 'to' in the list
		/// </summary>
		/// <param name="dataseries">DataSeries used</param>
		/// <param name="f">Boolean function to apply</param>
		/// <param name="from">start index</param>
		/// <param name="to">end index</param>
		/// <returns>return true if all returned function value return true</returns>
		public static bool andFilter(this DataSeries dataseries, Func<double, bool> f, int from = 0, int? to = null)
		{
			return dataseries.fold((acc, value) => acc && f(value), false, from, to);
		}

		public static bool isValidInterval(this DataSeries dataseries, int from, int? to)
		{
			return ((from >= 0) && (from <= to) &&  (to < dataseries.Count));

		}

		public static double volatility(this DataSeries dataseries, int period)
		{
			double maximum = dataseries.Maximum(period);
			double minimum = dataseries.Minimum(period);

			return (maximum - minimum);
		}
		/// <summary>
		/// dispersion of a DataSeries in Pips
		/// </summary>
		/// <param name="marketSeries"></param>
		/// <param name="period"></param>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static double volatilityInPips(this DataSeries dataseries, int period, Symbol symbol)
		{
			return dataseries.volatility(period) / symbol.PipSize;
		}

	}
}
