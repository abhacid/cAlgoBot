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
    public class FTBollingerBands : Robot
    {
        [Parameter("rsiperiod",DefaultValue=8)]
        public int rsiperiod { get; set; }
        
        [Parameter("bbperiod",DefaultValue=14)]
        public int bbperiod { get; set; }
        
        [Parameter("bbotcl",DefaultValue=1)]
        public int bbotcl { get; set; }
        
        [Parameter("SL",DefaultValue=500)]
        public int SL { get; set; }
        
        [Parameter("TP",DefaultValue=500)]
        public int TP { get; set; }
        
        [Parameter("mn",DefaultValue=10)]
        public int mn { get; set; }
        
        [Parameter("otstup",DefaultValue=105)]
        public int otstup { get; set; }
        
        [Parameter("rsiup",DefaultValue=30)]
        public int rsiup { get; set; }
        
        [Parameter("rsidw",DefaultValue=70)]
        public int rsidw { get; set; }
        
        [Parameter("SARstep",DefaultValue=0.003)]
        public double SARstep { get; set; }
        
        [Parameter("SARmax",DefaultValue=0.2)]
        public double SARmax { get; set; }
        
        [Parameter("SarTrailingStop",DefaultValue=1)]
        public int SarTrailingStop { get; set; }
        
        [Parameter("TrailingStep",DefaultValue=50)]
        public int TrailingStep { get; set; }
        
        [Parameter("Volume",DefaultValue=10000)]
        public int Lot { get; set; }
        
        private FractalChaosBands frc;
        private BollingerBands bb;
        private RelativeStrengthIndex rsi;
        private ParabolicSAR psar;
        private List<Position> pos=new List<Position>();
        private int okbuy=0;
        private int oksell=0;
        private double upfractal=0;
		private double dwfractal=0;
		
        protected override void OnStart()
        {
            bb=Indicators.BollingerBands(MarketSeries.Close,bbperiod,bbotcl, MovingAverageType.Simple);
            rsi=Indicators.RelativeStrengthIndex(MarketSeries.Close,rsiperiod);
            psar=Indicators.ParabolicSAR(SARstep,SARmax);
            frc=Indicators.FractalChaosBands(5);
        }

        protected override void OnTick()
        {
            int last=MarketSeries.Close.Count-1;
            if(!(MarketSeries.Open[last]==MarketSeries.High[last] && MarketSeries.Open[last]==MarketSeries.Low[last])) return;
            Pattern(last);
            
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
        private void Pattern(int last)
        {
        	double op,sl=0,tp=0;
   			double irsi;  
   			double fractal;
   			irsi=rsi.Result[last-1];
   			double bbup=bb.Top[last-1];
   			double bblow=bb.Bottom[last-1];
   			if(frc.High[last-3]!=0) upfractal=frc.High[last-3];
   			if(frc.Low[last-3]!=0) dwfractal=frc.Low[last-3];
   			if(irsi>bbup && MarketSeries.Close[last-1]<upfractal && okbuy==0) 
   			{
   				op=upfractal+otstup*Symbol.PointSize*mn;
   				if(SL>0){sl=op-SL*Symbol.PointSize*mn;}
   				if(TP>0){tp=op+TP*Symbol.PointSize*mn;}
   				
   				Trade.CreateBuyStopOrder(Symbol,Lot,op,sl,tp,null);
   				
   				okbuy=1;
   				Print("OK="+okbuy.ToString());
   			}
   			if(irsi<rsiup && okbuy==1)
   			{
   				for(int i=0;i<Account.PendingOrders.Count;i++)
   				{
   					if(Account.PendingOrders[i].TradeType== TradeType.Buy) Trade.DeletePendingOrder(Account.PendingOrders[i]);
   				}
   				okbuy=0;
  			}
  			
  			if(irsi<bblow && MarketSeries.Close[1]>dwfractal && oksell==0 ) 
  			{
   				op=dwfractal-otstup*Symbol.PointSize*mn;
   				if(SL>0){sl=op+SL*Symbol.PointSize*mn;}
   				if(TP>0){tp=op-TP*Symbol.PointSize*mn;}
   				Trade.CreateSellStopOrder(Symbol,Lot,op,sl,tp,null);
   				oksell=1;
   			}
   			if(irsi>rsidw && oksell==1)
   			{
   				for(int i=0;i<Account.PendingOrders.Count;i++)
   				{
   					if(Account.PendingOrders[i].TradeType== TradeType.Sell) Trade.DeletePendingOrder(Account.PendingOrders[i]);
   				}
   				okbuy=0;
  			}
		}
		
		private void SarTrailingStopf(int last)
		{
			double sar=psar.Result[last-1];
			for(int i=0; i<pos.Count; i++)
			{
				if(SarTrailingStop>0 && pos[i].TradeType== TradeType.Buy)
				{
					if(sar>pos[i].StopLoss)
					{
						if(sar-pos[i].StopLoss>=TrailingStep*Symbol.PointSize*mn)
							Trade.ModifyPosition(pos[i],sar,pos[i].TakeProfit);
					}
				}
				if(SarTrailingStop>0 && pos[i].TradeType== TradeType.Sell)
				{
					if(pos[i].StopLoss>sar)
					{
						if(pos[i].StopLoss-sar>TrailingStep*Symbol.PointSize*mn)
							Trade.ModifyPosition(pos[i],sar,pos[i].TakeProfit);
					}
				}
			}
		}
		
		protected override void OnPositionOpened(Position openedPosition)
		{
			pos.Add(openedPosition);
		}
		
		protected override void OnPositionClosed(Position position)
        {
        	for(int i=0;i<pos.Count;i++)
        		if(pos[i].Id==position.Id)
        		{
        			pos.RemoveAt(i);
        			break;
        		}
        }
    }
}