//#reference: ..\Indicators\TEMA.algo

using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class ZeroLagTema : Indicator
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Output("ZeroLag Tema", Color = Colors.OrangeRed)]
        public IndicatorDataSeries Result { get; set; }

        private Tema _tema;
        private Tema _tema2;

        protected override void Initialize()
        {
            _tema = Indicators.GetIndicator<Tema>(Source, Period);
            _tema2 = Indicators.GetIndicator<Tema>(_tema.Result, Period);
        }

        public override void Calculate(int index)
        {
            Result[index] = 2 * _tema.Result[index] - _tema2.Result[index];
        }
    }
}
