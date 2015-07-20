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




using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;


namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    [Levels(-20, 0, 20)]
    public class MACDPrbSARnoiseIndicator : Indicator
    {
        #region Indicator Parameters
        [Parameter("Period_MACD_SMA", DefaultValue = 9)]
        public int Period_MACD_SMA { get; set; }

        [Parameter("Noise_MACD_sm", DefaultValue = 41)]
        public int Noise_MACD_sm { get; set; }

        [Parameter("Noise_MACD_m0", DefaultValue = 161)]
        public int Noise_MACD_m0 { get; set; }

        [Parameter("Noise_MACD_s0", DefaultValue = 21)]
        public int Noise_MACD_s0 { get; set; }

        [Parameter("Noise_Prb_SAR_ema", DefaultValue = 351)]
        public int Noise_Prb_SAR_ema { get; set; }

        [Parameter("Step_PrbSAR", DefaultValue = 0.031)]
        public double Step_PrbSAR { get; set; }

        [Parameter("Period_SlowEMA", DefaultValue = 26)]
        public int Period_SlowEMA { get; set; }

        [Parameter("Period_FastEMA", DefaultValue = 12)]
        public int Period_FastEMA { get; set; }

        [Output("Main", Color = Colors.CornflowerBlue)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Histogram", Color = Colors.Green)]
        public IndicatorDataSeries Histogram { get; set; }

        [Output("Signal", Color = Colors.Red)]
        public IndicatorDataSeries Signal { get; set; }

        [Output("ParabolicSAR", Color = Colors.Pink)]
        public IndicatorDataSeries ParabolicSAR { get; set; }
        #endregion

        #region Indicators Variables
        private MacdHistogram i_MACD_Histogram;
        private MacdHistogram i_MCAD_signal;
        //	private SimpleMovingAverage i_MA_Close;
        private ParabolicSAR i_Parabolic_SAR;
        private SimpleMovingAverage i_MA_Open;
        private ExponentialMovingAverage i_EMAf;

        double _MACD_Histogram;
        double _MCAD_signal;
        //	double _MA_Close;
        double _Parabolic_SAR;

        bool _isMacdMainPositive;
        bool _isMacdMainAboveMacdSignal;
        bool _isMacdSignalPositive;
        bool _isParabolicSARBelowMaClose;

        private double _dixPowerDigits;
        #endregion

        #region Globals
        const int basePlot = 0;
        const int sizeSignalPlot = 20;
        const int spaceBetweenPlot = 2 * sizeSignalPlot + 5;

        public const int Neutral = basePlot;
        public const int Up = basePlot + sizeSignalPlot;
        public const int Dn = basePlot - sizeSignalPlot;

        #endregion

        protected override void Initialize()
        {

            _dixPowerDigits = Math.Pow(10, Symbol.Digits);

            i_MACD_Histogram = Indicators.MacdHistogram(MarketSeries.Close, Period_SlowEMA, Period_FastEMA, Period_MACD_SMA);
            i_MCAD_signal = Indicators.MacdHistogram(MarketSeries.Close, Period_SlowEMA, Period_FastEMA, Period_MACD_SMA);
            //	i_MA_Close = Indicators.SimpleMovingAverage(MarketSeries.Close, 1);
            i_Parabolic_SAR = Indicators.ParabolicSAR(Step_PrbSAR, 0.1);
            i_MA_Open = Indicators.SimpleMovingAverage(MarketSeries.Open, 1);
            i_EMAf = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period_FastEMA);
        }

        public override void Calculate(int index)
        {
            double spread = Math.Round(Symbol.Spread / Symbol.PipSize, 5);

            int signal = Neutral;

            _MACD_Histogram = i_MACD_Histogram.Histogram.Last(0);
            _MCAD_signal = i_MCAD_signal.Signal.Last(0);
            //	_MA_Close = i_MA_Close.Result.Last(0);
            _Parabolic_SAR = i_Parabolic_SAR.Result.Last(0);

            _isMacdMainPositive = (_MACD_Histogram > 0);
            _isMacdMainAboveMacdSignal = (_MACD_Histogram > _MCAD_signal);
            _isMacdSignalPositive = (_MCAD_signal > 0);
            _isParabolicSARBelowMaClose = (_Parabolic_SAR < MarketSeries.Close.LastValue);

            if (_isMacdMainAboveMacdSignal && _isMacdMainPositive && _isParabolicSARBelowMaClose && (Math.Abs(_MACD_Histogram - _MCAD_signal) > (Noise_MACD_sm / _dixPowerDigits)) && (Math.Abs(_MACD_Histogram) > (Noise_MACD_m0 / _dixPowerDigits)) && (Math.Abs(_Parabolic_SAR - i_EMAf.Result.Last(0)) > (Noise_Prb_SAR_ema / _dixPowerDigits)) && (Math.Abs(_MCAD_signal) > (Noise_MACD_s0 / _dixPowerDigits)) && _isMacdSignalPositive)
            {
                signal = Up;
            }

            else if (!_isMacdMainAboveMacdSignal && !_isMacdMainPositive && !_isParabolicSARBelowMaClose && (Math.Abs(_MACD_Histogram - _MCAD_signal) > (Noise_MACD_sm / _dixPowerDigits)) && (Math.Abs(_MACD_Histogram) > (Noise_MACD_m0 / _dixPowerDigits)) && (Math.Abs((_Parabolic_SAR - i_EMAf.Result.Last(0))) > (Noise_Prb_SAR_ema / _dixPowerDigits)) && (Math.Abs(_MCAD_signal) > (Noise_MACD_s0 / _dixPowerDigits)) && !_isMacdSignalPositive)
            {
                signal = Dn;
            }

            Result[index] = signal;

            Histogram[index] = i_MACD_Histogram.Histogram[index];
            Signal[index] = i_MACD_Histogram.Signal[index];
            ParabolicSAR[index] = i_Parabolic_SAR.Result[index];

            string text = string.Format("Buy : spread={0}", spread);
            ChartObjects.DrawText("Trade", "\t" + text, StaticPosition.TopLeft, Colors.Black);
        }



    }
}

