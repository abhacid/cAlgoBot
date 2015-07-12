using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Coral : Indicator
    {
        private IndicatorDataSeries _iSeries1;
        private IndicatorDataSeries _iSeries2;
        private IndicatorDataSeries _iSeries3;
        private IndicatorDataSeries _iSeries4;
        private IndicatorDataSeries _iSeries5;
        private IndicatorDataSeries _iSeries6;
        private IndicatorDataSeries _buffer;

        private const double D = 0.4;
        private double _coeff1;
        private double _coeff2;
        private double _coeff3;
        private double _coeff4;
        private double _coeff5;
        private bool upTrend;

        [Parameter("Smooth", DefaultValue = 34, MinValue = 1)]
        public double DI { get; set; }

        [Output("UpBuffer", Color = Colors.DarkGreen, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries UpBuffer { get; set; }
        [Output("DownBuffer", Color = Colors.DarkRed, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries DownBuffer { get; set; }
        [Output("HzBuffer", Color = Colors.Yellow, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries HzBuffer { get; set; }

        protected override void Initialize()
        {
            _buffer = CreateDataSeries();
            _iSeries1 = CreateDataSeries();
            _iSeries2 = CreateDataSeries();
            _iSeries3 = CreateDataSeries();
            _iSeries4 = CreateDataSeries();
            _iSeries5 = CreateDataSeries();
            _iSeries6 = CreateDataSeries();


            DI = (DI - 1.0) / 2.0 + 1.0;
            _coeff1 = 2 / (DI + 1.0);
            _coeff2 = 1 - _coeff1;

            _coeff3 = 3.0 * (D * D + D * D * D);
            _coeff4 = -3.0 * (2.0 * D * D + D + D * D * D);
            _coeff5 = 3.0 * D + 1.0 + D * D * D + 3.0 * D * D;

            Print(Symbol.PipSize.ToString());
            Print(Symbol.PointSize.ToString());
            Print(Symbol.Digits.ToString());

        }
        public override void Calculate(int index)
        {
            if (index < 2)
            {
                _iSeries1[index] = 0;
                _iSeries2[index] = 0;
                _iSeries3[index] = 0;
                _iSeries4[index] = 0;
                _iSeries5[index] = 0;
                _iSeries6[index] = 0;

                return;
            }


            _iSeries1[index] = _coeff1 * MarketSeries.Close[index] + _coeff2 * _iSeries1[index - 1];
            _iSeries2[index] = _coeff1 * _iSeries1[index] + _coeff2 * _iSeries2[index - 1];
            _iSeries3[index] = _coeff1 * _iSeries2[index] + _coeff2 * _iSeries3[index - 1];
            _iSeries4[index] = _coeff1 * _iSeries3[index] + _coeff2 * _iSeries4[index - 1];
            _iSeries5[index] = _coeff1 * _iSeries4[index] + _coeff2 * _iSeries5[index - 1];
            _iSeries6[index] = _coeff1 * _iSeries5[index] + _coeff2 * _iSeries6[index - 1];

            _buffer[index] = -D * D * D * _iSeries6[index] + _coeff3 * (_iSeries5[index]) + _coeff4 * (_iSeries4[index]) + _coeff5 * (_iSeries3[index]);


            UpBuffer[index] = _buffer[index];
            DownBuffer[index] = _buffer[index];
            HzBuffer[index] = _buffer[index];


            // UpTrend
            if ((_buffer[index] - _buffer[index - 1]) > Symbol.PointSize)
            {

                if (upTrend)
                {
                    DownBuffer[index] = double.NaN;
                    HzBuffer[index] = double.NaN;
                }
                else
                {
                    UpBuffer[index] = double.NaN;
                    DownBuffer[index] = double.NaN;
                    Print("switch uptrend: {0}", MarketSeries.OpenTime[index]);
                }

                upTrend = true;
            }
            // DownTrend
            else if ((_buffer[index - 1] - _buffer[index]) > Symbol.PointSize)
            {

                if (!upTrend)
                {
                    UpBuffer[index] = double.NaN;
                    HzBuffer[index] = double.NaN;
                }
                else
                {
                    UpBuffer[index] = double.NaN;
                    DownBuffer[index] = double.NaN;
                    Print("switch downtrend: {0}", MarketSeries.OpenTime[index]);

                }
                upTrend = false;
            }

        }


    }
}
