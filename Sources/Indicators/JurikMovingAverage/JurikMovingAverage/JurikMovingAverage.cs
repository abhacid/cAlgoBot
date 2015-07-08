using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class JurikMovingAverage : Indicator
    {
        #region variables

        private readonly double[] _list = new double[128];
        private readonly double[] _ring1 = new double[128];
        private readonly double[] _ring2 = new double[11];
        private readonly double[] _buffer = new double[62];

        private IndicatorDataSeries _jmaSeries;
        private IndicatorDataSeries _sourceSeries;
        private IndicatorDataSeries _series1;
        private IndicatorDataSeries _series2;
        private int _limitValue;
        private int _startValue;
        private int _loopIndex;
        private int _loopCriteria;
        
        private bool _initFlag;
        private double _phaseParam;
        private double _logParam;
        private double _sqrtParam;
        private double _lengthDivider;
        private double _lowDValue;
        private int _counterA;
        private double _dValue;
        private double _jmaValue;
        private int _counterB;
        private int _highLimit;
        private int _cycleLimit;
        private double _cycleDelta;

        #endregion

        #region Input

        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 10, MinValue = 1)]
        public int Length { get; set; }

        [Parameter(DefaultValue = 0, MinValue = -100, MaxValue = 100)]
        public double Phase { get; set; }

        #endregion

        #region output

        [Output("JMA", Color = Colors.Purple)]
        public IndicatorDataSeries Result { get; set; }

        #endregion

        protected override void Initialize()
        {
            _jmaSeries = CreateDataSeries();
            _sourceSeries = CreateDataSeries();
            _series1 = CreateDataSeries();
            _series2 = CreateDataSeries();

            _limitValue = _list.Length / 2 - 1;
            _startValue = _limitValue + 1;

            for (int i = 0; i <= _limitValue; i++)
                _list[i] = -1000000;
            for (int i = _startValue; i < _list.Length; i++)
                _list[i] = 1000000;

            _initFlag = true;

            double lengthParam = (Length <= 1) ? 0.0000000001 : (Length - 1) / 2.0;

            _phaseParam = Phase / 100.0 + 1.5;

            _logParam = Math.Log(Math.Sqrt(lengthParam)) / Math.Log(2.0);
            if (_logParam + 2.0 < 0)
                _logParam = 0;
            else _logParam += 2.0;

            _sqrtParam = Math.Sqrt(lengthParam) * _logParam;
            lengthParam *= 0.9;
            _lengthDivider = lengthParam / (lengthParam + 2.0);

        }

        public override void Calculate(int index)
        {

            double jmaTempValue = index == 0 ? Source[index] : _jmaSeries[index - 1];

            if (index < 1)
            {
                _jmaValue = Source[index];               
                _sourceSeries[index] = 0;
                _series1[index] = 0;
                _series2[index] = 0;

                return;
            }

            if (_loopIndex < 61)
            {
                _loopIndex++;
                _buffer[_loopIndex] = Source[index];
            }

            double paramA = 0;
            double paramB = 0;

            if (_loopIndex > 30)
            {
                if (_initFlag)
                {

                    _initFlag = false;
                    var diffFlag = 0;

                    for (int i = 1; i <= 29; i++)
                    {
                        if ((int)_buffer[i + 1] != (int)_buffer[i])
                        {
                            diffFlag = 1;
                            break;
                        }
                        
                    }

                    _highLimit = diffFlag * 30;

                    paramB = (_highLimit == 0) ? Source[index] : _buffer[index - 1];
                    paramA = paramB;

                    if (_highLimit > 29)
                    {
                        _highLimit = 29;
                    }
                }
                else
                {
                    _highLimit = 0;
                }

                for (int i = _highLimit; i >= 0; i--)
                {
                    double sValue = (i == 0) ? Source[index] : _buffer[ 31 + i];
                    double absValue = (Math.Abs(sValue - paramA) > Math.Abs(sValue - paramB))
                                          ? Math.Abs(sValue - paramA)
                                          : Math.Abs(sValue - paramB);

                    _dValue = absValue + 0.0000000001; 
                                        
                    if (_counterA <= 1)
                        _counterA = 127;
                    else
                        _counterA--; 

                    if (_counterB <= 1)
                        _counterB = 10;
                    else
                        _counterB--; 
                    
                    if (_cycleLimit < 128) 
                        _cycleLimit++;

                    _cycleDelta += (_dValue - _ring2[_counterB]);
                    
                    _ring2[_counterB] = _dValue;

                    double highDValue = (_cycleLimit > 10) ? _cycleDelta / 10.0 : _cycleDelta / _cycleLimit;
                    
                    int s68;
                    int s58;
                    int s38 = 0;
                    int s40 = 0;

                    if (_cycleLimit > 127)
                    {
                        _dValue = _ring1[_counterA];                        
                        _ring1[_counterA] = highDValue;
                        
                        s68 = 64;
                        s58 = s68;

                        while (s68 > 1)
                        {
                            if (_list[s58] < _dValue)
                            {
                                s68 /= 2;
                                s58 += s68;
                            }
                            else if (_list[s58] <= _dValue)
                                s68 = 1;
                            else
                            {
                                s68 /= 2;
                                s58 -= s68;
                            }
                        }
                    }
                    else
                    {
                        _ring1[_counterA] = highDValue;
                        
                        if ((_limitValue + _startValue) > 127)
                        {
                            _startValue--;
                            s58 = _startValue;
                        }
                        else
                        {
                            _limitValue++;
                            s58 = _limitValue;
                        }
                        s38 = (_limitValue > 96) ? 96 : _limitValue;
                        s40 = (_startValue < 32) ? 32 : _startValue;
                    }
                    
                    s68 = 64;
                    int s60 = s68;

                    while (s68 > 1)
                    {
                        if (_list[s60] >= highDValue)
                        {
                            if (_list[s60 - 1] <= highDValue) 
                                s68 = 1;
                            else
                            {
                                s68 /= 2;
                                s60 -= s68;
                            }
                        }
                        else
                        {
                            s68 /= 2;
                            s60 += s68;
                        }
                        if ((s60 == 127) && (highDValue > _list[127])) 
                            s60 = 128;
                    }

                    if (_cycleLimit > 127)
                    {
                        if (s58 >= s60)
                        {
                            if (((s38 + 1) > s60) && ((s40 - 1) < s60))
                                _lowDValue += highDValue;
                            else if ((s40 > s60) && ((s40 - 1) < s58))
                                _lowDValue += _list[s40 - 1];                        
                            
                        }
                        else if (s40 >= s60)
                        {
                            if (((s38 + 1) < s60) && ((s38 + 1) > s58))
                                _lowDValue += _list[s38 + 1];
                        }
                        else if ((s38 + 2) > s60)
                            _lowDValue += highDValue;
                        else if (((s38 + 1) < s60) && ((s38 + 1) > s58))
                            _lowDValue += _list[s38 + 1];
                        
                        
                        
                        if (s58 > s60)
                        {
                            if (((s40 - 1) < s58) && ((s38 + 1) > s58))
                                _lowDValue -= _list[s58];
                            else if ((s38 < s58) && ((s38 + 1) > s60))
                                _lowDValue -= _list[s38];
                        }
                        else
                        {
                            if (((s38 + 1) > s58) && ((s40 - 1) < s58))
                                _lowDValue -= _list[s58];
                            else if ((s40 > s58) && (s40 < s60))
                                _lowDValue -= _list[s40];
                        }
                    }

                    if (s58 <= s60)
                    {
                        if (s58 >= s60) 
                            _list[s60] = highDValue;
                        else
                        {
                            for (int j = s58 + 1; j <= (s60 - 1); j++) 
                                _list[j - 1] = _list[j];
                            
                            _list[s60 - 1] = highDValue;
                        }
                    }
                    else
                    {
                        for (int j = s58 - 1; j >= s60; j--) 
                            _list[j + 1] = _list[j];
                        
                        _list[s60] = highDValue;
                    }

                    if (_cycleLimit <= 127)
                    {
                        _lowDValue = 0;
                        for (int j = s40; j <= s38; j++) 
                            _lowDValue += _list[j];
                    }


                    if ((_loopCriteria) > 31) 
                        _loopCriteria = 31;
                    else 
                        _loopCriteria++;

                    double sqrtDivider = _sqrtParam / (_sqrtParam + 1.0);

                    if (_loopCriteria <= 30)
                    {
                        paramA = (sValue - paramA > 0) ? sValue : sValue - (sValue - paramA) * sqrtDivider;
                        paramB = (sValue - paramB < 0) ? sValue : sValue - (sValue - paramB) * sqrtDivider;
                        jmaTempValue = Source[index];

                        if (_loopCriteria == 30)
                        {
                            _sourceSeries[index] = Source[index];
                            int intPart = (Math.Ceiling(_sqrtParam) >= 1.0) ? (int)Math.Ceiling(_sqrtParam) : 1;
                            int leftInt = intPart;
                            intPart = (Math.Floor(_sqrtParam) >= 1.0) ? (int)Math.Floor(_sqrtParam) : 1;
                            int rightPart = intPart;

                            _dValue = (leftInt == rightPart)
                                          ? 1.0
                                          : (_sqrtParam - rightPart) / (leftInt - rightPart);
                            int upShift = (rightPart <= 29) ? rightPart : 29;
                            int dnShift = (leftInt <= 29) ? leftInt : 29;
                            _series1[index] = (Source[index] - _buffer[_loopIndex - upShift]) * (1 - _dValue) /
                                              rightPart +
                                              (Source[index] - _buffer[_loopIndex - dnShift]) * _dValue / leftInt;
                            
                        }
                                                
                    }
                    else
                    {
                        _dValue = _lowDValue / (s38 - s40 + 1);
                        double powerValue = (0.5 <= _logParam - 2.0) ? _logParam - 2.0 : 0.5;
                        _dValue = _logParam >= Math.Pow(absValue / _dValue, powerValue)
                                      ? Math.Pow(absValue / _dValue, powerValue)
                                      : _logParam;
                        if (_dValue < 1)
                            _dValue = 1;
                        powerValue = Math.Pow(sqrtDivider, Math.Sqrt(_dValue));
                        paramA = (sValue - paramA > 0) ? sValue : sValue - (sValue - paramA) * powerValue;
                        paramB = (sValue - paramB < 0) ? sValue : sValue - (sValue - paramB) * powerValue;
                    }
                    
                }

                if (_loopCriteria > 30)
                {
                    double powerValue = Math.Pow(_lengthDivider, _dValue);
                    double squareValue = powerValue * powerValue;

                    if(double.IsNaN(_series1[index-1]))
                        _series1[index-1] = 0;
                    if(double.IsNaN(_series2[index-1]))
                        _series2[index-1] = 0;

                    _sourceSeries[index] = (1 - powerValue) * Source[index] + powerValue * _sourceSeries[index - 1];
                    
                    _series2[index] = (Source[index] - _sourceSeries[index]) * (1.0 - _lengthDivider) +
                                      _lengthDivider * _series2[index - 1];
                    
                    _series1[index] = (_phaseParam * _series2[index] + _sourceSeries[index] - jmaTempValue) *
                                      (powerValue * (-2.0) + squareValue + 1) + squareValue * _series1[index - 1];
                    jmaTempValue += _series1[index];
                    
                }
            }

            _jmaValue = (_loopIndex < 30)? Source[index] : jmaTempValue; 
            
            _jmaSeries[index] = _jmaValue;

            Result[index] = _jmaValue;

            

        }
    }
}
