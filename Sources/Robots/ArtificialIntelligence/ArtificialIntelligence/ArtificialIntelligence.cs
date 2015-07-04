using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class ArtificialIntelligence : Robot
    {
        [Parameter("x1",DefaultValue=281)]
        public int x1 { get; set; }
        
        [Parameter("x2",DefaultValue=100)]
        public int x2 { get; set; }

        [Parameter("x3",DefaultValue=794)]
        public int x3 { get; set; }
        
        [Parameter("x4",DefaultValue=566)]
        public int x4 { get; set; }
        
        [Parameter("FastMA",DefaultValue=20)]
        public int FastMA { get; set; }
        
        [Parameter("SlowMA",DefaultValue=21)]
        public int SlowMA { get; set; }
        
        [Parameter("Step",DefaultValue=28)]
        public int Step { get; set; }
        
        [Parameter("StopLoss",DefaultValue=550)]
        public int StopLoss { get; set; }
        
        [Parameter("Volume",DefaultValue=10000)]
        public int Volume { get; set; }
        
        private MacdHistogram macd;
        private Position pos;
        private bool IsPosOpen=false;
        private double sl;
        protected override void OnStart()
        {
            macd=Indicators.MacdHistogram(SlowMA,FastMA,3);
        }

        protected override void OnTick()
        {
            int last=MarketSeries.Close.Count-1;
            if(!(MarketSeries.Open[last]==MarketSeries.High[last] && MarketSeries.Open[last]==MarketSeries.Low[last])) return;
            double per=percertron();
            if(!IsPosOpen)
            {
 				if(per>0) {Trade.CreateBuyMarketOrder(Symbol,Volume); IsPosOpen=true;}
 				if(per<0) {Trade.CreateSellMarketOrder(Symbol,Volume); IsPosOpen=true;}
            }
            else
            {
            	if(pos.TradeType==TradeType.Buy && per<0)
            	{
            		Trade.Close(pos);
            		Trade.CreateSellMarketOrder(Symbol,Volume);
            		return;
            	}
            	else
            	{
            		if(Symbol.Ask>sl+StopLoss*2*Symbol.PointSize) Trade.ModifyPosition(pos,Symbol.Ask-StopLoss*Symbol.PointSize,0);
            	}
            	if(pos.TradeType==TradeType.Sell && per>0)
            	{
            		Trade.Close(pos);
            		Trade.CreateBuyMarketOrder(Symbol,Volume);
            	}
            	else
            	{
            		if(Symbol.Bid<sl-StopLoss*2*Symbol.PointSize) Trade.ModifyPosition(pos,Symbol.Bid+StopLoss*Symbol.PointSize,0);
            	}
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
        
        protected override void OnPositionOpened(Position openedPosition)
        {
            pos=openedPosition;
            if(openedPosition.TradeType==TradeType.Buy)
            {
            	sl=Symbol.Ask-StopLoss*Symbol.PointSize;
            	Trade.ModifyPosition(openedPosition,sl,0);
           	}
            if(openedPosition.TradeType==TradeType.Sell)
            {
            	sl=Symbol.Bid+StopLoss*Symbol.PointSize;
            	Trade.ModifyPosition(openedPosition,sl,0);
           	}
        }
        
        private double percertron()
        {
        	int last=MarketSeries.Close.Count-1;
        	double w1=x1-100;
        	double w2=x2-100;
        	double w3=x3-100;
        	double w4=x4-100;
        	double a1=macd.Histogram[last-1];
        	double a2=macd.Histogram[last-1-Step];
        	double a3=macd.Histogram[last-1-Step*2];
        	double a4=macd.Histogram[last-1-Step*3];
        	return w1*a1+w2*a2+w3*a3+w4*a4;
        }
    }
}