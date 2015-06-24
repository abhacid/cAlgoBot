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

// Project Hosting for Open Source Software on Codeplex : https://github.com/abhacid/cAlgoBot
#endregion


using cAlgo.API;
using cAlgo.API.Internals;
using System.Collections.Generic;

namespace cAlgo.Lib
{
	/// <summary>
	///
	/// </summary>
	public class OrderParams
	{
		public TradeType? TradeType { get; set; }
		public Symbol Symbol { get; set; }
		public double? Volume { get; set; }
		public string Label { get; set; }
		public double? StopLoss { get; set; }
		public double? TakeProfit { get; set; }
		public double? Slippage { get; set; }
		public string Comment { get; set; }
		public int? Id { get; set; }

		public List<double> Parties{ get; set; }

		public OrderParams()
		{ }

		public OrderParams(TradeType? tradeType, Symbol symbol, double? volume, string label, double? stopLoss, double? takeProfit, double? slippage, string comment, int? id, List<double> parties)
		{
			TradeType = tradeType;
			Symbol = symbol;
			Volume = volume;
			Label = label;
			StopLoss = stopLoss;
			TakeProfit = takeProfit;
			Slippage = slippage;
			Comment = comment;
			Id = id;

			Parties = parties;
		}


		public OrderParams(Position p)
		{
			//Robot = robot;
			TradeType = p.TradeType;
			Symbol = (new Robot()).MarketData.GetSymbol(p.SymbolCode);
			Volume = p.Volume;
			Label = p.Label;
			StopLoss = p.StopLoss;
			TakeProfit = p.TakeProfit;
			Slippage = null;
			Comment = p.Comment;
			Id = p.Id;
		}


		public OrderParams(OrderParams op) : this(op.TradeType,op.Symbol,op.Volume,op.Label,op.StopLoss,op.TakeProfit, op.Slippage, op.Comment, op.Id,op.Parties){}


	}
}
