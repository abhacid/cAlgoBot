using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class SchaffTrendCycle : Indicator
    {
        private IndicatorDataSeries _frac1;
        private IndicatorDataSeries _frac2;
        private MacdHistogram _macd;
        private IndicatorDataSeries _pf;
        private IndicatorDataSeries _xmac;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("Short Cycle", DefaultValue = 23)]
        public int ShortCycle { get; set; }

        [Parameter("Long Cycle", DefaultValue = 50)]
        public int LongCycle { get; set; }

        [Parameter("Period", DefaultValue = 10)]
        public int Period { get; set; }

        [Parameter("Factor", DefaultValue = 0.5)]
        public double Factor { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _frac1 = CreateDataSeries();
            _frac2 = CreateDataSeries();
            _pf = CreateDataSeries();
            _macd = Indicators.MacdHistogram(LongCycle, ShortCycle, Period);
        }

        public override void Calculate(int index)
        {
            double low = Lowest(_macd.Histogram, index);
            double high = Highest(_macd.Histogram, index) - low;

            if (high > 0)
                _frac1[index] = 100 * (_macd.Histogram[index] - low) / high;
            else if (index > 0)
                _frac1[index] = _frac1[index - 1];
            else
                _frac1[index] = 0;

            if (index > 0)
                _pf[index] = _pf[index - 1] + Factor * (_frac1[index] - _pf[index - 1]);
            else
                _pf[index] = _frac1[index];

            low = Lowest(_pf, index);
            high = Highest(_pf, index) - low;

            if (high > 0)
                _frac2[index] = 100 * (_pf[index] - low) / high;
            else if (index > 0)
                _frac2[index] = _frac2[index - 1];
            else
                _frac2[index] = 0;

            if (index > 0)
                Result[index] = Result[index - 1] + Factor * (_frac2[index] - Result[index - 1]);
            else
                Result[index] = _frac2[index];
        }

        private double Highest(IndicatorDataSeries macd, int index)
        {
            double high = 0.0;
            for (int i = index - Period; i < index; i++)
            {
                if (macd[i] > high)
                    high = macd[i];
            }
            return high;
        }

        private double Lowest(IndicatorDataSeries macd, int index)
        {
            double low = macd[index - Period];
            for (int i = index - Period; i < index; i++)
            {
                if (macd[i] < low)
                    low = macd[i];
            }
            return low;
        }
    }
}
