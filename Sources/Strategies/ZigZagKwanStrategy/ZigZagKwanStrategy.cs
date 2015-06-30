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
	/// <summary>
	/// 	ZigZag Kwan MBFX Timing ou Beta
	/// </summary>
	public class ZigZagKwanStrategy : Strategy
	{
		#region Strategy Parameters
		#endregion
		public int MbfxLen { get; set; }
		public double MbfxFilter { get; set; }
		
		private ZigZagKwanIndicator zzKMBFXT;

		public ZigZagKwanStrategy(Robot robot, int MbfxLen, double MbfxFilter)
			: base(robot)
		{
			this.MbfxLen = MbfxLen;
			this.MbfxFilter = MbfxFilter;

			Initialize();
		}

		protected override void Initialize()
		{
			zzKMBFXT = Robot.Indicators.GetIndicator<ZigZagKwanIndicator>(MbfxLen, MbfxFilter);

		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public override TradeType? signal()
		{
			if (zzKMBFXT.signal.HasCrossedAbove(ZigZagKwanIndicator.standSignal + 0.5, 0))
			{
				return TradeType.Buy;
			}
			else if (zzKMBFXT.signal.HasCrossedBelow(ZigZagKwanIndicator.standSignal - 0.5, 0))
			{
				return TradeType.Sell;
			}

			return null;
		}
	}
}

