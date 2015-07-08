// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//    Please use the “Feedback” tab to provide us with your suggestions about cAlgo’s API.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class DynamicRangeIndicator : Indicator
    {    	    	
        [Parameter("Period", DefaultValue = 40)]
        public int Period { get; set; }		
        [Parameter("Levels of History", DefaultValue = 300)]
        public int LevelH { get; set; }		
        [Output("Sell Signal Line", Color = Colors.Red, IsHistogram = true)]
        public IndicatorDataSeries SellSignal { get; set; }        
		[Output("Buy Signal Lint", Color = Colors.Blue, IsHistogram = true)]
        public IndicatorDataSeries BuySignal { get; set; }
                
        private IndicatorDataSeries g_ibuf_156 { get; set; }
        private IndicatorDataSeries g_ibuf_160 { get; set; }
        private IndicatorDataSeries g_ibuf_152 { get; set; }
        private IndicatorDataSeries g_ibuf_164 { get; set; }
				
        protected override void Initialize()
        {
           g_ibuf_156 = CreateDataSeries();
           g_ibuf_160 = CreateDataSeries();
           g_ibuf_152 = CreateDataSeries();
           g_ibuf_164 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {        	
        	int iSeriesCloseCnt = MarketSeries.Close.Count-1;
        	double ld_0,  ld_8, ld_16, ld_24, ld_32, ld_40, ld_48;
            if(index == iSeriesCloseCnt) {	            
	            int iIndexMinusPeriod = index-Period;
	            //--
	           for (int l_index_56 = index-LevelH; l_index_56 < index; l_index_56++) {
			      ld_0 = 0;
			      ld_8 = 0;
			      ld_16 = 0;
			      ld_24 = 0;
			      
			      for (int l_count_60 = 0; l_count_60 < Period; l_count_60++) {
			      
	   			      ld_0  +=  MarketSeries.Close[l_index_56 - l_count_60];	   			      
			          ld_8  += (MarketSeries.Close[l_index_56 - l_count_60]) * l_count_60;
			          ld_16 += l_count_60;
			          ld_24 += l_count_60 * l_count_60;
			          
			      }			      
			      		      
			      ld_48 = ld_24 * Period - ld_16 * ld_16;
			      ld_40 = (ld_8 * Period - ld_16 * ld_0) / ld_48;
			      ld_32 = (ld_0 - ld_16 * ld_40) / Period;
			      			      			      
			      g_ibuf_156[l_index_56] = MyLinRegr(true , ld_32, ld_40, ld_48, l_index_56);
			      g_ibuf_160[l_index_56] = MyLinRegr(false, ld_32, ld_40, ld_48, l_index_56);
				  
				 
			      if (g_ibuf_160[l_index_56] - g_ibuf_156[l_index_56] != 0.0) g_ibuf_152[l_index_56] = 100.0 * ((MarketSeries.Close[l_index_56] - g_ibuf_156[l_index_56]) / (g_ibuf_160[l_index_56] - g_ibuf_156[l_index_56]));
			      else g_ibuf_152[l_index_56] = 50;
			      
			      if (g_ibuf_160[l_index_56 - 5] - (g_ibuf_156[l_index_56 - 5]) != 0.0) g_ibuf_164[l_index_56] = 100.0 * ((MarketSeries.Close[l_index_56] - (g_ibuf_156[l_index_56 - 5])) / (g_ibuf_160[l_index_56 - 5] - (g_ibuf_156[l_index_56 - 5])));
			      else g_ibuf_164[l_index_56] = 50;
			   }
			   
			   for (int l_index_56 = index; l_index_56 > index-LevelH; l_index_56--) {
			      if (g_ibuf_152[l_index_56] > 10.0 && g_ibuf_152[l_index_56 - 1] <= 10.0) BuySignal [l_index_56] = 50;
			      if (g_ibuf_152[l_index_56] < 90.0 && g_ibuf_152[l_index_56 - 1] >= 90.0) SellSignal[l_index_56] = 50;
			   }			   
			}          
        }  
		
		
		
		double MyLinRegr(bool ai_0, double ad_4, double ad_12, double ad_unused_20, int ai_28, int ai_32 = 0) {
			   double[] lda_52 = new double[500];
			   double[] lda_56 = new double[500];
			   double[] lda_60 = new double[500];
			   double ld_36 = 0.0;
			   double ld_44 = 0.0;
			   //--
			   for (int l_index_64 = 0; l_index_64 < Period; l_index_64++) lda_52[l_index_64] = ad_4 + ad_12 * l_index_64;
			   //--
			   for (int l_index_64 = 0; l_index_64 < Period; l_index_64++) {
			      if (MarketSeries.Close[ai_28 - l_index_64 - ai_32] - lda_52[l_index_64] > ld_36) ld_36 = MarketSeries.Close[ai_28 - l_index_64 - ai_32] - lda_52[l_index_64];
			      if (lda_52[l_index_64] - (MarketSeries.Close[ai_28 - l_index_64 - ai_32]) > ld_44) ld_44 = lda_52[l_index_64] - (MarketSeries.Close[ai_28 - l_index_64 - ai_32]);
			   }
			   //--
			   if (ld_36 > ld_44) {
			      for (int l_index_64 = 0; l_index_64 < Period; l_index_64++) {
			         lda_56[l_index_64] = ad_4 - ld_36 + ad_12 * l_index_64;
			         lda_60[l_index_64] = ad_4 + ld_36 + ad_12 * l_index_64;
			      }
			   } else {
			      for (int l_index_64 = 0; l_index_64 < Period; l_index_64++) {
			         lda_56[l_index_64] = ad_4 - ld_44 + ad_12 * l_index_64;
			         lda_60[l_index_64] = ad_4 + ld_44 + ad_12 * l_index_64;
			      }
			   }
			   if (ai_0) return (lda_56[0]);
			   return (lda_60[0]);
			}		
        
    }
}