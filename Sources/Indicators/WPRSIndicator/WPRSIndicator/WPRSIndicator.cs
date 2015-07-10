
#region Licence
//The MIT License (MIT)
//Copyright (c) 2014 abdallah HACID, https://www.facebook.com/ab.hacid

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//and associated documentation files (the "Software"), to deal in the Software without restriction,
//including without limitation the rights to use, copy, modify, merge, publish, distribute,
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
//is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or
//substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
//BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/Robot_Forex
#endregion

#region Description
// -------------------------------------------------------------------------------
//				WPRSIndicator
// -------------------------------------------------------------------------------

//		Williams Percent Range with signals (18 juillet 2014)
//		version 1.2014.7.18.11h
//		Author : https://www.facebook.com/ab.hacid
//
//	Utiliser :
//			WPR Source						=	Open					
//			WPR Period						=   17
//			WPR Overbuy Ceil				=	-20						//	Seuil d'oversell
//			WPR Oversell Ceil				=	-80						//	Seuil d'overbuy
//			WPR Crossed Period				=	2						//	Permet d'etendre le temps de detection du signal et cree plus de signaux (Magic)
//			WPR Min/Max Period				=	114						//	Periode pendant laquelle on calcule le minimum et le maximum pour detecter l'etendue du range
//			WPR Exceed MinMax				=	2						//	Decalage par rapport au Minimum et au Maximum pour cloturer les positions

//
//	L'indicateur Williams Percent Range avec signaux d'achat ou de vente.
//	Les deux premiers signaux testent le franchissement des seuils de survente (-80) et surachat (-20) tandis que les deux
//	suivants testent une limite haute et basse basee sur un range calcule sur "WPR Min/Max Period" periodes. 
#endregion
//The MIT License (MIT)
//Copyright (c) 2014 abdallah HACID, https://www.facebook.com/ab.hacid

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//and associated documentation files (the "Software"), to deal in the Software without restriction,
//including without limitation the rights to use, copy, modify, merge, publish, distribute,
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
//is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or
//substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
//BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/cAlgoBot 

using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Levels(-0, -20, -50, -80, -100)]
    [Indicator(ScalePrecision = 2, AccessRights = AccessRights.None)]
    public class WPRSIndicator : Indicator
    {
        #region Parameters
        [Parameter("WPR Source")]
        public DataSeries Source { get; set; }

        [Parameter("WPR Period", DefaultValue = 17)]
        public int WprPeriod { get; set; }

        [Parameter("Overbuy Ceil", DefaultValue = -20, MinValue = -100, MaxValue = 0)]
        public int OverbuyCeil { get; set; }

        [Parameter("Oversell Ceil", DefaultValue = -80, MinValue = -100, MaxValue = 0)]
        public int OversellCeil { get; set; }

        [Parameter("Crossed Period", DefaultValue = 2, MinValue = 0)]
        public int CrossedPeriod { get; set; }

        [Parameter("Min/Max Period", DefaultValue = 114)]
        public int MinMaxPeriod { get; set; }

        [Parameter("Exceed Min/Max", DefaultValue = 2)]
        public int ExceedMinMax { get; set; }


        [Output("WPR", Color = Colors.SkyBlue, Thickness = 2)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Cross Above Oversell Ceil", Color = Colors.Green, Thickness = 2)]
        public IndicatorDataSeries CrossAboveOversellSignal { get; set; }

        [Output("Cross Below Overbuy Ceil", Color = Colors.Red, Thickness = 2)]
        public IndicatorDataSeries CrossBelowOverbuySignal { get; set; }

        [Output("Exceed Minimum", Color = Colors.GreenYellow, Thickness = 2)]
        public IndicatorDataSeries ExceedMinimumSignal { get; set; }

        [Output("Exceed Maximum", Color = Colors.OrangeRed, Thickness = 2)]
        public IndicatorDataSeries ExceedMaximumSignal { get; set; }


        #endregion

        #region Globals
        const int basePlot = -125;
        const int sizeSignalPlot = 20;
        const int spaceBetweenPlot = 2 * sizeSignalPlot + 5;

        //Buy
        public const int wprCrossAboveOversellSignal = basePlot + sizeSignalPlot;
        public const int wprCrossAboveOversellNeutralSignal = basePlot;
        public const int wprCrossAboveOversellCloseSignal = basePlot - sizeSignalPlot;

        //Sell
        public const int wprCrossBelowOverbuySignal = basePlot - spaceBetweenPlot - sizeSignalPlot;
        public const int wprCrossBelowOverbuyNeutralSignal = basePlot - spaceBetweenPlot;
        public const int wprCrossBelowOverbuyCloseSignal = basePlot - spaceBetweenPlot + sizeSignalPlot;

        //Buy
        public const int wprExceedLowSignal = basePlot - 2 * spaceBetweenPlot + sizeSignalPlot;
        public const int wprExceedLowNeutralSignal = basePlot - 2 * spaceBetweenPlot;
        public const int wprExceedLowCloseSignal = basePlot - 2 * spaceBetweenPlot - sizeSignalPlot;

        //Sell
        public const int wprExceedHighSignal = basePlot - 3 * spaceBetweenPlot - sizeSignalPlot;
        public const int wprExceedHighNeutralSignal = basePlot - 3 * spaceBetweenPlot;
        public const int wprExceedHighCloseSignal = basePlot - 3 * spaceBetweenPlot + sizeSignalPlot;
        #endregion

        #region Predicate

        public bool IsCrossBelowOverbuy
        {
            //Sell Signal
            get { return (CrossBelowOverbuySignal.LastValue < WPRSIndicator.wprCrossBelowOverbuyNeutralSignal); }
        }

        public bool IsCrossAboveOversell
        {
            //Buy Signal
            get { return (CrossAboveOversellSignal.LastValue > WPRSIndicator.wprCrossAboveOversellNeutralSignal); }
        }

        public bool IsExceedHigh
        {
            //Sell Signal
            get { return (ExceedMaximumSignal.LastValue < WPRSIndicator.wprExceedHighNeutralSignal); }
        }

        public bool IsExceedLow
        {
            //Buy Signal
            get { return (ExceedMinimumSignal.LastValue > WPRSIndicator.wprExceedLowNeutralSignal); }
        }

        #endregion

        public override void Calculate(int index)
        {
            double max = MarketSeries.High.Maximum(WprPeriod);
            double min = MarketSeries.Low.Minimum(WprPeriod);
            double trigger = Source[index];

            double wprMax = Result.Maximum(MinMaxPeriod);
            double wprMin = Result.Minimum(MinMaxPeriod);

            //WPR Signal
            if ((max - min) > 0)
                Result[index] = -100 * (max - trigger) / (max - min);
            else
                Result[index] = 0.0;

            // Cross above oversell limit (Buy)
            if (Result.HasCrossedAbove(OversellCeil, CrossedPeriod))
                CrossAboveOversellSignal[index] = wprCrossAboveOversellSignal;
            else
                CrossAboveOversellSignal[index] = wprCrossAboveOversellNeutralSignal;

            // Cross below overbuy limit (Sell)
            if (Result.HasCrossedBelow(OverbuyCeil, CrossedPeriod))
                CrossBelowOverbuySignal[index] = wprCrossBelowOverbuySignal;
            else
                CrossBelowOverbuySignal[index] = wprCrossBelowOverbuyNeutralSignal;

            //Exceed low limit (Buy)
            if (Result.HasCrossedBelow(wprMin + ExceedMinMax, CrossedPeriod))
                ExceedMinimumSignal[index] = wprExceedLowSignal;
            else
                ExceedMinimumSignal[index] = wprExceedLowNeutralSignal;

            //Exceed Hight Limit (Sell)
            if (Result.HasCrossedAbove(wprMax - ExceedMinMax, CrossedPeriod))
                ExceedMaximumSignal[index] = wprExceedHighSignal;
            else
                ExceedMaximumSignal[index] = wprExceedHighNeutralSignal;




        }
    }
}

