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


using System;
using System.Diagnostics;
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
			public int CandleSize { get; set; }
			public int? BollingerDivisions { get; set; }
		#endregion


		private RelativeStrengthIndex _rsi;
		private BollingerBands _bollingerBand;

		public DoubleCandleStrategy(Robot robot, int period, int candleSize, int? bollingerDivisions=null)
			: base(robot)
		{
			this.Period = period;
			this.CandleSize = candleSize;
			this.BollingerDivisions = bollingerDivisions;

			Initialize();
		}

		protected override void Initialize()
		{
			_rsi = Robot.Indicators.RelativeStrengthIndex(Robot.MarketSeries.Close,Period);
			_bollingerBand = Robot.Indicators.BollingerBands(Robot.MarketSeries.Close, 20, 2, MovingAverageType.Simple);

			Debug.Assert(CandleSize >= 0, "The candle size must be greather than 0");

		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public override TradeType? signal()
		{
			double candleSize = CandleSize * Robot.Symbol.PipSize;
			int lastIndex = Robot.MarketSeries.Close.Count - 2;
			int previewIndex = lastIndex - 1;

			double previewOpen = Robot.MarketSeries.Open[previewIndex];
			double previewClose = Robot.MarketSeries.Close[previewIndex];
			double lastOpen = Robot.MarketSeries.Open[lastIndex];
			double lastClose = Robot.MarketSeries.Close[lastIndex];

			double thresholTridggering = (BollingerDivisions.HasValue) ? (_bollingerBand.Top.LastValue - _bollingerBand.Bottom.LastValue) / BollingerDivisions.Value : 0;

			bool bollingerTestBuy = Math.Abs(_bollingerBand.Top.LastValue - Robot.Symbol.Mid()) >= thresholTridggering;
			bool bollingerTestSell = Math.Abs(_bollingerBand.Bottom.LastValue - Robot.Symbol.Mid()) >= thresholTridggering;

			if (!Robot.existBuyPositions() && (lastClose > lastOpen + candleSize) && (previewClose > previewOpen + candleSize) && _rsi.Result.LastValue<65 && bollingerTestBuy)
				return TradeType.Buy;

			if (!Robot.existSellPositions() && (lastClose + candleSize < lastOpen) && (previewClose + candleSize < previewOpen) && _rsi.Result.LastValue>35 && bollingerTestSell)
				return TradeType.Sell;

			return null;
		}



	}
}

