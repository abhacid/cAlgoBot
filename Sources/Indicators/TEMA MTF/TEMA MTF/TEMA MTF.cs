using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC)]
    public class TEMAMTF : Indicator
    {
        [Parameter(DefaultValue = 5)]
        public int PeriodFast { get; set; }

        [Parameter(DefaultValue = 21)]
        public int PeriodSlow { get; set; }

        [Parameter("TEMA Timeframe", DefaultValue = "Daily")]
        public TimeFrame TEMATimeframe { get; set; }

        [Output("TFast", Color = Colors.Blue)]
        public IndicatorDataSeries TFast { get; set; }

        [Output("TSlow", Color = Colors.Red)]
        public IndicatorDataSeries TSlow { get; set; }


        private MarketSeries series1D;

        private TEMA TemaFast;
        private TEMA TemaSlow;

        protected override void Initialize()
        {
            series1D = MarketData.GetSeries(TEMATimeframe);

            TemaFast = Indicators.GetIndicator<TEMA>(series1D.Close, PeriodFast);
            TemaSlow = Indicators.GetIndicator<TEMA>(series1D.Close, PeriodSlow);

        }

        public override void Calculate(int index)
        {

            var index1 = GetIndexByDate(series1D, MarketSeries.OpenTime[index]);
            if (index1 != -1)
            {
                TFast[index] = TemaFast.tema[index1];
                TSlow[index] = TemaSlow.tema[index1];
            }

        }


        private int GetIndexByDate(MarketSeries series, DateTime time)
        {
            for (int i = series.Close.Count - 1; i > 0; i--)
            {
                if (time == series.OpenTime[i])
                    return i;
            }
            return -1;
        }
    }
}
