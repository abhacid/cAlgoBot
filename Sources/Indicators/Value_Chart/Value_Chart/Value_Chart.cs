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
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class Value_Chart : Indicator
    {
        [Parameter(DefaultValue = 5)]
        public int Period { get; set; }
        
  
        
        [Output("UpCandle", IsHistogram = true,  Color = Colors.Green)]
        public IndicatorDataSeries UpCandle { get; set; }  
        
        [Output("UpCandleBack", IsHistogram = true,  Color = Colors.Black)]
        public IndicatorDataSeries UpCandleBack { get; set; }
        
        [Output("UpShadow", IsHistogram = true,  Color = Colors.Green)]
        public IndicatorDataSeries UpShadow { get; set; }
        
        [Output("DownCandle", IsHistogram = true,  Color = Colors.Red)]
        public IndicatorDataSeries DownCandle { get; set; }  
        
        
        [Output("DownCandleBack", IsHistogram = true,  Color = Colors.Black)]
        public IndicatorDataSeries DownCandleBack { get; set; }
        
        [Output("DownShadow", IsHistogram = true,  Color = Colors.Red)]
        public IndicatorDataSeries DownShadow { get; set; }
        
        
        [Output("ShadowBack", IsHistogram = true,  Color = Colors.Black)]
        public IndicatorDataSeries ShadowBack { get; set; }
        
       
        [Output("UpNiveau", Color = Colors.White, LineStyle = LineStyle.LinesDots)]
        public IndicatorDataSeries UpNiveau { get; set; }
        
        [Output("DownNiveau", Color = Colors.White , LineStyle = LineStyle.LinesDots)]
        public IndicatorDataSeries DownNiveau { get; set; }
        
        [Output("UpNiveau2", Color = Colors.White, LineStyle = LineStyle.LinesDots)]
        public IndicatorDataSeries UpNiveau2 { get; set; }
        
        [Output("DownNiveau2", Color = Colors.White,  LineStyle = LineStyle.LinesDots)]
        public IndicatorDataSeries DownNiveau2 { get; set; }
        
        // Local Variable
        private IndicatorDataSeries Range;   
        private IndicatorDataSeries _Avalue;
        
        private IndicatorDataSeries MiddleRange;
        private MovingAverage MovRange;
        private MovingAverage _Bvalue;


        protected override void Initialize()
        {
            // Initialize and create nested indicators
            Range       = CreateDataSeries();
            _Avalue     = CreateDataSeries();
            MiddleRange = CreateDataSeries();
            
            // Initialize the Monving Average
            MovRange = Indicators.MovingAverage(Range, Period, MovingAverageType.Simple);
            _Bvalue  = Indicators.MovingAverage(MiddleRange, Period, MovingAverageType.Simple);
            
        }

        public override void Calculate(int index)
        {
            /** Create a defaut value **/
            UpCandle[index]       = 0.0;
            UpCandleBack[index]   = 0.0;
            UpShadow[index]       = 0.0;
            DownCandle[index]     = 0.0;
            DownCandleBack[index] = 0.0;
            DownShadow[index]     = 0.0;
            ShadowBack[index]     = 0.0;
            
            /*********/
            Range[index] = MarketSeries.High[index]-MarketSeries.Low[index];
            MovRange.Calculate(index);
            
            ///
            _Avalue[index] = 0.2*MovRange.Result[index];
            
            ///
            MiddleRange[index]  =  (MarketSeries.High[index]+MarketSeries.Low[index])/2;
            _Bvalue.Calculate(index);
            
            DownNiveau[index]  = 7.0;
            UpNiveau[index]    = 23.0;
            DownNiveau2[index] = 9.0;
            UpNiveau2[index]   = 21.0;
            
            if(MarketSeries.Close[index]>MarketSeries.Open[index])
                {
            		UpCandle[index] = 15+((MarketSeries.Close[index]-_Bvalue.Result[index])/_Avalue[index]);
            		UpCandleBack[index] = 15+((MarketSeries.Open[index]-_Bvalue.Result[index])/_Avalue[index]);
            		UpShadow[index] =  15+((MarketSeries.High[index]-_Bvalue.Result[index])/_Avalue[index]);
            	}
            else
				{
					DownCandle[index] = 15+((MarketSeries.Open[index]-_Bvalue.Result[index])/_Avalue[index]);
            		DownCandleBack[index] = 15+((MarketSeries.Close[index]-_Bvalue.Result[index])/_Avalue[index]);
            		DownShadow[index] =  15+((MarketSeries.High[index]-_Bvalue.Result[index])/_Avalue[index]);
				}
           
            ShadowBack[index] =  15+((MarketSeries.Low[index]-_Bvalue.Result[index])/_Avalue[index]);           
                    
        }
    }
}