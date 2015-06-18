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
	public class TrendMagicStrategy : Strategy
	{
		#region Strategy Parameters
		public int TmCciPeriod { get; set; }

		public int TmAtrPeriod { get; set; }
		#endregion

		double zigZagPrevValue;

		TrendMagicIndicator trendMagic;
		private CommodityChannelIndex _cci;

		public TrendMagicStrategy(Robot robot,int tmCciPeriod, int tmAtrPeriod) : base(robot)
		{
			this.TmAtrPeriod = tmAtrPeriod;
			this.TmCciPeriod = TmCciPeriod;

			Initialize();
		}

		protected override void Initialize()
		{
			trendMagic = Robot.Indicators.GetIndicator<TrendMagicIndicator>(TmCciPeriod, TmAtrPeriod);
			_cci = Robot.Indicators.CommodityChannelIndex(TmCciPeriod);


		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public override TradeType? signal()
		{
			if (!(Robot.existBuyPositions()) && trendMagic.BufferUpOutput.HasCrossedAbove(Robot.Symbol.Ask, 1) && _cci.Result.LastValue > 0)
			{
				Robot.closeAllSellPositions();
				return TradeType.Buy;
			}
			else
				if (!(Robot.existSellPositions()) && trendMagic.BufferDnOutput.HasCrossedBelow(Robot.Symbol.Bid, 1) && _cci.Result.LastValue < 0)
				{
					Robot.closeAllBuyPositions();
					return TradeType.Sell;
				}

			return null;
		}


	}
}
