// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//    Please use the “Feedback” tab to provide us with your suggestions about cAlgo’s API.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class BreakthroughBB : Robot
    {
        
        [Parameter("PeriodMALong",DefaultValue=50)] // Период средней, необходимой для определения тренда
        public int Period_MA_Long { get; set; }
        
        [Parameter("PeriodBB",DefaultValue=22)] // Период средней болинджера
        public int Period_BB { get; set; }
        
        [Parameter("Deviation",DefaultValue=3)] // Девиация болинджер бандс
        public double Deviation { get; set; }
        
        [Parameter("Reserve",DefaultValue=200)] // отступ (в пунктах) от границ болинджер бандс для установки стоп лоса
        public int Reserve { get; set; }
        
        [Parameter("Volume",DefaultValue=10000)] // объем для открываемой позиции
        public int vol { get; set; }
        
        private BollingerBands bb;
        private MovingAverage sma;
        private Position pos;
        private bool IsOpenPos=false;
        
        protected override void OnStart()
        {
            sma=Indicators.MovingAverage(MarketSeries.Close,Period_MA_Long,MovingAverageType.Simple);
            bb=Indicators.BollingerBands(MarketSeries.Close,Period_BB,Deviation, MovingAverageType.Simple);
        }
        
        protected override void OnTick()
        {
            int last=MarketSeries.Close.Count-1;
            if(!(MarketSeries.Open[last]==MarketSeries.High[last] && MarketSeries.Open[last]==MarketSeries.Low[last])) return; // проверка на начало бара
            // Закрытие позиции
            if(IsOpenPos)
            {
            	if((pos.TradeType==TradeType.Buy && MarketSeries.Close[last-1]<bb.Main[last-1]) || (pos.TradeType==TradeType.Sell && MarketSeries.Close[last-1]>bb.Main[last-1]))
            		Trade.Close(pos);
            }
            // открытие пщзиций
            if(!IsOpenPos)
            {
            	// открытие длинной позиции
            	if(MarketSeries.Close[last-2]<bb.Top[last-1] && MarketSeries.Close[last-1]>bb.Top[last-1] && sma.Result[last-1]>sma.Result[last-4])
            	{	
            		Trade.CreateBuyMarketOrder(Symbol,vol);
            		IsOpenPos=true;
            	}
            	// открытие короткой позиции
            	if(MarketSeries.Close[last-2]>bb.Bottom[last-1] && MarketSeries.Close[last-1]<bb.Bottom[last-1] && sma.Result[last-1]<sma.Result[last-4])
            	{
            		Trade.CreateSellMarketOrder(Symbol,vol);
            		IsOpenPos=true;
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
            if(pos.TradeType==TradeType.Buy)
            	Trade.ModifyPosition(pos,bb.Bottom[bb.Bottom.Count-2]-Reserve*Symbol.PointSize,0);
            if(pos.TradeType==TradeType.Sell)
            	Trade.ModifyPosition(pos,bb.Top[bb.Top.Count-2]+Reserve*Symbol.PointSize,0);
        }
        
        protected override void OnPositionClosed(Position position)
        {
        	pos=null;
        	IsOpenPos=false;
        }
    }
}