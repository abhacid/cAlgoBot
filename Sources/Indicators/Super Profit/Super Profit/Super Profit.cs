using System;
using cAlgo.API;
using System.Runtime.InteropServices;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SuperProfit : Indicator
    {
        // Alert
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        [Parameter(DefaultValue = 35)]
        public int DllPeriod { get; set; }

        [Parameter(DefaultValue = 1.7)]
        public double Period { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Weighted)]
        public MovingAverageType MaType { get; set; }

        [Parameter()]
        public DataSeries Price { get; set; }

        [Parameter(DefaultValue = 5)]
        public int StopLoss { get; set; }

        [Parameter(DefaultValue = 20)]
        public int TakeProfit { get; set; }

        [Output("Up", PlotType = PlotType.Points, Thickness = 4)]
        public IndicatorDataSeries UpSeries { get; set; }

        [Output("Down", PlotType = PlotType.Points, Color = Colors.Red, Thickness = 4)]
        public IndicatorDataSeries DownSeries { get; set; }


        private DateTime _openTime;

        private MovingAverage _movingAverage1;
        private MovingAverage _movingAverage2;
        private MovingAverage _movingAverage3;
        private IndicatorDataSeries _dataSeries;
        private IndicatorDataSeries _trend;


        protected override void Initialize()
        {
            _dataSeries = CreateDataSeries();
            _trend = CreateDataSeries();

            var period1 = (int)Math.Floor(DllPeriod / Period);
            var period2 = (int)Math.Floor(Math.Sqrt(DllPeriod));

            _movingAverage1 = Indicators.MovingAverage(Price, period1, MaType);
            _movingAverage2 = Indicators.MovingAverage(Price, DllPeriod, MaType);
            _movingAverage3 = Indicators.MovingAverage(_dataSeries, period2, MaType);

        }

        public override void Calculate(int index)
        {
            if (index < 1)
                return;

            _dataSeries[index] = 2.0 * _movingAverage1.Result[index] - _movingAverage2.Result[index];
            _trend[index] = _trend[index - 1];

            if (_movingAverage3.Result[index] > _movingAverage3.Result[index - 1])
                _trend[index] = 1;
            else if (_movingAverage3.Result[index] < _movingAverage3.Result[index - 1])
                _trend[index] = -1;

            if (_trend[index] > 0)
            {
                UpSeries[index] = _movingAverage3.Result[index];

                if (_trend[index - 1] < 0.0)
                {
                    UpSeries[index - 1] = _movingAverage3.Result[index - 1];

                    if (IsLastBar)
                    {
                        var stopLoss = MarketSeries.Low[index - 1] - StopLoss * Symbol.PipSize;
                        var takeProfit = MarketSeries.Close[index] + TakeProfit * Symbol.PipSize;
                        var entryPrice = MarketSeries.Close[index - 1];

                        if (MarketSeries.OpenTime[index] != _openTime)
                        {
                            _openTime = MarketSeries.OpenTime[index];
                            DisplayAlert("Buy signal", takeProfit, stopLoss, entryPrice);
                        }
                    }
                }

                DownSeries[index] = double.NaN;
            }
            else if (_trend[index] < 0)
            {
                DownSeries[index] = _movingAverage3.Result[index];

                if (_trend[index - 1] > 0.0)
                {
                    DownSeries[index - 1] = _movingAverage3.Result[index - 1];

                    if (IsLastBar)
                    {
                        var stopLoss = MarketSeries.High[index - 1] + StopLoss * Symbol.PipSize;
                        var takeProfit = MarketSeries.Close[index] - TakeProfit * Symbol.PipSize;
                        var entryPrice = MarketSeries.Close[index - 1];

                        if (MarketSeries.OpenTime[index] != _openTime)
                        {
                            _openTime = MarketSeries.OpenTime[index];
                            DisplayAlert("Sell signal", takeProfit, stopLoss, entryPrice);
                        }
                    }
                }

                UpSeries[index] = double.NaN;
            }

        }

        protected void DisplayAlert(string tradyTypeSignal, double takeProfit, double stopLoss, double entryPrice)
        {
            string entryPricetext = entryPrice != 0.0 ? string.Format(" at price {0}", Math.Round(entryPrice, 4)) : "";
            string takeProfitText = takeProfit != 0.0 ? string.Format(", TP on  {0}", Math.Round(takeProfit, 4)) : "";
            string stopLossText = stopLoss != 0.0 ? string.Format(", SL on {0}", Math.Round(stopLoss, 4)) : "";

            var alertMessage = string.Format("{0} {1} {2} {3} {4}", tradyTypeSignal, entryPricetext, takeProfitText, stopLossText, Symbol.Code);

            MessageBox(new IntPtr(0), alertMessage, "Trade Signal", 0);

        }
    }
}
