
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class SidusBago : Indicator
    {
        #region Input

        [Parameter()]
        public DataSeries Price { get; set; }

        [Parameter("FastEMA", DefaultValue = 5)]
        public int FastEMA { get; set; }

        [Parameter("SlowEMA", DefaultValue = 12)]
        public int SlowEMA { get; set; }

        [Parameter("RSIPeriod", DefaultValue = 21)]
        public int RSIPeriod { get; set; }

        [Parameter("Alerts", DefaultValue = 1, MinValue = 0, MaxValue = 1)]
        public int Alerts { get; set; }


        #endregion

        #region indicator line

        [Output("ExtMapBuffer1", Color = Colors.Yellow)]
        public IndicatorDataSeries ExtMapBuffer1 { get; set; }

        [Output("ExtMapBuffer2", Color = Colors.Red)]
        public IndicatorDataSeries ExtMapBuffer2 { get; set; }

        [Output("ExtMapBuffer3", Color = Colors.Yellow, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries ExtMapBuffer3 { get; set; }

        [Output("ExtMapBuffer4", Color = Colors.Red, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries ExtMapBuffer4 { get; set; }

        #endregion


        private int sigCurrent;

        private int sigPrevious;
        private double pipdiffCurrent;
        private ExponentialMovingAverage _fastEma;
        private ExponentialMovingAverage _slowEma;
        private RelativeStrengthIndex iRSI;

        protected override void Initialize()
        {
            _fastEma = Indicators.ExponentialMovingAverage(MarketSeries.Close, FastEMA);
            _slowEma = Indicators.ExponentialMovingAverage(MarketSeries.Close, SlowEMA);
            iRSI = Indicators.RelativeStrengthIndex(MarketSeries.Close, RSIPeriod);
        }
        public override void Calculate(int index)
        {
            bool entry = false;
            double entry_point = 0;

            ExtMapBuffer1[index] = _fastEma.Result[index];
            ExtMapBuffer2[index] = _slowEma.Result[index];
            double rsi_sig = iRSI.Result[index];

            pipdiffCurrent = Math.Round((ExtMapBuffer1[index] - ExtMapBuffer2[index]), Symbol.Digits);

            if (IsRealTime)
            {
                ChartObjects.DrawText("pipdiffCurrent", "pipdiffCurrent = " + pipdiffCurrent + " ", StaticPosition.TopLeft, Colors.White);
            }
            if (pipdiffCurrent > 0 && rsi_sig > 50)
            {
                sigCurrent = 1;
                //Up
            }
            else if (pipdiffCurrent < 0 && rsi_sig < 50)
            {
                sigCurrent = 2;
                //Down
            }

            ExtMapBuffer3[index - 1] = double.NaN;
            ExtMapBuffer4[index - 1] = double.NaN;

            // Down-> Up
            if (sigCurrent == 1 && sigPrevious == 2)
            {

                ExtMapBuffer3[index - 1] = MarketSeries.Low[index - 1] - 5 * Symbol.PointSize;
                entry = true;
                entry_point = Symbol.Ask;
            }
            // Up -> Down
            else if (sigCurrent == 2 && sigPrevious == 1)
            {
                ExtMapBuffer4[index - 1] = MarketSeries.High[index - 1] - 5 * Symbol.PointSize;
                entry = true;
                entry_point = Symbol.Bid;
            }

            sigPrevious = sigCurrent;

            if (IsRealTime)
            {
                if (Alerts == 1 && entry)
                {
                    Notifications.PlaySound("alert.wav");
                    if (sigPrevious == 1)
                    {
                        ChartObjects.RemoveObject("EntryPointBuy");
                        ChartObjects.DrawText("EntryPointBuy", "Entry point: Buy at " + entry_point + "!!", StaticPosition.TopRight, Colors.Green);
                    }
                    else if (sigPrevious == 2)
                    {
                        ChartObjects.RemoveObject("EntryPointBuy");
                        ChartObjects.DrawText("EntryPointSell", "Entry point: Sell at " + entry_point + "!!", StaticPosition.TopRight, Colors.Red);
                    }
                }
                else
                {
                    ChartObjects.RemoveObject("EntryPointBuy");
                }
            }
        }
    }
}
