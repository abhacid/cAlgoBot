using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, ScalePrecision = 2, AutoRescale = false)]
    public class mySmoothSMASlope : Indicator
    {
        [Parameter("MAPeriods", DefaultValue = 24)]
        public int paramPeriods { get; set; }
        [Parameter("MASmoothing", DefaultValue = 3)]
        public int paramSmoothing { get; set; }
        [Parameter("SlopeBackStep", DefaultValue = 2)]
        //it take 3 periods to determine a peak
        public int paramBackstep { get; set; }
        //a 3 pip slope has enough momentum to generate a 10 pip reversal
        [Parameter("SlopeLimit", DefaultValue = 1)]
        public double paramSlopeLimit { get; set; }
        [Parameter("MATolerance", DefaultValue = 2)]
        //MA must reverse greater than this value to register a peak
        public double paramMATolerance { get; set; }
        [Parameter("SlopePeakTolerance", DefaultValue = 0.25)]
        //slope must be greater than this value to register a peak
        public double paramSlopePeakTolerance { get; set; }

        [Output("Slope", PlotType = PlotType.Histogram, Thickness = 2, Color = Colors.Purple)]
        public IndicatorDataSeries Result { get; set; }
        [Output("CrossRatio", Color = Colors.Aqua, Thickness = 1, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries Xsignal { get; set; }
        [Output("CrossRatioPts", Color = Colors.White, Thickness = 3, PlotType = PlotType.Points)]
        public IndicatorDataSeries XsignalPoints { get; set; }
        [Output("Peak", PlotType = PlotType.Points, Thickness = 4, Color = Colors.Blue)]
        public IndicatorDataSeries Peak { get; set; }
        [Output("Valley", PlotType = PlotType.Points, Thickness = 4, Color = Colors.Red)]
        public IndicatorDataSeries Valley { get; set; }
        [Output("FlatSignal", Color = Colors.Yellow, Thickness = 3, PlotType = PlotType.Points)]
        public IndicatorDataSeries FlatSignal { get; set; }
        [Output("Center", LineStyle = LineStyle.DotsRare, Color = Colors.White)]
        public IndicatorDataSeries CenterLine { get; set; }
        [Output("UpperLimit", PlotType = PlotType.Line, LineStyle = LineStyle.DotsRare, Thickness = 1, Color = Colors.Red)]
        public IndicatorDataSeries UpperLimit { get; set; }
        [Output("LowerLimit", PlotType = PlotType.Line, LineStyle = LineStyle.DotsRare, Thickness = 1, Color = Colors.Red)]
        public IndicatorDataSeries LowerLimit { get; set; }

        private mySmoothSMA MA;
        double LastSlopePeak;
        double LastSlopeValley;
        double LastPeakMA;
        double LastPeakClose;
        int PeakCount;

        protected override void Initialize()
        {
            string IndicatorName = GetType().ToString().Substring(GetType().ToString().LastIndexOf('.') + 1);
            //  returns ClassName
            Print("Indicator: " + IndicatorName);
            Print("IndicatorTimeZone: {0} Offset: {1} DST: {2}", TimeZone, TimeZone.BaseUtcOffset, TimeZone.SupportsDaylightSavingTime);

            PeakCount = 0;
            LastSlopePeak = paramSlopePeakTolerance;
            LastSlopeValley = -paramSlopePeakTolerance;
            LastPeakClose = 0;

            MA = Indicators.GetIndicator<mySmoothSMA>(MarketSeries.Close, paramPeriods, paramSmoothing);
        }

        public override void Calculate(int index)
        {
            int t0 = index;
            int t1 = t0 - 1;
            int t2 = t1 - 1;
            int t3 = t2 - 1;
            int tb = t0 - paramBackstep;
            double MADiff;
            double CloseDiff;

            if (tb < 0)
                return;
            //** prevent crash caused by posibly using a negetive index value
            if (double.IsNaN(MA.Result[tb]))
                return;
            //** skip printing bar until moving average data is calculated
            double close0 = MarketSeries.Close[t0];
            decimal MAt0 = (decimal)MA.Result[t0];
            decimal MAtb = (decimal)MA.Result[tb];
            decimal maslope0 = decimal.Round((MAt0 - MAtb) / ((decimal)Symbol.PipSize * paramBackstep), 2);
//Print("{0,20}{1,20}{2,20}{3,20}",MarketSeries.OpenTime[t0].AddHours(2).ToString("MM/dd/yyyy HH:mm"),MAt0, MAt1,maslope0);

            Result[index] = (double)(maslope0);

            int i = 1;
            double divisor;
            while (Result[index - i] == 0)
            {
                i++;
            }
            divisor = Result[index - i];

            double SlopeRatio = Math.Round(Result[index] / divisor, 2);

            if (SlopeRatio < 0)
            {
                MADiff = Math.Round((MA.Result[t0] - LastPeakMA) / Symbol.PipSize, 2);
                CloseDiff = LastPeakClose != 0 ? Math.Round((MarketSeries.Close[index] - LastPeakClose) / Symbol.PipSize, 2) : 0;
                if (MADiff < -paramMATolerance || MADiff > paramMATolerance || double.IsNaN(MADiff))
                {
                    Xsignal[index] = Result[index] > 0 ? -paramSlopeLimit : paramSlopeLimit;
                    XsignalPoints[index] = Xsignal[index];
                    LastSlopePeak = paramSlopePeakTolerance;
                    //Reset after a peak
                    LastSlopeValley = -paramSlopePeakTolerance;
                    LastPeakClose = close0;
                    //set the closing price at the last MA peak 
                    LastPeakMA = (double)(MAt0);
                    //set the last MA peak
                    PeakCount++;
                    //Print("PeakCount: "+PeakCount); //for testing
                    ChartObjects.DrawText("MADiffs" + index, Convert.ToString(MADiff), index, Xsignal[index], Result[index] <= 0 ? VerticalAlignment.Top : VerticalAlignment.Bottom, HorizontalAlignment.Center, Colors.Blue);
                    //ChartObjects.DrawText("CloseDiffs" + index, Convert.ToString(CloseDiff), index, Xsignal[index], Result[index]<=0?VerticalAlignment.Top:VerticalAlignment.Bottom, HorizontalAlignment.Center, Colors.Red);
                }
            }
            if (Result[t1] > LastSlopePeak && ((Result[t3] < Result[t2] && Result[t2] == Result[t1]) || Result[t2] < Result[t1]) && Result[t1] > Result[index])
            {
                Peak[index] = Result[t1];
                // Set a peak
                LastSlopePeak = Result[t1];
            }

            if (Result[t1] < LastSlopeValley && ((Result[t3] > Result[t2] && Result[t2] == Result[t1]) || Result[t2] > Result[t1]) && Result[t1] < Result[index])
            {
                Valley[index] = Result[t1];
                // Set a valley
                LastSlopeValley = Result[t1];
            }

            if (Result.Maximum(12) < paramSlopePeakTolerance && Result.Minimum(12) > -paramSlopePeakTolerance)
                FlatSignal[index] = 0;

            UpperLimit[index] = paramSlopeLimit;
            LowerLimit[index] = -paramSlopeLimit;
            CenterLine[index] = 0;
        }
    }
}
