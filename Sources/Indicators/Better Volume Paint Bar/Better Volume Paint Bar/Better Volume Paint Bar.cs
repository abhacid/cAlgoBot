using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Tick_BetterVolume : Indicator
    {
        [Parameter("Climax High", DefaultValue = "Red")]
        public string RedVolume { get; set; }

        [Parameter("Neutral", DefaultValue = "Blue")]
        public string BlueVolume { get; set; }

        [Parameter("Volume Low", DefaultValue = "Yellow")]
        public string YellowVolume { get; set; }

        [Parameter("High Churn", DefaultValue = "Green")]
        public string GreenVolume { get; set; }

        [Parameter("Climax Low", DefaultValue = "PaleGreen")]
        public string WhiteVolume { get; set; }

        [Parameter("Climax Churn", DefaultValue = "Magenta")]
        public string MagentaVolume { get; set; }

        [Parameter("MAPeriod", DefaultValue = 5)]
        public int MAPeriod { get; set; }

        [Parameter("LookBack", DefaultValue = 10)]
        public int LookBack { get; set; }

        [Parameter("Show description", DefaultValue = false)]
        public bool ShowDesc { get; set; }

        [Parameter("Candle width", DefaultValue = 5)]
        public int CandleWidth { get; set; }

        [Parameter("Wick width", DefaultValue = 1)]
        public int WickWidth { get; set; }


        private Colors color;
        private Colors colorRedVolume;
        private Colors colorBlueVolume;
        private Colors colorYellowVolume;
        private Colors colorGreenVolume;
        private Colors colorWhiteVolume;
        private Colors colorMagentaVolume;

        public IndicatorDataSeries VolumeMA { get; set; }
        public IndicatorDataSeries dataRedVolume { get; set; }
        public IndicatorDataSeries dataBlueVolume { get; set; }
        public IndicatorDataSeries dataYellowVolume { get; set; }
        public IndicatorDataSeries dataGreenVolume { get; set; }
        public IndicatorDataSeries dataWhiteVolume { get; set; }
        public IndicatorDataSeries dataMagentaVolume { get; set; }


        private bool _incorrectColors;
        private Random _random = new Random();

        protected override void Initialize()
        {
            if (!Enum.TryParse<Colors>(RedVolume, out colorRedVolume) || !Enum.TryParse<Colors>(BlueVolume, out colorBlueVolume) || !Enum.TryParse<Colors>(YellowVolume, out colorYellowVolume) || !Enum.TryParse<Colors>(GreenVolume, out colorGreenVolume) || !Enum.TryParse<Colors>(WhiteVolume, out colorWhiteVolume) || !Enum.TryParse<Colors>(MagentaVolume, out colorMagentaVolume))

                _incorrectColors = true;

            VolumeMA = CreateDataSeries();
            dataRedVolume = CreateDataSeries();
            dataBlueVolume = CreateDataSeries();
            dataYellowVolume = CreateDataSeries();
            dataGreenVolume = CreateDataSeries();
            dataWhiteVolume = CreateDataSeries();
            dataMagentaVolume = CreateDataSeries();
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
            if (_incorrectColors)
            {
                var errorColor = _random.Next(2) == 0 ? Colors.Red : Colors.White;
                ChartObjects.DrawText("Error", "Incorrect colors", StaticPosition.Center, errorColor);
                return;
            }

            var open = MarketSeries.Open[index];
            var high = MarketSeries.High[index];
            var low = MarketSeries.Low[index];
            var close = MarketSeries.Close[index];

            dataRedVolume[index] = 0;
            dataBlueVolume[index] = MarketSeries.TickVolume[index];
            dataYellowVolume[index] = 0;
            dataGreenVolume[index] = 0;
            dataWhiteVolume[index] = 0;
            dataMagentaVolume[index] = 0;

            if (close > open)
            {
                color = Colors.LightGray;
                ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
                ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);
            }

            if (close < open)
            {
                color = Colors.Gray;
                ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
                ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);
            }

            int lowestIdx = VolumeLowest(index - LookBack + 1, LookBack);
            if (lowestIdx == -1)
            {
                return;
            }

            double volLowest = MarketSeries.TickVolume[lowestIdx];

            if (MarketSeries.TickVolume[index] == volLowest)
            {
                dataYellowVolume[index] = MarketSeries.TickVolume[index];
                dataBlueVolume[index] = 0;
                color = colorYellowVolume;
                ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
                ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);
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
                dataRedVolume[index] = Math.Round(MarketSeries.TickVolume[index], 0);
                dataBlueVolume[index] = 0;
                dataYellowVolume[index] = 0;
                color = colorRedVolume;
                ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
                ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);
            }

            if (Value3 == HiValue3)
            {
                dataGreenVolume[index] = Math.Round(MarketSeries.TickVolume[index], 0);
                dataBlueVolume[index] = 0;
                dataYellowVolume[index] = 0;
                dataRedVolume[index] = 0;
                color = colorGreenVolume;
                ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
                ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);
            }

            if (Value2 == HiValue2 && Value3 == HiValue3)
            {
                dataMagentaVolume[index] = Math.Round(MarketSeries.TickVolume[index], 0);
                dataBlueVolume[index] = 0;
                dataRedVolume[index] = 0;
                dataGreenVolume[index] = 0;
                dataYellowVolume[index] = 0;
                color = colorMagentaVolume;
                ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
                ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);
            }

            if (Value2 == HiValue2 && MarketSeries.Close[index] <= (MarketSeries.High[index] + MarketSeries.Low[index]) / 2)
            {
                dataWhiteVolume[index] = Math.Round(MarketSeries.TickVolume[index], 0);
                dataMagentaVolume[index] = 0;
                dataBlueVolume[index] = 0;
                dataRedVolume[index] = 0;
                dataGreenVolume[index] = 0;
                dataYellowVolume[index] = 0;
                color = colorWhiteVolume;
                ChartObjects.DrawLine("candle" + index, index, open, index, close, color, CandleWidth, LineStyle.Solid);
                ChartObjects.DrawLine("line" + index, index, high, index, low, color, WickWidth, LineStyle.Solid);

            }

        }

    }
}
