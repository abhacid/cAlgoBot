using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Tick_BetterVolume : Indicator
    {
        [Output("Climax High", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries RedVolume { get; set; }

        [Output("Neutral", Color = Colors.CornflowerBlue, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries BlueVolume { get; set; }

        [Output("Volume Low", Color = Colors.Yellow, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries YellowVolume { get; set; }

        [Output("High Churn", Color = Colors.Green, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries GreenVolume { get; set; }

        [Output("Climax Low", Color = Colors.White, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries WhiteVolume { get; set; }

        [Output("Climx Churn", Color = Colors.Magenta, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries MagentaVolume { get; set; }

        [Output("MA", Color = Colors.DarkGray, PlotType = PlotType.Line, LineStyle = LineStyle.Dots, Thickness = 1)]
        public IndicatorDataSeries VolumeMA { get; set; }

        [Parameter("MAPeriod", DefaultValue = 100)]
        public int MAPeriod { get; set; }

        [Parameter("LookBack", DefaultValue = 20)]
        public int LookBack { get; set; }

        [Parameter("Show description", DefaultValue = true)]
        public bool ShowDesc { get; set; }

        protected override void Initialize()
        {
            ShowDescription();
        }

        int VolumeLowest(int start, int count)
        {
            double min = MarketSeries.TickVolume[start];
            int index = start;

            for (int i = start + 1; i < start + count; ++i)
            {
                double vol = MarketSeries.TickVolume[i];
                if (vol < min)
                {
                    min = vol;
                    index = i;
                }
            }

            return index;
        }

        public override void Calculate(int index)
        {
            RedVolume[index] = 0;
            BlueVolume[index] = MarketSeries.TickVolume[index];
            YellowVolume[index] = 0;
            GreenVolume[index] = 0;
            WhiteVolume[index] = 0;
            MagentaVolume[index] = 0;

            int lowestIdx = VolumeLowest(index - LookBack + 1, LookBack);
            if (lowestIdx == -1)
            {
                return;
            }

            double volLowest = MarketSeries.TickVolume[lowestIdx];

            if (MarketSeries.TickVolume[index] == volLowest)
            {
                YellowVolume[index] = MarketSeries.TickVolume[index];
                BlueVolume[index] = 0;
            }

            double Range = MarketSeries.High[index] - MarketSeries.Low[index];
            if (Range < 0.5 * Symbol.PipSize)
            {
                return;
            }

            double Value2 = MarketSeries.TickVolume[index] * Range;

            double Value3 = 0;
            if (Range != 0)
            {
                Value3 = MarketSeries.TickVolume[index] / Range;
            }

            double tempv = 0;
            for (int n = index - MAPeriod + 1; n <= index; ++n)
            {
                if (!double.IsNaN(MarketSeries.TickVolume[n]))
                {
                    tempv += MarketSeries.TickVolume[n];
                }
            }

            double tempv2 = 0, tempv3 = 0, HiValue2 = 0, HiValue3 = 0, LoValue3 = double.MaxValue;
            for (int n = index - LookBack + 1; n <= index; ++n)
            {
                if (!double.IsNaN(MarketSeries.TickVolume[n]))
                {
                    tempv2 = MarketSeries.TickVolume[n] * (MarketSeries.High[n] - MarketSeries.Low[n]);

                    if (tempv2 >= HiValue2)
                    {
                        HiValue2 = tempv2;
                    }

                    if (tempv2 != 0)
                    {
                        tempv3 = MarketSeries.TickVolume[n] / (MarketSeries.High[n] - MarketSeries.Low[n]);
                        if (tempv3 > HiValue3)
                        {
                            HiValue3 = tempv3;
                        }
                        if (tempv3 < LoValue3)
                        {
                            LoValue3 = tempv3;
                        }
                    }
                }
            }

            if (!double.IsNaN(tempv))
            {
                VolumeMA[index] = Math.Round(tempv / MAPeriod, 0);
            }

            if (Value2 == HiValue2 && MarketSeries.Close[index] > (MarketSeries.High[index] + MarketSeries.Low[index]) / 2)
            {
                RedVolume[index] = Math.Round(MarketSeries.TickVolume[index], 0);
                BlueVolume[index] = 0;
                YellowVolume[index] = 0;
            }

            if (Value3 == HiValue3)
            {
                GreenVolume[index] = Math.Round(MarketSeries.TickVolume[index], 0);
                BlueVolume[index] = 0;
                YellowVolume[index] = 0;
                RedVolume[index] = 0;
            }

            if (Value2 == HiValue2 && Value3 == HiValue3)
            {
                MagentaVolume[index] = Math.Round(MarketSeries.TickVolume[index], 0);
                BlueVolume[index] = 0;
                RedVolume[index] = 0;
                GreenVolume[index] = 0;
                YellowVolume[index] = 0;
            }

            if (Value2 == HiValue2 && MarketSeries.Close[index] <= (MarketSeries.High[index] + MarketSeries.Low[index]) / 2)
            {
                WhiteVolume[index] = Math.Round(MarketSeries.TickVolume[index], 0);
                MagentaVolume[index] = 0;
                BlueVolume[index] = 0;
                RedVolume[index] = 0;
                GreenVolume[index] = 0;
                YellowVolume[index] = 0;
            }
        }

        private static T GetAttributeFrom<T>(object instance, string propertyName)
        {
            var attrType = typeof(T);
            var property = instance.GetType().GetProperty(propertyName);
            return (T)property.GetCustomAttributes(attrType, false).GetValue(0);
        }

        private void ShowDescription()
        {
            if (ShowDesc)
            {
                ChartObjects.DrawText("desc1", "Climax High", StaticPosition.TopRight, GetAttributeFrom<OutputAttribute>(this, "RedVolume").Color);
                ChartObjects.DrawText("desc2", "\nNeutral", StaticPosition.TopRight, GetAttributeFrom<OutputAttribute>(this, "BlueVolume").Color);
                ChartObjects.DrawText("desc3", "\n\nVolume Low", StaticPosition.TopRight, GetAttributeFrom<OutputAttribute>(this, "YellowVolume").Color);
                ChartObjects.DrawText("desc4", "\n\n\nHigh Churn", StaticPosition.TopRight, GetAttributeFrom<OutputAttribute>(this, "GreenVolume").Color);
                ChartObjects.DrawText("desc5", "\n\n\n\nClimax Low", StaticPosition.TopRight, GetAttributeFrom<OutputAttribute>(this, "WhiteVolume").Color);
                ChartObjects.DrawText("desc6", "\n\n\n\n\nClimax Churn", StaticPosition.TopRight, GetAttributeFrom<OutputAttribute>(this, "MagentaVolume").Color);
            }
        }
    }
}
