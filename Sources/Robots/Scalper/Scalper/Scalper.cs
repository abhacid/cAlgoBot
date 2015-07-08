// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//    Please use the “Feedback” tab to provide us with your suggestions about cAlgo’s API.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using System.Collections.Generic;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class Scalper : Robot
    {
        [Parameter("BarCount",DefaultValue=4)]
        public int BarCount { get; set; }
        
        [Parameter("MaxOrders",DefaultValue=10)]
        public int MaxOrders { get; set; }
        
        [Parameter("TakeProfit",DefaultValue=100)]
        public int TakeProfit { get; set; }
        
        [Parameter("StopLoss",DefaultValue=100)]
        public int StopLoss { get; set; }
        
        [Parameter("Volume",DefaultValue=10000)]
        public int Volume { get; set; }
        
        [Parameter("MaxDropDown",DefaultValue=0)]
        public double MaxDropDown { get; set; }
        
        [Parameter("MaxProfit",DefaultValue=0)]
        public double MaxProfit { get; set; }
        
        private int PosOpen=0;
        private int OpenIndex=0;
        private double StartBalanse;
        private DateTime dt;
        
        protected override void OnStart()
        {
            StartBalanse=Account.Balance;
            dt=Server.Time;
        }

        protected override void OnTick()
        {
            int last=MarketSeries.Close.Count-1;
            if(!(MarketSeries.Open[last]==MarketSeries.High[last] && MarketSeries.Open[last]==MarketSeries.Low[last])) return;
            if(dt.Date!=Server.Time.Date)
            {
            	StartBalanse=Account.Balance;
            	dt=Server.Time;
            }
            
            double bp=(StartBalanse-Account.Balance)/(StartBalanse/100);
            if(bp>0 && bp>=MaxDropDown && MaxDropDown!=0) return;
            if(bp<0 && Math.Abs(bp)>=MaxProfit && MaxProfit!=0) return;
            if(BarCount<1)
            {
              Print("Мало баров для анализа тренда. BarCount должно быть больше или равно 1");
              return;
            }
            if(PosOpen<MaxOrders)
            {
      			if(OpenIndex==0 || last-OpenIndex>BarCount)
      			{
            		if(IsBuy(last))
            		{
            			Trade.CreateBuyMarketOrder(Symbol,Volume);
            			PosOpen++;
            			OpenIndex=last;
            		}
            		if(IsSell(last))
            		{
            			Trade.CreateSellMarketOrder(Symbol,Volume);
            			PosOpen++;
            			OpenIndex=last;
            		}
            	}
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
        
        protected override void OnPositionOpened(Position openedPosition)
        {
            if(openedPosition.TradeType==TradeType.Buy)
            	Trade.ModifyPosition(openedPosition,Symbol.Ask-StopLoss*Symbol.PointSize,Symbol.Ask+TakeProfit*Symbol.PointSize);
            if(openedPosition.TradeType==TradeType.Sell)
            	Trade.ModifyPosition(openedPosition,Symbol.Bid+StopLoss*Symbol.PointSize,Symbol.Bid-TakeProfit*Symbol.PointSize);
        }
        
        protected override void OnPositionClosed(Position position)
        {
        	PosOpen--;
        	if(PosOpen<0) PosOpen=0;
        }
        
        private bool IsBuy(int last)
        {
        	
        	for(int i=BarCount;i>0;i--)
        	{
        		if(MarketSeries.Open[last-i]<MarketSeries.Close[last-i]) return false;
        		if(i<2) continue;
        		if(MarketSeries.High[last-i]>MarketSeries.High[last-i-1]) return false;
        	}
        	return true;
        }
        
        private bool IsSell(int last)
        {
        	
        	for(int i=BarCount;i>0;i--)
        	{
        		if(MarketSeries.Open[last-i]>MarketSeries.Close[last-i]) return false;
        		if(i<2) continue;
        		if(MarketSeries.Low[last-i]<MarketSeries.Low[last-i-1]) return false;
        	}
        	return true;
        }
    }
}