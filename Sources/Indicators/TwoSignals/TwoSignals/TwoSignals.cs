using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC)]
    public class MultiSymbolMA : Indicator
    {
        private MarketSeries series2;
        private Symbol symbol2;

        [Parameter(DefaultValue = "EURCHF")]
        public string Symbol2 { get; set; }

        [Output("Symbol2Trace", Color = Colors.LightSkyBlue)]
        public IndicatorDataSeries Symbol2Trace { get; set; }

        [Parameter(DefaultValue = 0)]
        public double Symbol2SerieOffset { get; set; }


        private MovingAverage ma;
        double multiplier;

        protected override void Initialize()
        {
            ma = Indicators.MovingAverage(MarketSeries.Close, 200, MovingAverageType.Simple);
            if (Symbol2 != "")
            {
                symbol2 = MarketData.GetSymbol(Symbol2);
                series2 = MarketData.GetSeries(symbol2, TimeFrame);
            }

            multiplier = Symbol.Ask / symbol2.Ask;

        }

        public override void Calculate(int index)
        {
            DrawSeries(series2, index, Symbol2Trace, 0);
        }

        public void DrawSeries(MarketSeries serie, int index, IndicatorDataSeries indicator, double offset)
        {
            int index2 = serie.OpenTime.GetIndexByExactTime(MarketSeries.OpenTime[index]);

            Print("{0} - {1}", MarketSeries.OpenTime[index], serie.OpenTime[index2]);

            if (serie != null)
                indicator[index2] = (serie.Close[index2 + (int)Symbol2SerieOffset]) * multiplier;
        }
    }
}
