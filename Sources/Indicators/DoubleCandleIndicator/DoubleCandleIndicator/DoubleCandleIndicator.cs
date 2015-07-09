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

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/Robot_Forex
#endregion

#region Description
// Détecte deux bougie de même type haussières ou baissière en tenant compte d'une taille minimale 
// pour la détection.
#endregion


using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class DoubleCandleIndicator : Indicator
    {
        [Parameter("Signal Fineness", DefaultValue = 0.01)]
        public double SignalFineness { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Signal { get; set; }

        #region Globals
        const int _basePlot = 0;
        const int _sizeSignalPlot = 10;
        const int _spaceBetweenPlot = 2 * _sizeSignalPlot + 5;

        public const int _Neutral = _basePlot;
        public const int _Up = _basePlot + _sizeSignalPlot;
        public const int _Dn = _basePlot - _sizeSignalPlot;

        double _candleCeil;

        #endregion



        protected override void Initialize()
        {
            // Initialize and create nested indicators
            _candleCeil = SignalFineness * Symbol.PipSize;
        }

        public override void Calculate(int index)
        {
            double signal = _Neutral;

            int previewIndex = index - 1;

            double previewOpen = MarketSeries.Open[previewIndex];
            double previewClose = MarketSeries.Close[previewIndex];
            double lastOpen = MarketSeries.Open[index];
            double lastClose = MarketSeries.Close[index];

            if ((lastClose > lastOpen + _candleCeil) && (previewClose > previewOpen + _candleCeil) && (lastOpen >= previewClose))
                signal = _Up;
            else if ((lastClose + _candleCeil < lastOpen) && (previewClose + _candleCeil < previewOpen) && (lastOpen <= previewClose))
                signal = _Dn;

            Signal[index] = signal;

        }
    }
}
