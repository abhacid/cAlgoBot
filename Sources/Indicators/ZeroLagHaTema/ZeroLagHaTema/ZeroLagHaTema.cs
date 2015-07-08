//#reference: ..\Indicators\Tema.algo
using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class ZeroLagHaTema : Indicator
    {
        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Output("ZeroLag HaTema", Color = Colors.OrangeRed)]
        public IndicatorDataSeries Result { get; set; }

        private IndicatorDataSeries _haClose;
        private IndicatorDataSeries _haOpen;

        private Tema _tema;
        private Tema _tema2;

        protected override void Initialize()
        {
            _haOpen = CreateDataSeries();
            _haClose = CreateDataSeries();

            if (_haClose != null)
                _tema = Indicators.GetIndicator<Tema>(_haClose, Period);
            if (_tema != null)
                _tema2 = Indicators.GetIndicator<Tema>(_tema.Result, Period);
        }

        public override void Calculate(int index)
        {
            if (index < 1)
            {
                _haOpen[index] = 0.0;
                _haClose[index] = 0.0;
                return;
            }

            _haOpen[index] = ((MarketSeries.Open[index - 1] + MarketSeries.High[index - 1] + MarketSeries.Low[index - 1] + MarketSeries.Close[index - 1]) / 4.0 + _haOpen[index - 1]) / 2.0;

            _haClose[index] = ((MarketSeries.Open[index] + MarketSeries.High[index] + MarketSeries.Low[index] + MarketSeries.Close[index]) / 4.0 + _haOpen[index] + Math.Max(MarketSeries.High[index], _haOpen[index]) + Math.Min(MarketSeries.Low[index], _haOpen[index])) / 4.0;


            Result[index] = 2 * _tema.Result[index] - _tema2.Result[index];


        }
    }
}
