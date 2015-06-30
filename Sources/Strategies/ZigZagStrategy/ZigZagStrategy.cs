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


using cAlgo;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using cAlgo.Lib;

namespace cAlgo.Strategies
{
    public class ZigZagStrategy : Strategy
	{
		#region Strategy Parameters
		#endregion

		double zigZagPrevValue;

		public object ZzDepth { get; set; }
		public object ZzDeviation { get; set; }
		public object ZzBackStep { get; set; }

		ZigZagIndicator zigZag;

		public ZigZagStrategy(Robot robot, int ZzDepth, int ZzDeviation, int ZzBackStep) : base(robot)
		{
			this.ZzDepth = ZzDepth;
			this.ZzDeviation = ZzDeviation;
			this.ZzBackStep = ZzBackStep;

			Initialize();
		}

		protected override void Initialize()
		{
			zigZag = Robot.Indicators.GetIndicator<ZigZagIndicator>(ZzDepth, ZzDeviation, ZzBackStep);

		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public override TradeType? signal()
		{
			double lastValue = zigZag.Result.LastValue;

			TradeType? tradeType = null;

			if (!double.IsNaN(lastValue))
			{
				if (lastValue < zigZagPrevValue)
					tradeType = TradeType.Buy;
				else if (lastValue > zigZagPrevValue && zigZagPrevValue > 0.0)
					tradeType = TradeType.Sell;

				zigZagPrevValue = lastValue;
			}

			return tradeType;
		}
	}
}
