using System;
using cAlgo.API;
using cAlgo.API.Indicators;

using cAlgo.Lib;

namespace cAlgo.Indicators
{
    /// <summary>
    /// https://github.com/abhacid/cAlgoBotdiscussions/554324
    /// </summary>
    [Indicator(ScalePrecision = 0, IsOverlay = false, AccessRights = AccessRights.None)]
    [Levels(30, 70)]
    public class RSIWellesIndicator : Indicator
    {
        [Parameter("Period", DefaultValue = 14, MinValue = 2)]
        public int Period { get; set; }

        [Output("RSI-Welles", Color = Colors.Yellow)]
        public IndicatorDataSeries wellesRSI { get; set; }

        public IndicatorDataSeries AvgRise { get; set; }

        public IndicatorDataSeries AvgFall { get; set; }

        [Output("Overbought", Color = Colors.Turquoise)]
        public IndicatorDataSeries overbought { get; set; }

        [Output("Oversold", Color = Colors.Red)]
        public IndicatorDataSeries oversold { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            AvgRise = CreateDataSeries();
            AvgFall = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            if (IsLastBar)
                return;

            double increasing = 0;
            double fall = 0;
            double rise = 0;

            if (index < Period)
                wellesRSI[index] = 0;
            else if (index == Period)
            {
                for (int i = 0; i < Period; i++)
                {
                    increasing = MarketSeries.Close[i + 1] - MarketSeries.Close[i];
                    if (increasing > 0)
                        rise += increasing;
                    else
                        fall -= increasing;
                }

                AvgRise[index] = rise / Period;
                AvgFall[index] = fall / Period;
            }
            else
            {
                // Rest of averages are smoothed
                increasing = MarketSeries.Close[index] - MarketSeries.Close[index - 1];
                if (increasing > 0)
                    rise = increasing;
                else
                    fall = -increasing;

                AvgRise[index] = (AvgRise[index - 1] * (Period - 1) + rise) / Period;
                AvgFall[index] = (AvgFall[index - 1] * (Period - 1) + fall) / Period;
            }

            wellesRSI[index] = AvgFall[index] == 0 ? 100 : 100 * (1 - 1 / (1 + AvgRise[index] / AvgFall[index]));
        }
    }
}
