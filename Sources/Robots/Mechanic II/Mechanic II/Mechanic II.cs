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

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/Robot_Forex
#endregion

#region Description
// Author		: gorkroitor 
// link			: http://ctdn.com/algos/cbots/show/657
// Modified		: by Abdallah HACID

//The Mechanic Bot uses the Candlestick Tendency indicator II as a decision driver for entering trades. The basic idea 
//is to trade higher order timeframe with current local timeframe. It trades a single position at a time and 
//has mechanisms for trailing stops and basic money management (Stop Losss and Take Profit.

#endregion
using System;
using System.Linq;
using System.Reflection;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{

    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MechanicII : Robot
    {
        #region cBot Parameters
        [Parameter()]
        public TimeFrame HighOrderTimeFrame { get; set; }

        [Parameter(DefaultValue = 10000, Step = 1000, MinValue = 1000)]
        public int Volume { get; set; }

        [Parameter(DefaultValue = true)]
        public bool EnableStopLoss { get; set; }

        [Parameter(DefaultValue = 20, MinValue = 1, Step = 1)]
        public double StopLoss { get; set; }

        [Parameter(DefaultValue = false)]
        public bool EnableTrailingStop { get; set; }

        [Parameter(DefaultValue = 10, MinValue = 1, Step = 1)]
        public double TrailingStop { get; set; }

        [Parameter(DefaultValue = 10, MinValue = 1, Step = 1)]
        public double TrailingStart { get; set; }

        [Parameter(DefaultValue = true)]
        public bool EnableTakeProfit { get; set; }

        [Parameter(DefaultValue = 30, MinValue = 0)]
        public int TakeProfit { get; set; }

        [Parameter(DefaultValue = true)]
        public bool EnterOnSyncSignalOnly { get; set; }

        [Parameter(DefaultValue = false)]
        public bool ExitOnOppositeSignal { get; set; }
        #endregion


        #region cBot Variables
        private string _botName;
        private string _botVersion = Assembly.GetExecutingAssembly().FullName.Split(',')[1].Replace("Version=", "").Trim();
        private string _instanceLabel;

        private CandlestickTendencyII tendency;

        //private bool previewIsGlobalTendencyRising;
        //private bool previewIsGlobalTendencyFalling;
        //private bool previewIsLocalTrendRising;
        //private bool previewIsLocalTrendFalling;
        private int previewIndex;
        #endregion

        protected override void OnStart()
        {
            _botName = ToString();
            _instanceLabel = string.Format("{0}-{1}-{2}-{3}-{4}", _botName, _botVersion, Symbol.Code, TimeFrame.ToString(), HighOrderTimeFrame.ToString());

            tendency = Indicators.GetIndicator<CandlestickTendencyII>(HighOrderTimeFrame);

            //saveTentency(MarketSeries.Close.Count-2);
        }

        protected override void OnTick()
        {
            manageTrailingStops();

            int index = MarketSeries.Close.Count - 2;

            if (index <= previewIndex)
                return;

            previewIndex = index;

            Position position = CurrentPosition();

            if (ExitOnOppositeSignal && position != null && isCloseSignal(index - 1))
                ClosePosition(position);

            TradeType? tradeType = signal(index - 1);
            if (tradeType.HasValue)
                executeOrder(tradeType.Value);

            //saveTentency(index);		

        }

        private TradeType? signal(int index)
        {
            TradeType? tradeType = null;

            // this occur when the preceding boolean test instruction is true or there is no active position.
            if (CurrentPosition() == null)
            {
                if (EnterOnSyncSignalOnly)
                {
                    if (tendency.IsShortSignal(index - 1) && tendency.IsLongSignal(index))
                        tradeType = TradeType.Buy;
                    else if (tendency.IsLongSignal(index - 1) && tendency.IsShortSignal(index))
                        tradeType = TradeType.Sell;
                }
                else
                {

                    if (tendency.IsLongSignal(index))
                        tradeType = TradeType.Buy;
                    else if (tendency.IsShortSignal(index))
                        tradeType = TradeType.Sell;

                }
            }

            return tradeType;

        }

        protected TradeResult executeOrder(TradeType tradeType)
        {

            if (!EnableStopLoss && EnableTakeProfit)
                return ExecuteMarketOrder(tradeType, Symbol, Volume, _instanceLabel, null, TakeProfit);

            if (!EnableStopLoss && !EnableTakeProfit)
                return ExecuteMarketOrder(tradeType, Symbol, Volume, _instanceLabel, null, null);

            if (EnableStopLoss && !EnableTakeProfit)
                return ExecuteMarketOrder(tradeType, Symbol, Volume, _instanceLabel, StopLoss, null);

            return ExecuteMarketOrder(tradeType, Symbol, Volume, _instanceLabel, StopLoss, TakeProfit);
        }

        private Position CurrentPosition()
        {
            return Positions.Find(_instanceLabel);
        }

        private bool isShort()
        {
            Position position = CurrentPosition();
            return position != null && position.TradeType == TradeType.Sell;
        }

        private bool isLong()
        {
            Position position = CurrentPosition();
            return position != null && position.TradeType == TradeType.Buy;
        }

        private bool isCloseSignal(int index)
        {
            Position position = CurrentPosition();

            if (position != null)
                return ((position.TradeType == TradeType.Sell) ? tendency.IsLongSignal(index) : tendency.IsShortSignal(index));
            else
                return false;
        }

        //private void saveTentency(int index)
        //{
        //	previewIndex = index;
        //	previewIsLocalTrendRising = tendency.isLocalTrendRising(index);
        //	previewIsLocalTrendFalling = tendency.isLocalTrendFalling(index);
        //	previewIsGlobalTendencyRising = tendency.isGlobalTrendRising(index);
        //	previewIsGlobalTendencyFalling = tendency.isGlobalTrendFalling(index);			
        //}


        protected void manageTrailingStops()
        {
            if (!EnableTrailingStop)
                return;

            foreach (Position position in Positions.FindAll(_instanceLabel))
            {
                if (position.Pips >= TrailingStart)
                {
                    if (position.TradeType == TradeType.Buy)
                    {
                        var newStopLoss = Symbol.Bid - TrailingStop * Symbol.PipSize;
                        if (position.StopLoss < newStopLoss)
                            ModifyPosition(position, newStopLoss, null);
                    }
                    else if (position.TradeType == TradeType.Sell)
                    {
                        var newStopLoss = Symbol.Ask + TrailingStop * Symbol.PipSize;
                        if (position.StopLoss > newStopLoss)
                            ModifyPosition(position, newStopLoss, null);
                    }
                }
            }
        }

    }
}
