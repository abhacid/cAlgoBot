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

// Project Hosting for Open Source Software on Codeplex : https://calgobots.codeplex.com/
#endregion

#region cBot Infos
// -------------------------------------------------------------------------------
//
//		Breakout (17 novembre 2013)
//		version 1.0.0.0.4.
//		Author : https://www.facebook.com/ab.hacid
//
// -------------------------------------------------------------------------------
#endregion

#region cBot Parameters Comments
// h4, GBPUSD, Source=Open, SL=37,TP=62,Volume=100k,BBD=1,BBP=26,BBMAType=Wilder Smoothing,Consolidation=0 donne 20943 euros de gain entre 1/1/2014 et 22/7/2014
#endregion

#region advertisement
// -------------------------------------------------------------------------------
//			Trading using leverage carries a high degree of risk to your capital, and it is possible to lose more than
//			your initial investment. Only speculate with money you can afford to lose.
// -------------------------------------------------------------------------------
#endregion

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Lib;

namespace cAlgo.Robots
{
    /// <summary>
    /// The "Sample Breakout Robot" will check the difference in pips between the Upper Bollinger Band and the Lower Bollinger Band 
    /// and compare it against the "Band Height" parameter specified by the user.  If the height  is lower than the number of pips 
    /// specified, the market is considered to be consolidating, and the first candlestick to cross the upper or lower band will 
    /// generate a sell or buy signal respectively. The user can specify the number of periods that the market should be consolidating in the 
    /// "Consolidation Periods" parameter. The position is closed by a Stop Loss or Take Profit.  
    /// </summary>
    [Robot("BreakOut", TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public partial class Breakout : Robot
    {
        #region Properties
        [Parameter("Breakout")]
        public DataSeries Source { get; set; }

        [Parameter("Band Height", DefaultValue = 7.0, MinValue = 0)]
        private double BandHeight { get; set; }

        [Parameter("Stop Loss", DefaultValue = 37)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 62)]
        public int TakeProfit { get; set; }

        [Parameter("Volume", DefaultValue = 100000, MinValue = 0)]
        public int Volume { get; set; }

        [Parameter("Bollinger Bands Deviation", DefaultValue = 1)]
        public int Deviation { get; set; }

        // BB
        [Parameter("Bollinger Bands Periods", DefaultValue = 26)]
        public int Periods { get; set; }

        [Parameter("Bollinger Bands MA Type", DefaultValue = 6)]
        //Wilder Smoothing
        public MovingAverageType MAType { get; set; }

        [Parameter("Consolidation Periods", DefaultValue = 0)]
        public int ConsolidationPeriods { get; set; }

        [Parameter("Delta Bollinger", DefaultValue = 8)]
        private double DeltaBollinger { get; set; }

        #endregion


        // How many periods to consolidate in the channel before the strategy is 'armed'.
        private BollingerBands bollingerBands;
        private int consolidation;
        private int lastTrendBarIndex;


        protected override void OnStart()
        {
            Positions.Opened += OnPositionOpened;

            bollingerBands = Indicators.BollingerBands(Source, Periods, Deviation, MAType);
            BandHeight = (bollingerBands.Top.LastValue - bollingerBands.Bottom.LastValue) / 3;
            DeltaBollinger = BandHeight / 5;
        }

        protected override void OnTick()
        {
            var isNewTrendBar = lastTrendBarIndex < MarketSeries.Close.Count;
            lastTrendBarIndex = MarketSeries.Close.Count;

            if (isNewTrendBar)
            {
                double top = bollingerBands.Top.LastValue + DeltaBollinger;
                double bottom = bollingerBands.Bottom.LastValue - DeltaBollinger;

                consolidation = (top - bottom <= BandHeight) ? consolidation + 1 : 0;

                if (Symbol.Ask <= top && Symbol.Bid >= bottom)
                    return;

                if (consolidation >= ConsolidationPeriods)
                {
                    TradeType tradeType = TradeType.Sell;

                    if (Symbol.Ask > top)
                        tradeType = (Symbol.Bid > top) ? TradeType.Sell : TradeType.Buy;

                    if (Symbol.Bid < bottom)
                        tradeType = (Symbol.Ask < bottom) ? TradeType.Buy : TradeType.Sell;

                    ExecuteMarketOrder(tradeType, Symbol, Volume, this.botName());

                    consolidation = 0;
                }
            }
        }

        protected void OnPositionOpened(PositionOpenedEventArgs args)
        {
            Position position = args.Position;

            ModifyPosition(position, position.pipsToStopLoss(Symbol, StopLoss), position.pipsToTakeProfit(Symbol, TakeProfit));
        }


    }
}

