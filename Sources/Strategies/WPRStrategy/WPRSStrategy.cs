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
	public class WPRSStrategy : Strategy
	{
		#region Strategy Parameters
		public DataSeries WprSource { get; set; }
		public int WprPeriod { get; set; }
		public int WprOverbuyCeil { get; set; }
		public int WprOversellCeil { get; set; }
		public int WprCrossedPeriod { get; set; }
		public int WprMinMaxPeriod { get; set; }
		public int WprExceedMinMax { get; set; }

		#endregion

		WPRSIndicator wprs;

		public WPRSStrategy(Robot robot, DataSeries wprSource, int wprPeriod, int wprOverbuyCeil, int wprOversellCeil, int wprCrossedPeriod, int wprMinMaxPeriod, int wprExceedMinMax): base(robot)
		{
			this.WprSource = wprSource;
			this.WprPeriod = wprPeriod;
			this.WprOverbuyCeil = wprOverbuyCeil;
			this.WprOversellCeil = wprOversellCeil;
			this.WprCrossedPeriod = wprCrossedPeriod;
			this.WprMinMaxPeriod = wprMinMaxPeriod;
			this.WprExceedMinMax = wprExceedMinMax;

			Initialize();
		}

		protected override void Initialize()
		{
			wprs = Robot.Indicators.GetIndicator<WPRSIndicator>(WprSource, WprPeriod, WprOverbuyCeil, WprOversellCeil, WprCrossedPeriod, WprMinMaxPeriod, WprExceedMinMax);

		}

		/// <summary>
		/// Strategy according to Williams Percent Range indicator
		/// </summary>
		/// <returns></returns>
		public override TradeType? signal()
		{
			if (wprs.IsExceedLow && wprs.IsCrossAboveOversell)
				return TradeType.Buy;
			else if (wprs.IsCrossBelowOverbuy && wprs.IsExceedHigh)
				return TradeType.Sell;

			return null;
		}


	}
}
