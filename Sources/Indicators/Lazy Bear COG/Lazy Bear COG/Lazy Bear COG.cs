using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class LazyBearCOG : Indicator
    {
        [Parameter(DefaultValue = 34)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 2.5)]
        public double Deviation { get; set; }

        [Parameter(DefaultValue = 20)]
        public double Offset { get; set; }


        [Output("Median Line", PlotType = PlotType.Line, Color = Colors.RoyalBlue, LineStyle = LineStyle.Solid, Thickness = 1)]
        public IndicatorDataSeries MedianLine { get; set; }

        [Output("Upper Line", PlotType = PlotType.Line, Color = Colors.Red, LineStyle = LineStyle.Solid, Thickness = 1)]
        public IndicatorDataSeries UpperLine { get; set; }

        [Output("Upper Range", PlotType = PlotType.Points, Color = Colors.Red, LineStyle = LineStyle.Solid, Thickness = 1)]
        public IndicatorDataSeries UpperRange { get; set; }

        [Output("Upper Compression", PlotType = PlotType.Points, Color = Colors.Red, LineStyle = LineStyle.Solid, Thickness = 5)]
        public IndicatorDataSeries UpperCompression { get; set; }

        [Output("Lower Line", PlotType = PlotType.Line, Color = Colors.DarkGreen, LineStyle = LineStyle.Solid, Thickness = 1)]
        public IndicatorDataSeries LowerLine { get; set; }

        [Output("Lower Range", PlotType = PlotType.Points, Color = Colors.DarkGreen, LineStyle = LineStyle.Solid, Thickness = 1)]
        public IndicatorDataSeries LowerRange { get; set; }

        [Output("Lower Compression", PlotType = PlotType.Points, Color = Colors.DarkGreen, LineStyle = LineStyle.Solid, Thickness = 5)]
        public IndicatorDataSeries LowerCompression { get; set; }


        private StandardDeviation _StdDev;
        private LinearRegressionForecast _LinReg;
        private IndicatorDataSeries _Range;
        private SimpleMovingAverage _Baseline;


        protected override void Initialize()
        {
            _StdDev = Indicators.StandardDeviation(MarketSeries.Close, Period, MovingAverageType.Simple);
            _LinReg = Indicators.LinearRegressionForecast(MarketSeries.Close, Period);
            _Range = CreateDataSeries();
            _Baseline = Indicators.SimpleMovingAverage(_Range, Period);
        }


        public override void Calculate(int index)
        {

            double dev = _StdDev.Result[index] * Deviation;
            double basis = _LinReg.Result[index];

            UpperLine[index] = (basis + dev);
            LowerLine[index] = (basis - dev);
            MedianLine[index] = basis;


            double cRange = MarketSeries.High[index] - MarketSeries.Low[index];
            double pUpper = MarketSeries.High[index] - MarketSeries.Close[index - 1];
            double pLower = MarketSeries.Low[index] - MarketSeries.Close[index - 1];

            _Range[index] = Math.Max(cRange, Math.Max(pUpper, pLower));

            UpperRange[index] = basis + (2 * _Baseline.Result[index]);
            LowerRange[index] = basis - (2 * _Baseline.Result[index]);

            if (UpperRange[index] > UpperLine[index] && LowerRange[index] < LowerLine[index])
            {
                UpperCompression[index] = UpperLine[index];
                LowerCompression[index] = LowerLine[index];
            }

        }



    }
}

