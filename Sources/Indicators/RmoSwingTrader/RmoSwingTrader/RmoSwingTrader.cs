//#reference: ..\Indicators\ZeroLagTema.algo
//#reference: ..\Indicators\RMO.algo
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(ScalePrecision = 5)]
    public class RmoSwingTrader : Indicator
    {
        [Parameter(DefaultValue = 2)]
        public int Len1 { get; set; }

        [Parameter(DefaultValue = 10)]
        public int Len2 { get; set; }

        [Parameter(DefaultValue = 30)]
        public int Len3 { get; set; }

        [Parameter(DefaultValue = 81)]
        public int Len4 { get; set; }

        private RMO _rmo;
        private ZeroLagTema _zeroLagTema;

        [Output("Series2", Color = Colors.Blue, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries Series2 { get; set; }

        [Output("Series3", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries Series3 { get; set; }


        protected override void Initialize()
        {
            _rmo = Indicators.GetIndicator<RMO>(Len1, Len2, Len3, Len4);
            _zeroLagTema = Indicators.GetIndicator<ZeroLagTema>(_rmo.St2, 28);

        }

        public override void Calculate(int index)
        {
            if (index < Len4)
                return;

            Series2[index] = _zeroLagTema.Result[index];
            Series3[index] = _rmo.St3[index];
        }
    }
}
