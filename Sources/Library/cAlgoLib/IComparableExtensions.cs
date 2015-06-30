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

namespace cAlgo.Lib
{
	/// <summary>
	/// Méthodes d'extensions du type IComparableExtensions
	/// </summary>
	public static class IComparableExtensions
	{
		/// <summary>
		/// comparaison entre deux bornes pour une date, une chaîne, un entier
		/// </summary>
		/// <typeparam name="T">Type générique implémentant l'interface IComparable T </typeparam>
		/// <param name="value">L'instance à comparer</param>
		/// <param name="from">La borne inférieure</param>
		/// <param name="to">La borne supérieure</param>
		/// <returns>true si number est entre from et to, false sinon</returns>
		public static bool between<T>(this T value, T from, T to) where T : IComparable<T>
		{
			return value.CompareTo(from) >= 0 && value.CompareTo(to) <= 0;
		}
	}
}
