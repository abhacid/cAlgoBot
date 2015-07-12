// -------------------------------------------------------------------------------
//
//		Linear Weighted Moving Average (LWMA)
//		In the case of weighted moving average, the latest data is of more value than 
//		more early data. 
//		Weighted moving average is calculated by multiplying each one of the closing 
//		prices within the considered series, by a certain weight coefficient.
//    	
//		LWMA = SUM(Close(i)*i, N)/SUM(i, N)
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class LWMA : Indicator
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 25)]
        public int Period { get; set; }

        [Output("Main", Color = Colors.DarkGreen)]
        public IndicatorDataSeries Result { get; set; }


        public override void Calculate(int index)
        {
            double sum = 0.0;
            int weightSum = 0;
            int k = 1;

            for (int i = index - Period + 1; i <= index; i++)
            {
                sum += Source[i] * k;
                weightSum += k;
                k++;
            }

            Result[index] = sum / weightSum;

        }
    }
}
