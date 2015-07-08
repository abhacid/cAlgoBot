// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//    Please use the “Feedback” tab to provide us with your suggestions about cAlgo’s API.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Triggerlines : Indicator
    {
        [Parameter(DefaultValue = 0.0)]
        public double Parameter { get; set; }
        

       #region indicator line
       
        
        [Output("Up", Color = Colors.Lime,PlotType = PlotType.Points)]
        public IndicatorDataSeries UpBuffer { get; set; }
        [Output("UpMa", Color = Colors.Lime, PlotType = PlotType.Points)]
        public IndicatorDataSeries UpBuffer_ma { get; set; }

        [Output("Dn", Color = Colors.Magenta, PlotType = PlotType.Points)]
        public IndicatorDataSeries DnBuffer { get; set; }
        [Output("Dnma", Color = Colors.Magenta, PlotType = PlotType.Points)]
        public IndicatorDataSeries DnBuffer_ma { get; set; }



        #endregion
        private IndicatorDataSeries wt;
        private IndicatorDataSeries lsma_ma;
        private ExponentialMovingAverage _EMA;
        private const int length = 25;
        private const int lsma_length = 13;

        private int shift;
        private int loopbegin;
        private int cnt;
        private int i;
        private double su;
        private double lengthvar;
        private double tmp ;
        
        protected override void Initialize()
        {
            wt = CreateDataSeries();
            lsma_ma = CreateDataSeries();
            _EMA = Indicators.ExponentialMovingAverage(wt, lsma_length);

        }

        public override void Calculate(int index)
        {
            
         /*    DnBuffer[index] = double.NaN;
             DnBuffer_ma[index] = double.NaN;
             UpBuffer[index] = double.NaN;
             UpBuffer_ma[index] = double.NaN;*/

       
            for(cnt = index - 1; cnt <= index; cnt++)
                 { 
                   su = 0;
                   for(i = length; i >= 1 ; i--)
                        {
                           lengthvar = (length + 1);
                           lengthvar /= 3;
                           tmp = 0;
                           tmp = ( i - lengthvar)*MarketSeries.Close[index-length+i];
                           su+=tmp;
                         }
                     wt[cnt] = su*6/(length*(length+1));
                }

     
         lsma_ma[index] =_EMA.Result[index];
                          
//========== COLOR CODING ===========================================                       
        
        
            
            if (wt[index]  < lsma_ma[index] && wt[index-1]  < lsma_ma[index-1])
            {
              DnBuffer[index] = wt[index]; 
              DnBuffer_ma[index] = lsma_ma[index]; 
               UpBuffer[index] = double.NaN;
               UpBuffer_ma[index] = double.NaN;
            }          
            
           if (wt[index]  > lsma_ma[index]&& wt[index-1]  > lsma_ma[index-1])
            {
                UpBuffer[index] = wt[index]; 
                UpBuffer_ma[index] = lsma_ma[index]; 
                DnBuffer[index] = double.NaN;
                DnBuffer_ma[index] = double.NaN;
               
            }  
                  
      }  
    } 
}