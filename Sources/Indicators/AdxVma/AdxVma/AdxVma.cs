using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class AdxVma : Indicator
    {
        private double _k;

        private IndicatorDataSeries _iSeries;
        private IndicatorDataSeries _mdiSeries;
        private IndicatorDataSeries _mdmSeries;
        private IndicatorDataSeries _pdiSeries;
        private IndicatorDataSeries _pdmSeries;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 6)]
        public int Period { get; set; }

        [Output("Rising", Color = Colors.Green, PlotType = PlotType.Points, Thickness = 2)]
        public IndicatorDataSeries Rising { get; set; }

        [Output("Falling", Color = Colors.Red, PlotType = PlotType.Points, Thickness = 2)]
        public IndicatorDataSeries Falling { get; set; }

        [Output("Flat", Color = Colors.Gold, PlotType = PlotType.Points, Thickness = 2)]
        public IndicatorDataSeries Flat { get; set; }

        [Output("Result", Color = Colors.Black)]
        public IndicatorDataSeries Result { get; set; }


        protected override void Initialize()
        {
            _pdmSeries = CreateDataSeries();
            _mdmSeries = CreateDataSeries();
            _pdiSeries = CreateDataSeries();
            _mdiSeries = CreateDataSeries();
            _iSeries = CreateDataSeries();

            _k = 1.0 / Period;
        }


        public override void Calculate(int index)
        {
            if (index < Period)
            {
                _pdmSeries[index] = 0;
                _mdmSeries[index] = 0;
                _pdiSeries[index] = 0;
                _mdiSeries[index] = 0;
                _iSeries[index] = 0;
                _pdmSeries[index] = 0;
                Result[index] = Source[index];
                return;
            }
            double pdm = Math.Max((Source[index] - Source[index - 1]), 0);
            double mdm = Math.Max((Source[index - 1] - Source[index]), 0);

            _pdmSeries[index] = ((1 - _k) * _pdmSeries[index - 1] + _k * pdm);
            _mdmSeries[index] = ((1 - _k) * _mdmSeries[index - 1] + _k * mdm);

            double sum = _pdmSeries[index] + _mdmSeries[index];
            double pdi = 0.0;
            double mdi = 0.0;

            if (sum > double.Epsilon)
            {
                pdi = _pdmSeries[index] / sum;
                mdi = _mdmSeries[index] / sum;
            }

            _pdiSeries[index] = ((1 - _k) * _pdiSeries[index - 1] + _k * pdi);
            _mdiSeries[index] = ((1 - _k) * _mdiSeries[index - 1] + _k * mdi);

            double diff = Math.Abs(_pdiSeries[index] - _mdiSeries[index]);

            sum = _pdiSeries[index] + _mdiSeries[index];

            if (sum > double.Epsilon)
                _iSeries[index] = ((1 - _k) * _iSeries[1] + _k * diff / sum);


            double hhv = Math.Max(_iSeries[index], _iSeries.Maximum(Period));
            double llv = Math.Min(_iSeries[index], _iSeries.Minimum(Period));

            diff = hhv - llv;
            double vIndex = 0;

            if (diff > double.Epsilon)
                vIndex = (_iSeries[index] - llv) / diff;

            Result[index] = (1 - _k * vIndex) * Result[index - 1] + _k * vIndex * Source[index];

            Rising[index] = double.NaN;
            Falling[index] = double.NaN;
            Flat[index] = double.NaN;

            if (Result.IsRising())
                Rising[index] = Result[index];
            else if (Result.IsFalling())
                Falling[index] = Result[index];
            else
                Flat[index] = Result[index];
        }
    }
}
