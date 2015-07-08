using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    internal class CycleIdentifierAL : Indicator
    {        
        private const int RangeLength = 250;

        #region input parameters

        [Parameter("Use Cycle Filter", DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int UseCycleFilter { get; set; }

        [Parameter("RSI/MA Filter", DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int UseRsiFilter { get; set; }
        
        [Parameter("MA Filter Strength", DefaultValue = 12)]
        public int SmaFilter { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Parameter("RSI Filter Strength", DefaultValue = 21)]
        public int RsiFilter { get; set; }

        [Parameter("Length", DefaultValue = 3)]
        public int Length { get; set; }

        [Parameter("Price Action Filter", DefaultValue = 1, MinValue = 1)]
        public int PriceActionFilter { get; set; }

        [Parameter("Major Cycle Strength", DefaultValue = 4)]
        public int MajorCycleStrength { get; set; }

        #endregion

        #region Output

        [Output("Line", Color = Colors.Gray)]
        public IndicatorDataSeries Line { get; set; }

        [Output("Major Buy Cycle", PlotType = PlotType.Histogram, Color = Colors.Lime, Thickness = 3)]
        public IndicatorDataSeries MajorBuy { get; set; }

        [Output("Major Sell Cycle", PlotType = PlotType.Histogram, Color = Colors.Red, Thickness = 3)]
        public IndicatorDataSeries MajorSell { get; set; }

        [Output("Minor Buy Cycle", PlotType = PlotType.Histogram, Color = Colors.DarkGreen)]
        public IndicatorDataSeries MinorBuy { get; set; }

        [Output("Minor Sell Cycle", PlotType = PlotType.Histogram, Color = Colors.Brown)]
        public IndicatorDataSeries MinorSell { get; set; }

        [Output("False Signal", PlotType = PlotType.Histogram, Color = Colors.Fuchsia)]
        public IndicatorDataSeries FalseSignal { get; set; }

        [Output("DotsBuy", PlotType = PlotType.Points, Color = Colors.Green, Thickness = 5)]
        public IndicatorDataSeries DotsBuy { get; set; }

        [Output("DotsSell", PlotType = PlotType.Points, Color = Colors.Red, Thickness = 5)]
        public IndicatorDataSeries DotsSell { get; set; }

        #endregion

        #region private fields

        private IndicatorDataSeries _cyclePrice;
        private bool _flatMajorBuy, _flatMajorSell;
        private bool _flatMinorBuy, _flatMinorSell;

        private int _indexDiffMajor,
                    _indexDiffMinor;

        //private int _isLong;
        //private int _isShort;
        private MovingAverage _ma;

        private double _previousCyclePriceMajorBuy;

        private double _previousCyclePriceMajorSell;

        private double _previousCyclePriceMinorBuy;
        private double _previousCyclePriceMinorSell;

        private int _previousIndexMajorBuy = 1;

        private int _previousIndexMajorSell;

        private int _previousIndexMinorBuy = 1;
        private int _previousIndexMinorSell;
        private RelativeStrengthIndex _rsi;

        private bool _runInit = true;

        private int _switchMajor,
                    _switchMinor;

        private bool _useCycleFilter;
        private IndicatorDataSeries _zeroLag;

        #endregion

        protected override void Initialize()
        {
            _cyclePrice = CreateDataSeries();
            _zeroLag = CreateDataSeries();
            _ma = Indicators.MovingAverage(MarketSeries.Close, PriceActionFilter, MaType);
            _rsi = Indicators.RelativeStrengthIndex(_cyclePrice, 14);
            _useCycleFilter = UseCycleFilter == 1;
        }

        public override void Calculate(int index)
        {
            if (index < RangeLength)
                return;

            double sRange = 0;
            for (int i = 0; i < RangeLength; i++)
                sRange += (MarketSeries.High[i] - MarketSeries.Low[i]);


            double range = sRange/RangeLength*Length;
            double sweep = range*MajorCycleStrength;

            _cyclePrice[index] = PriceActionFilter == 1
                                     ? MarketSeries.Close[index]
                                     : _ma.Result[index];            

            if (_runInit)
            {
                _previousCyclePriceMinorSell = _previousCyclePriceMinorBuy = _cyclePrice[index];
                _previousCyclePriceMajorSell = _previousCyclePriceMajorBuy = _cyclePrice[index];
                Line[index] = 0;
                MinorBuy[0] = 0;
                MinorSell[0] = 0;
                MajorBuy[0] = 0;
                MajorSell[0] = 0;
                _zeroLag[RangeLength - 1] = 0;
                _zeroLag[RangeLength - 2] = 0;
                _runInit = false;
            }

            _zeroLag[index] = UseRsiFilter == 0
                                              ? ZeroLag(_cyclePrice[index], SmaFilter, index)
                                              : ZeroLag(_rsi.Result[index], RsiFilter, index);


            #region Minor Cycle

            if (_switchMinor > -1)
            {
                SetMinorBuy(index, range);
            }
            if (_switchMinor < 1)
            {
                SetMinorSell(index, range);
            }

            #endregion

            #region Major Cycle

            if (_switchMajor > -1)
            {
                SetMajorBuy(index, sweep);
            }
            if (_switchMajor < 1)
            {
                SetMajorSell(index, sweep);
            }

            #endregion

            if (double.IsNaN(DotsBuy[index]) && double.IsNaN(DotsSell[index]))
            {
                if (!double.IsNaN(DotsBuy[index - 1]))
                {
                    DotsBuy[index] = DotsBuy[index - 1];
                }
                if (!double.IsNaN(DotsSell[index - 1]))
                {
                    DotsSell[index] = DotsSell[index - 1];
                }
            }
        }

        private double ZeroLag(double price, int length, int index)
        {
            if (length < 3)
                return price;

            double a = Math.Exp(-1.414*3.14159/length);
            double b = 2*a*Math.Cos(1.414*180/length);
            double c = 1 - b + a*a;

            return c*price + b*_zeroLag[index - 1] - a*a*_zeroLag[index - 2];
        }

        private void SetMinorBuy(int index, double range)
        {
            if (_cyclePrice[index] < _previousCyclePriceMinorBuy)
            {
                if (_flatMinorBuy && (_zeroLag[index] < _zeroLag[index - 1] || !_useCycleFilter))
                {
                    FalseSignal[_previousIndexMinorBuy] = -1;
                    MinorBuy[_previousIndexMinorBuy] = 0;
                    Line[_previousIndexMinorBuy] = 0;

                    DotsBuy[_previousIndexMinorBuy] = 0;
                    DotsSell[_previousIndexMinorBuy] = double.NaN;
                }

                _previousCyclePriceMinorBuy = _cyclePrice[index];
                _previousIndexMinorBuy = index;
                _flatMinorBuy = true;
            }
            else if (_cyclePrice[index] > _previousCyclePriceMinorBuy)
            {
                if (_zeroLag[index] > _zeroLag[index - 1] || !_useCycleFilter)
                {
                    MinorBuy[_previousIndexMinorBuy] = -1;
                    Line[_previousIndexMinorBuy] = -1;
                    Line[index] = 0;

                    DotsBuy[_previousIndexMinorBuy] = 0;
                    DotsSell[_previousIndexMinorBuy] = double.NaN;
                }

                _flatMinorBuy = true;

                double cyclePrice1 = UseRsiFilter == 0
                                         ? _ma.Result[_previousIndexMinorBuy]
                                         : _rsi.Result[_previousIndexMinorBuy];

                _indexDiffMinor = index - _previousIndexMinorBuy;

                if (_cyclePrice[index] - cyclePrice1 >= range && _indexDiffMinor >= 1)
                {
                    _switchMinor = -1;
                    _previousCyclePriceMinorSell = _cyclePrice[index];
                    _previousIndexMinorSell = index;
                    _flatMinorSell = false;
                    _flatMinorBuy = false;
                }
            }
        }

        private void SetMinorSell(int index, double range)
        {
            if (_cyclePrice[index] > _previousCyclePriceMinorSell)
            {
                if (_flatMinorSell && (_zeroLag[index] > _zeroLag[index - 1] || !_useCycleFilter))
                {
                    FalseSignal[_previousIndexMinorSell] = 1;
                    MinorSell[_previousIndexMinorSell] = 0;
                    Line[_previousIndexMinorSell] = 0;

                    DotsSell[_previousIndexMinorSell] = 0;
                    DotsBuy[_previousIndexMinorSell] = double.NaN;
                }
                _previousCyclePriceMinorSell = _cyclePrice[index];
                _previousIndexMinorSell = index;
                _flatMinorSell = true;
            }
            else if (_cyclePrice[index] < _previousCyclePriceMinorSell)
            {
                if (_zeroLag[index] < _zeroLag[index - 1] || !_useCycleFilter)
                {
                    MinorSell[_previousIndexMinorSell] = 1;
                    Line[_previousIndexMinorSell] = 1;
                    Line[index] = 0;

                    DotsSell[_previousIndexMinorSell] = 0;
                    DotsBuy[_previousIndexMinorSell] = double.NaN;
                }

                _flatMinorSell = true;

                _indexDiffMinor = index - _previousIndexMinorSell;

                double cyclePrice2 = UseRsiFilter == 0
                                         ? _ma.Result[_previousIndexMinorSell]
                                         : _rsi.Result[_previousIndexMinorSell];

                if (cyclePrice2 - _cyclePrice[index] >= range && _indexDiffMinor >= 1)
                {
                    _switchMinor = 1;
                    _previousCyclePriceMinorBuy = _cyclePrice[index];
                    _previousIndexMinorBuy = index;
                    _flatMinorSell = false;
                    _flatMinorBuy = false;
                }
            }
        }

        private void SetMajorBuy(int index, double sweep)
        {
            if (_cyclePrice[index] < _previousCyclePriceMajorBuy)
            {
                if (_flatMajorBuy && (_zeroLag[index] < _zeroLag[index - 1] || !_useCycleFilter))
                {
                    FalseSignal[_previousIndexMajorBuy] = -1;
                    MajorBuy[_previousIndexMajorBuy] = 0;
                    Line[_previousIndexMajorBuy] = 0;

                    DotsBuy[_previousIndexMajorBuy] = 0;
                    DotsSell[_previousIndexMajorBuy] = double.NaN;
                }
                _previousCyclePriceMajorBuy = _cyclePrice[index];
                _previousIndexMajorBuy = index;
                _flatMajorBuy = true;
            }
            else if (_cyclePrice[index] > _previousCyclePriceMajorBuy)
            {
                if (_zeroLag[index] > _zeroLag[index - 1] || !_useCycleFilter)
                {
                    MajorBuy[_previousIndexMajorBuy] = -1;
                    Line[_previousIndexMajorBuy] = -1;
                    Line[index] = 0;

                    DotsBuy[_previousIndexMajorBuy] = 0;
                    DotsSell[_previousIndexMajorBuy] = double.NaN;
                }

                _flatMajorBuy = true;

                _indexDiffMajor = index - _previousIndexMajorBuy;

                double cyclePrice3 = UseRsiFilter == 0
                                         ? _ma.Result[_previousIndexMajorBuy]
                                         : _rsi.Result[_previousIndexMajorBuy];

                if (_cyclePrice[index] - cyclePrice3 >= sweep && _indexDiffMajor >= 1)
                {
                    _switchMajor = -1;
                    _previousCyclePriceMajorSell = _cyclePrice[index];
                    _previousIndexMajorSell = index;
                    _flatMajorSell = false;
                    _flatMajorBuy = false;
                }
            }
        }

        private void SetMajorSell(int index, double sweep)
        {
            if (_cyclePrice[index] > _previousCyclePriceMajorSell)
            {
                if (_flatMajorSell && (_zeroLag[index] > _zeroLag[index - 1] || !_useCycleFilter))
                {
                    FalseSignal[_previousIndexMajorSell] = 1;
                    MajorSell[_previousIndexMajorSell] = 0;
                    Line[_previousIndexMajorSell] = 0;

                    DotsSell[_previousIndexMajorSell] = 0;
                    DotsBuy[_previousIndexMajorSell] = double.NaN;
                }
                _previousCyclePriceMajorSell = _cyclePrice[index];
                _previousIndexMajorSell = index;
                _flatMajorSell = true;
            }
            else if (_cyclePrice[index] < _previousCyclePriceMajorSell)
            {
                if (_zeroLag[index] < _zeroLag[index - 1] || !_useCycleFilter)
                {
                    MajorSell[_previousIndexMajorSell] = 1;
                    Line[_previousIndexMajorSell] = 1;
                    Line[index] = 0;

                    DotsSell[_previousIndexMajorSell] = 0;
                    DotsBuy[_previousIndexMajorSell] = double.NaN;
                }

                _flatMajorSell = true;
                _indexDiffMajor = index - _previousIndexMajorSell;

                double cyclePrice4 = UseRsiFilter == 0
                                         ? _ma.Result[_previousIndexMajorSell]
                                         : _rsi.Result[_previousIndexMajorSell];

                if (cyclePrice4 - _cyclePrice[index] >= sweep && _indexDiffMajor >= 1)
                {
                    _switchMajor = 1;
                    _previousCyclePriceMajorBuy = _cyclePrice[index];
                    _previousIndexMajorBuy = index;
                    _flatMajorSell = false;
                    _flatMajorBuy = false;
                }
            }
        }
    }
}