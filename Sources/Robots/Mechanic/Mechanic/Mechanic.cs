using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{

    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Mechanic : Robot
    {

        // general params

        [Parameter()]
        public TimeFrame HighOrderTimeFrame { get; set; }

        [Parameter(DefaultValue = 10000, Step = 1000, MinValue = 1000)]
        public int Volume { get; set; }

        [Parameter(DefaultValue = true)]
        public bool EnableStopLoss { get; set; }

        [Parameter(DefaultValue = 20, MinValue = 1, Step = 1)]
        public double StopLoss { get; set; }

        [Parameter(DefaultValue = false)]
        public bool EnableBreakEven { get; set; }

        [Parameter(DefaultValue = 10, MinValue = 0, Step = 1)]
        public double BreakEvenPips { get; set; }

        [Parameter(DefaultValue = 20, MinValue = 0, Step = 1)]
        public double BreakEvenGain { get; set; }

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

        private string label;
        private const int indexOffset = 0;
        private int index;
        private CandlestickTendency tendency;

        public bool globalTendencyWasLong;
        public bool globalTendencyWasShort;
        public bool localTendencyWasLong;
        public bool localTendencyWasShort;

        public Position currentPosition
        {
            get { return Positions.Find(label); }
        }
        public bool inPosition
        {
            get { return currentPosition != null; }
        }
        public bool inShortPosition
        {
            get { return currentPosition != null && currentPosition.TradeType == TradeType.Sell; }
        }
        public bool inLongPosition
        {
            get { return currentPosition != null && currentPosition.TradeType == TradeType.Buy; }
        }

        public bool globalTendencyIsLong
        {
            get { return tendency.HighOrderLine[index] > 0; }
        }
        public bool localTendencyIsLong
        {
            get { return tendency.Line[index] > 0; }
        }
        public bool globalTendencyIsShort
        {
            get { return tendency.HighOrderLine[index] < 0; }
        }
        public bool localTendencyIsShort
        {
            get { return tendency.Line[index] < 0; }
        }
        public bool longSignal
        {
            get { return localTendencyIsLong && globalTendencyIsLong; }
        }
        public bool shortSignal
        {
            get { return localTendencyIsShort && globalTendencyIsShort; }
        }
        public bool closeSignal
        {
            get { return inPosition ? ((currentPosition.TradeType == TradeType.Sell) ? longSignal : shortSignal) : false; }
        }

        protected override void OnStart()
        {

            label = "Mechanic " + Symbol.Code + " " + TimeFrame.ToString() + " / " + HighOrderTimeFrame.ToString();
            tendency = Indicators.GetIndicator<CandlestickTendency>(HighOrderTimeFrame);
            index = MarketSeries.Close.Count - 1;
        }

        protected void UpdateTrailingStops()
        {

            if (!EnableTrailingStop)
                return;

            var positions = Positions.FindAll(label);
            if (positions == null)
                return;

            foreach (var position in positions)
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

        protected void MoveToBreakEven()
        {

            if (!EnableBreakEven)
                return;

            var positions = Positions.FindAll(label);
            if (positions == null)
                return;

            foreach (var position in positions)
            {
                if (position.Pips >= BreakEvenPips)
                {
                    if (position.TradeType == TradeType.Buy)
                    {
                        var newStopLoss = Symbol.Bid - BreakEvenGain * Symbol.PipSize;
                        if (position.StopLoss < newStopLoss)
                            ModifyPosition(position, newStopLoss, null);
                    }
                    else if (position.TradeType == TradeType.Sell)
                    {
                        var newStopLoss = Symbol.Ask + BreakEvenGain * Symbol.PipSize;
                        if (position.StopLoss > newStopLoss)
                            ModifyPosition(position, newStopLoss, null);
                    }
                }
            }
        }

        protected TradeResult EnterInPosition(TradeType direction)
        {

            if (!EnableStopLoss && EnableTakeProfit)
                return ExecuteMarketOrder(direction, Symbol, Volume, label, null, TakeProfit);

            if (!EnableStopLoss && !EnableTakeProfit)
                return ExecuteMarketOrder(direction, Symbol, Volume, label, null, null);

            if (EnableStopLoss && !EnableTakeProfit)
                return ExecuteMarketOrder(direction, Symbol, Volume, label, StopLoss, null);

            return ExecuteMarketOrder(direction, Symbol, Volume, label, StopLoss, TakeProfit);
        }

        protected override void OnTick()
        {

            index = MarketSeries.Close.Count - 1;
            UpdateTrailingStops();
            MoveToBreakEven();

        }

        protected override void OnBar()
        {

            index = MarketSeries.Close.Count - 2;

            if (ExitOnOppositeSignal && closeSignal)
                ClosePosition(currentPosition);

            if (!inPosition)
            {

                if (EnterOnSyncSignalOnly)
                {

                    if (localTendencyWasShort && globalTendencyWasShort && localTendencyIsLong && globalTendencyIsLong)
                    {
                        EnterInPosition(TradeType.Buy);
                    }
                    else if (localTendencyWasLong && globalTendencyWasLong && localTendencyIsShort && globalTendencyIsShort)
                    {
                        EnterInPosition(TradeType.Sell);
                    }

                }
                else
                {

                    if (shortSignal)
                    {
                        EnterInPosition(TradeType.Sell);
                    }
                    else if (longSignal)
                    {
                        EnterInPosition(TradeType.Buy);
                    }
                }
            }

            localTendencyWasLong = localTendencyIsLong;
            localTendencyWasShort = localTendencyIsShort;
            globalTendencyWasLong = globalTendencyIsLong;
            globalTendencyWasShort = globalTendencyIsShort;
        }

        protected override void OnStop()
        {
        }
    }
}
