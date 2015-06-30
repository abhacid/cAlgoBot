
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

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/cAlgoBot
#endregion


#region advertisement
// -------------------------------------------------------------------------------
//			Trading using leverage carries a high degree of risk to your capital, and it is possible to lose more than
//			your initial investment. Only speculate with money you can afford to lose.
// -------------------------------------------------------------------------------
#endregion


//Needs to install: http://ctdn.com/algos/indicators/show/17, Manage references.
//
//Date: 17/12/2014
//Country: Chile
//Copyright: Felipe Sepulveda Maldonado 
//LinkedIn: https://cl.linkedin.com/in/felipesepulvedamaldonado
//Facebook: https://www.facebook.com/mymagicflight1
//Whats Up: +56 9 58786321
// felipe.sepulveda@gmail.com
//
//Recomended Timeframe: Minute, for more frequency and accuracy.
//Cheers!

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using cAlgo.Lib;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class XkaliburBaII : Robot
    {
        #region cBot Parameters
        [Parameter("Volume", DefaultValue = 1000, MinValue = 1000)]
        public int InitialVolume { get; set; }

        [Parameter("Take Profit", DefaultValue = 2, MinValue = 0, MaxValue = 102)]
        public double TakeProfit { get; set; }

        [Parameter("WilliamsR High", DefaultValue = 70, MinValue = 60, MaxValue = 95)]
        public int WilliamsPercentRangeHigh { get; set; }

        [Parameter("WilliamsR Low", DefaultValue = 30, MinValue = 5, MaxValue = 40)]
        public int WilliamsPercentRangeLow { get; set; }

        [Parameter("WilliamsR Period", DefaultValue = 120, MinValue = 10, MaxValue = 150)]
        public int WilliamsPercentRangePeriod { get; set; }

        [Parameter("Zero Loss Inverse (Pips)", DefaultValue = 6, MinValue = 4, MaxValue = 15)]
        public int ZeroLossInverse { get; set; }

        [Parameter("EHMA Period", DefaultValue = 12, MinValue = 7)]
        public int HullPeriod { get; set; }

        [Parameter("Max. Time Open (Minutes)", DefaultValue = 30, MinValue = 5, MaxValue = 120)]
        public int maxTimeOpen { get; set; }

        #endregion

        #region cBot variables

        private string _botVersion = Assembly.GetExecutingAssembly().FullName.Split(',')[1].Replace("Version=", "").Trim();
        private string _botName;
        private string _instanceLabel;

        private double _trailingStop;
        private long _nVolume;
        private string _commentId;
        private EHMA _hullEma1;
        private EHMA _hullEma2;

        #endregion

        #region cBot Events
        protected override void OnStart()
        {
            _botName = ToString();
            _instanceLabel = _botName + "-" + _botVersion + "-" + Symbol.Code + "-" + TimeFrame.ToString();

            _hullEma1 = Indicators.GetIndicator<EHMA>(HullPeriod);
            _hullEma2 = Indicators.GetIndicator<EHMA>(HullPeriod * 6);
            Timer.Start(1);
        }

        protected override void OnTimer()
        {
        }

        protected override void OnTick()
        {
            double averageTrueRange = 100000 * Indicators.AverageTrueRange(MarketSeries, 5, MovingAverageType.VIDYA).Result.Last(0);

            TradeType? tradeType = signal();

            if (tradeType.HasValue)
                ExecuteMarketOrder(tradeType.Value, Symbol, InitialVolume, _instanceLabel, 100, 100, 2);

            SetTrailingStop(averageTrueRange);

            ZeroLoss();
        }
        #endregion

        private TradeType? signal()
        {
            double williamwPercentRange = 100 + Indicators.WilliamsPctR(WilliamsPercentRangePeriod).Result.Last(0);

            TradeType? tradeType = null;

            if ((williamwPercentRange < WilliamsPercentRangeLow) && _hullEma1.ehma.IsRising() && _hullEma2.ehma.IsRising())
            {
                System.Threading.Thread.Sleep(15000);
                tradeType = TradeType.Buy;
            }

            if ((williamwPercentRange > WilliamsPercentRangeHigh) && _hullEma1.ehma.IsFalling() && _hullEma2.ehma.IsFalling())
            {
                System.Threading.Thread.Sleep(15000);
                tradeType = TradeType.Sell;
            }

            return tradeType;

        }

        private void SetTrailingStop(double market)
        {
            var positions = Positions.FindAll(_instanceLabel, Symbol);
            int actualTimeInMinutes = (Time.Hour * 60) + Time.Minute;
            _trailingStop = market / 30;

            foreach (Position position in positions)
            {
                int entryTimeInMinutes = (position.EntryTime.Hour * 60) + position.EntryTime.Minute;

                if (entryTimeInMinutes + maxTimeOpen < actualTimeInMinutes)
                    continue;

                if (position.TradeType == TradeType.Sell)
                {
                    if (position.EntryPrice - Symbol.Ask >= TakeProfit * Symbol.PipSize)
                    {

                        double newStopLossPrice = Symbol.Ask + _trailingStop * Symbol.PipSize;
                        if (position.StopLoss == null || newStopLossPrice < position.StopLoss)
                            ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                    }
                }
                else
                {
                    if (Symbol.Bid - position.EntryPrice >= TakeProfit * Symbol.PipSize)
                    {
                        double newStopLossPrice = Symbol.Bid - _trailingStop * Symbol.PipSize;
                        if (position.StopLoss == null || newStopLossPrice > position.StopLoss)
                            ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                    }
                }

            }
        }

        private void ZeroLoss()
        {
            var positions = Positions.FindAll(_instanceLabel, Symbol);

            foreach (Position position in positions)
            {
                if (!(position.TakeProfit.HasValue))
                    continue;

                bool isModifySellPosition = false, isModifyBuyPosition = false;

                if (position.TradeType == TradeType.Sell)
                {
                    isModifySellPosition = (position.EntryPrice - Symbol.Ask < -ZeroLossInverse * Symbol.PipSize) && (((position.EntryPrice - position.TakeProfit.Value) / Symbol.PipSize) > 90);

                    if (isModifySellPosition)
                        ModifyPosition(position, position.StopLoss, Symbol.Ask - 90 * Symbol.PipSize);
                }
                else
                {
                    isModifyBuyPosition = (Symbol.Bid - position.EntryPrice < -ZeroLossInverse * Symbol.PipSize) && (((position.TakeProfit.Value - position.EntryPrice) / Symbol.PipSize) > 90);

                    if (isModifyBuyPosition)
                        ModifyPosition(position, position.StopLoss, Symbol.Bid + 90 * Symbol.PipSize);
                }

                if (isModifySellPosition || isModifyBuyPosition)
                {
                    switch (position.Volume)
                    {
                        case 1000:
                            _nVolume = 2000;
                            break;
                        case 2000:
                            _nVolume = 3000;
                            break;
                        default:
                            _nVolume = position.Volume * 2;
                            break;
                    }

                    if (position.Comment != "")
                        _commentId = position.Comment;
                    else
                        _commentId = position.Id.ToString();

                    ExecuteMarketOrder(position.inverseTradeType(), Symbol, _nVolume, _instanceLabel, 100, 100, 2, _commentId);
                }


            }

        }
    }
}

