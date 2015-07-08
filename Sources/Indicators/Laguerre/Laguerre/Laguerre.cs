using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator("Laguerre")]
    public class Laguerre : Indicator
    {
        private const double gamma = 0.7;
        private double _cdown;
        private double _cup;
        private double _lag0;
        private double _lag0Prev;
        private double _lag1;
        private double _lag1Prev;
        private double _lag2;
        private double _lag2Prev;
        private double _lag3;
        private double _lag3Prev;
        private double _laguerreRsi;

        [Output("Signal", PlotType = PlotType.Line, Color = Colors.Magenta)]
        public IndicatorDataSeries Result { get; set; }

        public override void Calculate(int index)
        {
            _lag0Prev = _lag0;
            _lag1Prev = _lag1;
            _lag2Prev = _lag2;
            _lag3Prev = _lag3;

            _lag0 = (1 - gamma)*MarketSeries.Close[index] + gamma*_lag0Prev;
            _lag1 = -gamma * _lag0 + _lag0Prev + gamma * _lag1Prev;
            _lag2 = -gamma * _lag1 + _lag1Prev + gamma * _lag2Prev;
            _lag3 = -gamma*_lag2 + _lag2Prev + gamma*_lag3Prev;

            _cup = 0;
            _cdown = 0;

            if (_lag0 >= _lag1) 
                _cup = _lag0 - _lag1;
            else 
                _cdown = _lag1 - _lag0;
            if (_lag1 >= _lag2)
                _cup = _cup + _lag1 - _lag2;
            else 
                _cdown = _cdown + _lag2 - _lag1;
            if (_lag2 >= _lag3) 
                _cup = _cup + _lag2 - _lag3;
            else 
                _cdown = _cdown + _lag3 - _lag2;

            double epsilon = 0.1*Math.Pow(10,-10);

            if (Math.Abs(_cup + _cdown - 0) > epsilon)
                _laguerreRsi = _cup/(_cup + _cdown);

            Result[index] = _laguerreRsi;
        }
    }
}