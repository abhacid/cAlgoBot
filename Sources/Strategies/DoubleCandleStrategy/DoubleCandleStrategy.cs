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


using cAlgo;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using cAlgo.Lib;

namespace cAlgo.Strategies
{
	public class DoubleCandleStrategy : Strategy
	{
		#region Strategy Parameters
			public int Period { get; set; }
			public int DoubleCandleStep { get; set; }
		#endregion


		private RelativeStrengthIndex _rsi;

		public DoubleCandleStrategy(Robot robot, int doubleCandleperiod, int doubleCandleStep): base(robot)
		{
			this.Period = doubleCandleperiod;
			this.DoubleCandleStep = doubleCandleStep;

			Initialize();
		}

		protected override void Initialize()
		{
			_rsi = Robot.Indicators.RelativeStrengthIndex(Robot.MarketSeries.Close,Period);

		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public override TradeType? signal()
		{
			double step = DoubleCandleStep * Robot.Symbol.PipSize;
			int lastIndex = Robot.MarketSeries.Close.Count - 2;
			int previewIndex = lastIndex - 1;

			double previewOpen = Robot.MarketSeries.Open[previewIndex];
			double previewClose = Robot.MarketSeries.Close[previewIndex];
			double lastOpen = Robot.MarketSeries.Open[lastIndex];
			double lastClose = Robot.MarketSeries.Close[lastIndex];


			if (!Robot.existBuyPositions() && (lastClose > lastOpen + step) && (previewClose > previewOpen + step) && lastClose > previewClose && _rsi.Result.LastValue<65)
			{
				Robot.closeAllSellPositions();
				return TradeType.Buy;
			}

			if (!Robot.existSellPositions() && (lastClose + step < lastOpen) && (previewClose + step < previewOpen) && lastClose < previewClose && _rsi.Result.LastValue>35)
			{
				Robot.closeAllBuyPositions();
				return TradeType.Sell;
			}

			return null;
		}



	}
}

