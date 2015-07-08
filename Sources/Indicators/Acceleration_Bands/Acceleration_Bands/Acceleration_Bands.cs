using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Acceleration_Bands : Indicator
    {
        [Parameter(DefaultValue = 0.0005)]
        public double Factor { get; set; }
        
        [Parameter(DefaultValue = 25)]
        public int Period { get; set; }
        [Output("UpperBand")]
        public IndicatorDataSeries upperband { get; set; }

        [Output("LowerBand")]
        public IndicatorDataSeries lowerband { get; set; }

    	double sumupper;
		double sumlower;
		public IndicatorDataSeries upperbandholder;
		public IndicatorDataSeries lowerbandholder;
		protected override void Initialize()
        {
        	upperbandholder = CreateDataSeries();
        	lowerbandholder = CreateDataSeries();
		}
		
        public override void Calculate(int index)
        {
			upperbandholder[index] = (MarketSeries.High[index] * ( 1 + 2 * (((( MarketSeries.High[index] - MarketSeries.Low[index] )
			/(( MarketSeries.High[index] + MarketSeries.Low[index] ) / 2 )) * 1000 ) * Factor )));
			lowerbandholder[index]= ( MarketSeries.Low[index] * ( 1 - 2 * (((( MarketSeries.High[index] - MarketSeries.Low[index] )
			/(( MarketSeries.High[index] + MarketSeries.Low[index] ) / 2 )) * 1000 ) * Factor )));
			sumupper=0;
			sumlower=0;
			for(int i=0;i<Period;i++)
			{
				sumupper+=upperbandholder[index-i];
				sumlower+=lowerbandholder[index-i];
			}
			sumupper=sumupper/Period;
			sumlower=sumlower/Period;
			
			lowerband[index]=sumlower;
			upperband[index]=sumupper;
        }
    }
}