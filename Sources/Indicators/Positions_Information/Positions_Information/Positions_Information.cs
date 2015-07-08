
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class Positions_Information : Indicator
    {
//-----------------------------------------------------------------
    private double sumBuy, sumSell, SumTTL;
    private double countBuy,countSell,countTTL;   
//-----------------------------------------------------------------
    public override void Calculate(int index)
    {    
    sumBuy = 0;
    sumSell = 0;
    countBuy = 0;
    countSell = 0;
    
    var positionsBuy = Positions.FindAll("", Symbol, TradeType.Buy);
    foreach (var position in positionsBuy)
    {
        sumBuy += position.NetProfit;
        countBuy += position.Volume;
    }
        
    var positionsSell = Positions.FindAll("", Symbol, TradeType.Sell);
    foreach (var position in positionsSell)
    {
        sumSell += position.NetProfit;
        countSell += position.Volume;
    }

    SumTTL = sumBuy + sumSell;
    countTTL = countBuy + countSell;

    Colors SpreadColor = Symbol.Spread < 4*Symbol.PipSize ? Colors.LimeGreen : Colors.Red; 

// Column Header
    string strHeaderC = string.Format("{0,20}","Volume");
    ChartObjects.DrawText("strHeaderC",strHeaderC,StaticPosition.TopLeft, Colors.Yellow);    
    string strHeaderS = string.Format("{0,40}","Net P/L");
    ChartObjects.DrawText("strHeaderS",strHeaderS,StaticPosition.TopLeft, Colors.Yellow);

//Row Header
    string strB = string.Format("\n{0,0}","Buy: ");
    string strS = string.Format("\n\n{0,0}","Sell: ");
    string strL = string.Format("\n\n\n{0,0}","-----------------------------------");
    string strT = string.Format("\n\n\n\n{0,0}","Total: ");
    
    ChartObjects.DrawText("strB",strB,StaticPosition.TopLeft, Colors.Yellow);
    ChartObjects.DrawText("strS",strS,StaticPosition.TopLeft, Colors.Yellow);
    ChartObjects.DrawText("strL",strL,StaticPosition.TopLeft, Colors.Yellow);
    ChartObjects.DrawText("strT",strT,StaticPosition.TopLeft, Colors.Yellow);

//Content
    string strBC = string.Format("\n{0,20}",countBuy.ToString());
    string strSC = string.Format("\n\n{0,20}",countSell.ToString());
    string strTC = string.Format("\n\n\n\n{0,20}",countTTL.ToString());           
    ChartObjects.DrawText("strBC",strBC,StaticPosition.TopLeft,Colors.White);
    ChartObjects.DrawText("strSC",strSC,StaticPosition.TopLeft, Colors.White);
    ChartObjects.DrawText("strTC",strTC,StaticPosition.TopLeft, Colors.LimeGreen);
    
    string strBS = string.Format("\n{0,40}",Math.Round(sumBuy).ToString());
    string strSS = string.Format("\n\n{0,40}",Math.Round(sumSell).ToString());
    string strTS = string.Format("\n\n\n\n{0,40}",Math.Round(SumTTL).ToString());           
    ChartObjects.DrawText("strBS",strBS,StaticPosition.TopLeft,Colors.White);
    ChartObjects.DrawText("strSS",strSS,StaticPosition.TopLeft, Colors.White);
    ChartObjects.DrawText("strTS",strTS,StaticPosition.TopLeft, Colors.LimeGreen);        
   	}
//-----------------------------------------------------------------      	
    }   
}
