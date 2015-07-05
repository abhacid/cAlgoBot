//+------------------------------------------------------------------+
//+                          Code generated using FxPro Quant 2.0.12 |
//+------------------------------------------------------------------+

using System;
using System.Reflection;
using System.Threading;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.API.Requests;
using cAlgo.Indicators;
using cAlgo.Lib;
using FxProQuant.Lib;


namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC)]
    public class MACDPrbSARnoise : Robot
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
        public long Volume { get; set; }

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

		private Position _position;

        protected override void OnStart()
        {
			_botName = ToString();
			_instanceLabel = string.Format("{0}-{1}-{2}-{3}", _botName, _botVersion, Symbol.Code, TimeFrame.ToString());
			_position = null;

            i_MACD_main = Indicators.MacdHistogram(MarketSeries.Close, Period_SlowEMA, Period_FastEMA, Period_MACD_SMA);
            i_MCAD_signal = Indicators.MacdHistogram(MarketSeries.Close, Period_SlowEMA, Period_FastEMA, Period_MACD_SMA);
            i_MA_Close = Indicators.SimpleMovingAverage(MarketSeries.Close, 1);
            i_Parabolic_SAR = Indicators.ParabolicSAR(Step_PrbSAR, 0.1);
            i_MA_Open = Indicators.SimpleMovingAverage(MarketSeries.Open, 1);
            i_EMAf = Indicators.ExponentialMovingAverage(MarketSeries.Close, (int)Period_FastEMA);
        }

        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;

            TriState _Open_Buy = new TriState();
            TriState _Open_Sell = new TriState();
            TriState _Close_Sell = new TriState();
            TriState _Close_Buy = new TriState();

            _MACD_main = i_MACD_main.Histogram.Last(0);
            _MCAD_signal = i_MCAD_signal.Signal.Last(0);
            _MA_Close = i_MA_Close.Result.Last(0);
            _Parabolic_SAR = i_Parabolic_SAR.Result.Last(0);

            _isMacdMainPositive = (_MACD_main > 0);
            _isMacdMainAboveMacdSignal = (_MACD_main > _MCAD_signal);
            _isMacdSignalPositive = (_MCAD_signal > 0);
            _isParabolicSARBelowMaClose = (_Parabolic_SAR < _MA_Close);

			TradeType? tradeType = signal();
			if(tradeType.HasValue)
				openPosition(tradeType, Volume, 0, Stop_Loss, null, "");
			
			if(_position!=null && _position.NetProfit > Math.Abs(_position.Commissions + _position.Swap))
			{
				if(_position.TradeType==TradeType.Sell && _isMacdMainAboveMacdSignal && _isParabolicSARBelowMaClose)
					closePosition();
				else
					if (_position.TradeType==TradeType.Buy && !_isMacdMainAboveMacdSignal && !_isParabolicSARBelowMaClose)
							closePosition();
			}
        }

		private TradeType? signal()
		{
			TradeType? tradeType=null;

			if((_isMacdMainAboveMacdSignal &&
				_isMacdMainPositive &&
				_isParabolicSARBelowMaClose &&
				(Math.Abs(_MACD_main - _MCAD_signal) > (Noise_MACD_sm / (Math.Pow(10, Symbol.Digits)))) &&
				(Math.Abs(_MACD_main) > (Noise_MACD_m0 / (Math.Pow(10, Symbol.Digits)))) &&
				(Math.Abs(_Parabolic_SAR - i_EMAf.Result.Last(0)) > (Noise_Prb_SAR_ema / (Math.Pow(10, Symbol.Digits)))) &&
				(Math.Abs(_MCAD_signal) > (Noise_MACD_s0 / (Math.Pow(10, Symbol.Digits)))) &&
				_isMacdSignalPositive))
					
					tradeType = TradeType.Buy;
			else
				if((!_isMacdMainAboveMacdSignal &&
					  !_isMacdMainPositive &&
					  !_isParabolicSARBelowMaClose &&
					  (Math.Abs(_MACD_main - _MCAD_signal) > (Noise_MACD_sm / (Math.Pow(10, Symbol.Digits)))) &&
					  (Math.Abs(_MACD_main) > (Noise_MACD_m0 / (Math.Pow(10, Symbol.Digits)))) &&
					  (Math.Abs((_Parabolic_SAR - i_EMAf.Result.Last(0))) > (Noise_Prb_SAR_ema / (Math.Pow(10, Symbol.Digits)))) &&
					  (Math.Abs(_MCAD_signal) > (Noise_MACD_s0 / (Math.Pow(10, Symbol.Digits)))) &&
					  !_isMacdSignalPositive))
						
						tradeType = TradeType.Sell;

			return tradeType;
		}

        private TradeResult openPosition(TradeType? tradeType, long volume, double slippage, double? stopLoss, double? takeProfit, string comment)
        {
			if(!(tradeType.HasValue) || _position != null)
				return null;

			TradeResult tradeResult=ExecuteMarketOrder(tradeType.Value, Symbol, volume, _instanceLabel, stopLoss, takeProfit, slippage, comment);

            if (!(tradeResult.IsSuccessful))
                Thread.Sleep(400);
			else
				_position = tradeResult.Position;

            return tradeResult;
        }

		private TradeResult closePosition()
        {
			if(_position == null)
				return null;
			
            TradeResult tradeResult = ClosePosition(_position);

			if(!(tradeResult.IsSuccessful))
				Thread.Sleep(400);
			else
				_position = null;

            return tradeResult;
        }

      
    }
}


