using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class HeikenAshiSmoothed : Indicator
    {
        [Parameter(DefaultValue = 5, MinValue = 1)]
        public int Periods { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }

        private MovingAverage maOpen;
        private MovingAverage maClose;
        private MovingAverage maHigh;
        private MovingAverage maLow;
        private IndicatorDataSeries haClose;
        private IndicatorDataSeries haOpen;

        protected override void Initialize()
        {
            maOpen = Indicators.MovingAverage(MarketSeries.Open, Periods, MAType);
            maClose = Indicators.MovingAverage(MarketSeries.Close, Periods, MAType);
            maHigh = Indicators.MovingAverage(MarketSeries.High, Periods, MAType);
            maLow = Indicators.MovingAverage(MarketSeries.Low, Periods, MAType);
            haOpen = CreateDataSeries();
            haClose = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            double haHigh;
            double haLow;
            Colors Color;

            if (index > 0 && !double.IsNaN(maOpen.Result[index - 1]))
            {
                haOpen[index] = (haOpen[index - 1] + haClose[index - 1]) / 2;
                haClose[index] = (maOpen.Result[index] + maClose.Result[index] + maHigh.Result[index] + maLow.Result[index]) / 4;
                haHigh = Math.Max(maHigh.Result[index], Math.Max(haOpen[index], haClose[index]));
                haLow = Math.Min(maLow.Result[index], Math.Min(haOpen[index], haClose[index]));
                Color = (haOpen[index] > haClose[index]) ? Colors.Red : Colors.Blue;
                ChartObjects.DrawLine("BarHA" + index, index, haOpen[index], index, haClose[index], Color, 5, LineStyle.Solid);
                ChartObjects.DrawLine("LineHA" + index, index, haHigh, index, haLow, Color, 1, LineStyle.Solid);
            }
            else if (!double.IsNaN(maOpen.Result[index]))
            {
                haOpen[index] = (maOpen.Result[index] + maClose.Result[index]) / 2;
                haClose[index] = (maOpen.Result[index] + maClose.Result[index] + maHigh.Result[index] + maLow.Result[index]) / 4;
                haHigh = maHigh.Result[index];
                haLow = maLow.Result[index];
            }
        }
    }
}
