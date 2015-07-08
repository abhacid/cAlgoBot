//#reference: ..\Indicators\ThirdGenMovingAverage.algo

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Requests;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot()]
    public class ThirdGenMA : Robot
    {
        private int _trendDirection;
        private double _pipSize;

        private ThirdGenMovingAverage _thirdGenMAIndi;
        private MovingAverage _maIndi;

        [Parameter(DefaultValue = "Third Gen MA")]
        public string Label { get; set; }

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 100, MinValue = 20)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 20, MinValue = 8)]
        public int SamplingPeriod { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MAType { get; set; }

        [Parameter(DefaultValue = 5)]
        public int MinMADiff { get; set; }

        [Parameter(DefaultValue = 100000)]
        public int Volume { get; set; }

        [Parameter(DefaultValue = 20)]
        public int StopLoss { get; set; }

        [Parameter(DefaultValue = 300)]
        public int TakeProfit { get; set; }

        [Parameter(DefaultValue = 0)]
        public int TrailingStop { get; set; }

        [Parameter(DefaultValue = 0)]
        public int TriggerTrailing { get; set; }

        [Parameter(DefaultValue = 3)]
        public int Slippage { get; set; }


        protected override void OnStart()
        {
            _thirdGenMAIndi = Indicators.GetIndicator<ThirdGenMovingAverage>(Source, Period, SamplingPeriod, MAType);
            _maIndi = Indicators.MovingAverage(Source, Period, MAType);
            _pipSize = Symbol.PipSize;
        }

        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;

            CheckCross();

            if (TrailingStop > 0)
                TrailPosition();
        }

        // Check for cross and open/close the positions respectively       
        private void CheckCross()
        {
            double fmaCurrent = _thirdGenMAIndi.Result.LastValue;

            double smaCurrent = _maIndi.Result.LastValue;

            switch (_trendDirection)
            {
                case 0:
                    if ((fmaCurrent - smaCurrent) >= MinMADiff * _pipSize)
                        _trendDirection = 1;
                    //Bullish state
                    else if ((smaCurrent - fmaCurrent) >= MinMADiff * _pipSize)
                        _trendDirection = -1;
                    //Bearish state
                    break;
                case 1:
                    //Became bearish
                    if ((smaCurrent - fmaCurrent) >= MinMADiff * _pipSize)
                    {
                        ClosePrev();
                        ExecuteTrade(TradeType.Sell);
                        _trendDirection = -1;
                    }
                    break;
                case -1:
                    //Became bullish
                    if ((fmaCurrent - smaCurrent) >= MinMADiff * _pipSize)
                    {
                        ClosePrev();
                        ExecuteTrade(TradeType.Buy);
                        _trendDirection = 1;
                    }
                    break;
            }
        }

        private void ClosePrev()
        {
            foreach (Position position in Account.Positions.Where(position => position.Label == Label && position.SymbolCode == Symbol.Code))
            {
                Trade.Close(position);
            }
        }

        private void ExecuteTrade(TradeType tradeType)
        {
            var request = new MarketOrderRequest(tradeType, Volume) 
            {
                Label = Label,
                SlippagePips = Slippage,
                StopLossPips = StopLoss,
                TakeProfitPips = TakeProfit
            };
            Trade.Send(request);
        }


        private void TrailPosition()
        {
            foreach (Position position in Account.Positions.Where(position => position.Label == Label && position.SymbolCode == Symbol.Code))
                if (position.TradeType == TradeType.Buy)
                {
                    var distance = (double)(position.StopLoss == null ? Symbol.Bid - position.EntryPrice : Symbol.Bid - position.StopLoss);

                    if (distance >= TriggerTrailing * Symbol.PipSize)
                    {
                        double newStopLossPrice = Math.Round(Symbol.Bid - TrailingStop * Symbol.PipSize, Symbol.Digits);

                        if (position.StopLoss == null || newStopLossPrice > position.StopLoss)
                        {
                            Trade.ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                        }
                    }
                }
                else
                {
                    var distance = (double)(position.StopLoss == null ? position.EntryPrice - Symbol.Ask : position.StopLoss - Symbol.Ask);

                    if (distance >= TriggerTrailing * Symbol.PipSize)
                    {
                        double newStopLossPrice = Math.Round(Symbol.Ask + TrailingStop * Symbol.PipSize, Symbol.Digits);

                        if (position.StopLoss == null || newStopLossPrice < position.StopLoss)
                        {
                            Trade.ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                        }
                    }
                }
        }

        protected override void OnError(Error error)
        {
            Print(error.Code);
            Stop();
        }
    }
}
