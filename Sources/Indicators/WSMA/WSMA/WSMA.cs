// -------------------------------------------------------------------------------
//
//		WSMA
//    	Wilder’s Smoothing AKA Smoothed Moving Average
//		SUM(1) = SUM(CLOSE, N)
//		WSMA(1) = Simple MA = SUM(1)/N -  Wilder’s Smoothing for the first period.
//		WSMA(i) = (SUM(i - 1) - WSMA(i - 1) + CLOSE(i)) / N
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class WSMA : Indicator
    {
        [Output("WSMA", Color = Colors.Maroon, PlotType = PlotType.Line)]
        public IndicatorDataSeries Result { get; set; }

        [Parameter("Data Source")]
        public DataSeries DataSource { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }


        private SimpleMovingAverage sma;
        private IndicatorDataSeries sumBuffer;

        protected override void Initialize()
        {
            sumBuffer = CreateDataSeries();
            sma = Indicators.SimpleMovingAverage(DataSource, Period);
        }

        public override void Calculate(int index)
        {
            var previousValue = Result[index - 1];
            var previousSum = sumBuffer[index - 1];

            if (double.IsNaN(previousValue))
            {
                Result[index] = sma.Result[index];
                sumBuffer[index] = sma.Result[index] * Period;
            }
            else
            {
                sumBuffer[index] = sumBuffer[index - 1] - previousValue + DataSource[index];
                Result[index] = sumBuffer[index] / Period;
            }
        }
    }
}

