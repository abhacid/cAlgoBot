using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class HilbertTransfrorm : Indicator
    {
        private const int MedianArraySize = 5;
        private IndicatorDataSeries _cycle;
        private IndicatorDataSeries _deltaPhase;
        private IndicatorDataSeries _i1;
        private IndicatorDataSeries _instPeriod;
        private double _medianDelta;
        private IndicatorDataSeries _price;
        private IndicatorDataSeries _q1;
        private IndicatorDataSeries _smooth;

        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        [Output("InPhase", Color = Colors.Blue)]
        public IndicatorDataSeries InPhase { get; set; }

        [Output("Quadrature", Color = Colors.Green)]
        public IndicatorDataSeries Quadrature { get; set; }

        protected override void Initialize()
        {
            _price = CreateDataSeries();
            _smooth = CreateDataSeries();
            _i1 = CreateDataSeries();
            _q1 = CreateDataSeries();
            _deltaPhase = CreateDataSeries();
            _instPeriod = CreateDataSeries();
            _cycle = CreateDataSeries();
        }


        public override void Calculate(int index)
        {
            _price[index] = Source[index];
            _smooth[index] = (_price[index] + 2*_price[index - 1] + 2*_price[index - 2] + _price[index - 3])/6;

            if (index < 7)
            {
                _i1[index] = 0;
                _q1[index] = 0;
                _deltaPhase[index] = 0;
                _instPeriod[index] = 0;

                _cycle[index] = (_price[index] - 2*_price[index - 1] + _price[index - 2])/4;

                return;
            }

            _cycle[index] = (1 - 0.5*Alpha)*(1 - 0.5*Alpha)*(_smooth[index] - 2*_smooth[index - 1] + _smooth[index - 2])
                            + 2*(1 - Alpha)*_cycle[index - 1] - (1 - Alpha)*(1 - Alpha)*(_cycle[index - 2]);

            _q1[index] = (0.0962*_cycle[index] + 0.5769*_cycle[index - 2] - 0.5769*_cycle[index - 4] -
                          0.0962*_cycle[index - 6])
                         *(0.5 + 0.08*_instPeriod[index - 1]);

            _i1[index] = _cycle[index - 3];


            if (Math.Abs(_q1[index] - 0.0) > double.Epsilon && Math.Abs(_q1[index - 1] - 0.0) > double.Epsilon)
                _deltaPhase[index] = (_i1[index]/_q1[index] - _i1[index - 1]/_q1[index - 1])
                                     /(1.0 + _i1[index]*_i1[index - 1]/(_q1[index]*_q1[index - 1]));

            //Set Boundaries
            if (_deltaPhase[index] < 0.1)
                _deltaPhase[index] = 0.1;
            else if (_deltaPhase[index] > 1.1)
                _deltaPhase[index] = 1.1;

            // Median Delta
            var array = new double[MedianArraySize];
            for (int i = 0; i < MedianArraySize; i++)
            {
                array[i] = _deltaPhase[index - i];
            }

            Array.Sort(array);
            const int median = MedianArraySize/2;
            _medianDelta = array[median];

            double dc = 15.0;
            if (Math.Abs(_medianDelta - 0) > double.Epsilon)
                dc = 6.28318/_medianDelta + 0.5;

            _instPeriod[index] = 0.33*dc + 0.67*_instPeriod[index - 1];

            Quadrature[index] = _q1[index];
            InPhase[index] = _i1[index];
        }
    }
}