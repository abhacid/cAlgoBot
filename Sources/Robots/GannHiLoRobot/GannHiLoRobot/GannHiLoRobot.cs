//# reference: ..\Indicators\GannHighLow.algo
// -------------------------------------------------------------------------------
//
//    This is a Robot based on the GannHighLow Indicator. 
//    If close price rises above the GannHighLow indicator a buy is triggered and 
//    if the prices falls below the GannHighLow  indicator a sell is triggered.
//    There is only one position open at a time. So, if a buy is triggered and a sell is open
//    the sell will be closed.
//
//    The default parameters use no SL/TP (Default to zero)
//    If SLTrigger and TrailingStop are greater than zero then Trailing stop is used
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Requests;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot()]
    public class GannHiLoRobot : Robot
    {
        private GannHighLow _gannHighLowIndicator;
        private Position _position;

        [Parameter("Period", DefaultValue = 50)]
        public int Period { get; set; }

        [Parameter("Volume", DefaultValue = 10000)]
        public int Volume { get; set; }

        [Parameter(DefaultValue = 0)]
        public int StopLoss { get; set; }


        [Parameter(DefaultValue = 0)]
        public int TakeProfit { get; set; }


        [Parameter(DefaultValue = 0)]
        public double SLTrigger { get; set; }

        [Parameter(DefaultValue = 0)]
        public double TrailingStop { get; set; }

        private bool _isTrigerred;


        protected bool UseTrailingStop
        {
            get { return SLTrigger > 0.0 && TrailingStop > 0.0; }
        }

        protected override void OnStart()
        {
            _gannHighLowIndicator = Indicators.GetIndicator<GannHighLow>(Period);
        }

        protected override void OnTick()
        {

            if (Trade.IsExecuting || _position == null)
                return;

            if (UseTrailingStop)
                Trail();

        }

        /// <summary>
        /// If close price rises above the GannHighLow indicator a buy is triggered and 
        /// if the prices falls below the GannHighLow  indicator a sell is triggered.
        /// </summary>
        protected override void OnBar()
        {
            bool isLongPositionOpen = _position != null && _position.TradeType == TradeType.Buy;
            bool isShortPositionOpen = _position != null && _position.TradeType == TradeType.Sell;


            if (_gannHighLowIndicator.Result.HasCrossedAbove(MarketSeries.Close, 1) && !isShortPositionOpen)
            {
                ClosePosition();
                Sell();
            }
            else if (_gannHighLowIndicator.Result.HasCrossedBelow(MarketSeries.Close, 1) && !isLongPositionOpen)
            {
                ClosePosition();
                Buy();
            }
        }


        /// <summary>
        /// Close the existing position
        /// </summary>
        private void ClosePosition()
        {
            if (_position == null)
                return;

            Trade.Close(_position);
            _position = null;
        }

        /// <summary>
        /// Send a Buy trade request
        /// </summary>
        private void Buy()
        {
            Request request = new MarketOrderRequest(TradeType.Buy, Volume) 
            {
                Label = "Gann HiLo",
                //SlippagePips = 1,
                StopLossPips = StopLoss > 0 ? (int?)StopLoss : null,
                TakeProfitPips = TakeProfit > 0 ? (int?)TakeProfit : null
            };
            Trade.Send(request);
        }

        /// <summary>
        /// Send a Sell trade request
        /// </summary>
        private void Sell()
        {
            Request request = new MarketOrderRequest(TradeType.Sell, Volume) 
            {
                Label = "Gann HiLo",
                //SlippagePips = 1,
                StopLossPips = StopLoss > 0 ? (int?)StopLoss : null,
                TakeProfitPips = TakeProfit > 0 ? (int?)TakeProfit : null
            };
            Trade.Send(request);
        }

        protected override void OnPositionOpened(Position openedPosition)
        {
            _position = openedPosition;
        }


        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }

        /// <summary>
        /// Trailing Stop
        /// </summary>
        private void Trail()
        {
            if (_position.TradeType == TradeType.Buy)
            {
                double distance = Symbol.Bid - _position.EntryPrice;

                if (distance >= SLTrigger * Symbol.PipSize)
                {
                    if (!_isTrigerred)
                    {
                        _isTrigerred = true;
                        Print("Trailing Stop Loss triggered...");
                    }

                    double newStopLossPrice = Math.Round(Symbol.Bid - TrailingStop * Symbol.PipSize, Symbol.Digits);

                    if (_position.StopLoss == null || newStopLossPrice > _position.StopLoss)
                    {
                        Trade.ModifyPosition(_position, newStopLossPrice, _position.TakeProfit);
                    }
                }
            }
            else
            {
                double distance = _position.EntryPrice - Symbol.Ask;

                if (distance >= SLTrigger * Symbol.PipSize)
                {
                    if (!_isTrigerred)
                    {
                        _isTrigerred = true;
                        Print("Trailing Stop Loss triggered...");
                    }

                    double newStopLossPrice = Math.Round(Symbol.Ask + TrailingStop * Symbol.PipSize, Symbol.Digits);

                    if (_position.StopLoss == null || newStopLossPrice < _position.StopLoss)
                    {
                        Trade.ModifyPosition(_position, newStopLossPrice, _position.TakeProfit);
                    }
                }
            }
        }

    }
}
