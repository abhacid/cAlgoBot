//Date: 18/12/2014
//Country: Chile
//Copyright: Felipe Sepulveda Maldonado 
//LinkedIn: https://cl.linkedin.com/in/felipesepulvedamaldonado
//Facebook: https://www.facebook.com/mymagicflight1
//Whats Up: +56 9 58786321
//Donations Wallet: wallet.google.com felipe.sepulveda@gmail.com
//Paper: Moving Averages for Financial Data Smoothing. EHMA is the Best.
//Paper Authors: Aistis Raudys, Edmundas Malčius, and Vaidotas Lenčiauskas
//As can be seen winning algorithms are Exponential Hull and TRIX. TRIX is the
//leader between stocks and EHMA is everywhere else. For Futures, Forex and ETF
//TRIX is the second best algorithm.
//

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class EHMA : Indicator
    {
        [Output("EHMA", Color = Colors.Orange)]
        public IndicatorDataSeries ehma { get; set; }

        [Parameter(DefaultValue = 21)]
        public int Period { get; set; }

        private IndicatorDataSeries diff;
        private ExponentialMovingAverage ema1;
        private ExponentialMovingAverage ema2;
        private ExponentialMovingAverage ema3;

        protected override void Initialize()
        {
            diff = CreateDataSeries();
            ema1 = Indicators.ExponentialMovingAverage(MarketSeries.Close, (int)(Period / 2));
            ema2 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period);
            ema3 = Indicators.ExponentialMovingAverage(diff, (int)(Math.Sqrt(Period)));
        }

        public override void Calculate(int index)
        {
            double var1 = 2 * ema1.Result[index];
            double var2 = ema2.Result[index];

            diff[index] = var1 - var2;

            ehma[index] = ema3.Result[index];
        }
    }
}
