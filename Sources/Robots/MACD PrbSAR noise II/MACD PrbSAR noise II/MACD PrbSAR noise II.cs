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

#region cBot Infos
// -------------------------------------------------------------------------------
//	Code based on generator using FxPro Quant 2.0.12
//	author egormas Masakou Yahor, Belarus, http://ctdn.com/algos/cbots/show/714
//	Modified by Abdallah Hacid, France.
//
//  Advisor "MCAD PrbSAR noise" trades on signals indicators MACD and Parabolic SAR, 
//  default is optimized on a pair EURUSD H1. You can optimize any instrument
//	and timeframe. Additionally Advisor equipped noise levels on indicators MACD and
//	Parabolic SAR. Closing of open positions occurs on signals indicators MACD 
//	and Parabolic SAR only in profits, or - the stop-loss.
// -------------------------------------------------------------------------------
#endregion

using System;
using System.Reflection;
using System.Threading;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;


namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC)]
    public class MACDPrbSARnoiseII : Robot
    {

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

        [Parameter("Volume", DefaultValue = 10000)]
        public int Volume { get; set; }

        [Parameter("Stop_Loss", DefaultValue = 300)]
        public int Stop_Loss { get; set; }

        [Parameter("Period_SlowEMA", DefaultValue = 26)]
        public int Period_SlowEMA { get; set; }

        [Parameter("Period_FastEMA", DefaultValue = 12)]
        public int Period_FastEMA { get; set; }

        //Global declaration
        private MacdHistogram i_MACD_main;
        private MacdHistogram i_MCAD_signal;
        private SimpleMovingAverage i_MA_Close;
        private ParabolicSAR i_Parabolic_SAR;
        private SimpleMovingAverage i_MA_Open;
        private ExponentialMovingAverage i_EMAf;

        double _MACD_main;
        double _MCAD_signal;
        double _MA_Close;
        double _Parabolic_SAR;

        bool _isMacdMainPositive;
        bool _isMacdMainAboveMacdSignal;
        bool _isMacdSignalPositive;
        bool _isParabolicSARBelowMaClose;

        DateTime LastTradeExecution = new DateTime(0);

        private string _botName;
        private string _botVersion = Assembly.GetExecutingAssembly().FullName.Split(',')[1].Replace("Version=", "").Trim();

        // le label permet de s'y retrouver parmis toutes les instances possibles.
        private string _instanceLabel;
		private double _dixPowerDigits;
        private Position _position;

        protected override void OnStart()
        {
            _botName = ToString();
            _instanceLabel = string.Format("{0}-{1}-{2}-{3}", _botName, _botVersion, Symbol.Code, TimeFrame.ToString());
			_dixPowerDigits = Math.Pow(10, Symbol.Digits);
            _position = null;

            i_MACD_main = Indicators.MacdHistogram(MarketSeries.Close, Period_SlowEMA, Period_FastEMA, Period_MACD_SMA);
            i_MCAD_signal = Indicators.MacdHistogram(MarketSeries.Close, Period_SlowEMA, Period_FastEMA, Period_MACD_SMA);
            i_MA_Close = Indicators.SimpleMovingAverage(MarketSeries.Close, 1);
            i_Parabolic_SAR = Indicators.ParabolicSAR(Step_PrbSAR, 0.1);
            i_MA_Open = Indicators.SimpleMovingAverage(MarketSeries.Open, 1);
            i_EMAf = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period_FastEMA);
        }

        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;

            _MACD_main = i_MACD_main.Histogram.Last(0);
            _MCAD_signal = i_MCAD_signal.Signal.Last(0);
            _MA_Close = i_MA_Close.Result.Last(0);
            _Parabolic_SAR = i_Parabolic_SAR.Result.Last(0);

            _isMacdMainPositive = (_MACD_main > 0);
            _isMacdMainAboveMacdSignal = (_MACD_main > _MCAD_signal);
            _isMacdSignalPositive = (_MCAD_signal > 0);
            _isParabolicSARBelowMaClose = (_Parabolic_SAR < _MA_Close);

            TradeType? tradeType = signal();
            if (tradeType.HasValue)
                openPosition(tradeType, Volume, 0, Stop_Loss, null, "");

            if (_position != null)// && _position.NetProfit > Math.Abs(_position.Commissions + _position.Swap))
            {
                if (_position.TradeType == TradeType.Sell && _isMacdMainAboveMacdSignal && _isParabolicSARBelowMaClose)
                    closePosition();
                else if (_position.TradeType == TradeType.Buy && !_isMacdMainAboveMacdSignal && !_isParabolicSARBelowMaClose)
                    closePosition();
            }
        }

        private TradeType? signal()
        {
            TradeType? tradeType = null;

			if(_isMacdMainAboveMacdSignal && 
				_isMacdMainPositive && 
				_isParabolicSARBelowMaClose && 
				(Math.Abs(_MACD_main - _MCAD_signal) > (Noise_MACD_sm / _dixPowerDigits)) && 
				(Math.Abs(_MACD_main) > (Noise_MACD_m0 / _dixPowerDigits)) && 
				(Math.Abs(_Parabolic_SAR - i_EMAf.Result.Last(0)) > (Noise_Prb_SAR_ema / _dixPowerDigits)) && 
				(Math.Abs(_MCAD_signal) > (Noise_MACD_s0 / _dixPowerDigits)) && 
				_isMacdSignalPositive)
					tradeType = TradeType.Buy;
			else if(!_isMacdMainAboveMacdSignal && 
					!_isMacdMainPositive && 
					!_isParabolicSARBelowMaClose && 
					(Math.Abs(_MACD_main - _MCAD_signal) > (Noise_MACD_sm / _dixPowerDigits)) && 
					(Math.Abs(_MACD_main) > (Noise_MACD_m0 / _dixPowerDigits)) && 
					(Math.Abs((_Parabolic_SAR - i_EMAf.Result.Last(0))) > (Noise_Prb_SAR_ema / _dixPowerDigits)) && 
					(Math.Abs(_MCAD_signal) > (Noise_MACD_s0 / _dixPowerDigits)) && 
					!_isMacdSignalPositive)
						tradeType = TradeType.Sell;

            return tradeType;
        }

        private TradeResult openPosition(TradeType? tradeType, long volume, double slippage, double? stopLoss, double? takeProfit, string comment)
        {
            if (!(tradeType.HasValue) || _position != null)
                return null;

            TradeResult tradeResult = ExecuteMarketOrder(tradeType.Value, Symbol, volume, _instanceLabel, stopLoss, takeProfit, slippage, comment);

            if (!(tradeResult.IsSuccessful))
                Thread.Sleep(400);
            else
                _position = tradeResult.Position;

            return tradeResult;
        }

        private TradeResult closePosition()
        {
            if (_position == null)
                return null;

            TradeResult tradeResult = ClosePosition(_position);

            if (!(tradeResult.IsSuccessful))
                Thread.Sleep(400);
            else
                _position = null;

            return tradeResult;
        }


    }
}


