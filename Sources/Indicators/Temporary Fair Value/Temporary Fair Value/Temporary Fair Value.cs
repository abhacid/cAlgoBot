// -------------------------------------------------------------------------------
//
//      TemporaryFairValue (former name JeexHerd) is property of Jeex.Eu - victor@jeex.eu 
//      Current Version 1.0.3
//
//      Changes in 1.0.3
//          1. Corrected and added the calculus for past bars. TFV-values of past bars are based on smallest timeframe (minute), as past ticks are not available.
//
//      Changes in 1.0.2
//          1. Changed the name into TemporaryFairValue
//          2. Thanks to http://www.pinebaycm.com/
//          3. Added ResultMovement[bar] = double.NaN; to the method oldCows();
//          4. Changed the parameter _exponential to double
//
//      Changes in 1.0.1:
//          1. Translated Dutch names into English (or something that has to look like English
//          2. At the start of a new bar, the values are the average of the last minute of the previous candle and the current price.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using System.Collections.Generic;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TemporaryFairValue : Indicator
    {
        [Parameter("Exponential", DefaultValue = 2, MinValue = 0)]
        public double _exponential { get; set; }

        [Output("Candle Heart", Color = Colors.Salmon, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries ResultHeart { get; set; }

        [Output("Candle Spread", Color = Colors.DodgerBlue, PlotType = PlotType.Points, Thickness = 5)]
        public IndicatorDataSeries ResultMovement { get; set; }

        private int startCandle = 0;
        private int thisCandle = 0;
        private double prevAsk;
        private double prevBid;
        private int askCount = 0;
        private int bidCount = 0;
        private double aveAsk;
        private double aveBid;
        private MarketSeries minuut;

        protected override void Initialize()
        {
            thisCandle = MarketSeries.Open.Count - 1;
            startCandle = thisCandle;
            minuut = MarketData.GetSeries(TimeFrame.Minute);
            for (int i = 1; i <= startCandle; i++)
                oldCows(i);
        }

        public override void Calculate(int index)
        {

            // candles are build with the bid price.
            double candle = Symbol.Bid;
            int minuutindex = GetIndexByDate(minuut, MarketSeries.OpenTime[index]);

            if (index < startCandle)
                return;

            // start of a new candle
            if (index != thisCandle)
            {
                thisCandle = index;
                askCount = 1;
                bidCount = 1;

                // start a new candle with the average of the last minute of the previous candle + the current price.
                aveAsk = (minuut.Median[minuutindex - 1] + (_exponential * Symbol.Ask)) / (_exponential + 1);
                aveBid = (minuut.Median[minuutindex - 1] + (_exponential * Symbol.Bid)) / (_exponential + 1);
                prevAsk = aveAsk;
                prevBid = aveBid;

                ResultHeart[index] = aveBid;
                ResultMovement[index] = aveAsk;
            }
            else
            {
                // continue the current candle

                if (Symbol.Ask != prevAsk)
                {
                    // Ask price different from previous
                    aveAsk = ((askCount * aveAsk) + (_exponential * Symbol.Ask)) / (askCount + _exponential);
                    prevAsk = Symbol.Ask;
                    askCount++;
                }

                if (Symbol.Bid != prevBid)
                {
                    // Bid price different from previous
                    aveBid = ((bidCount * aveBid) + (_exponential * Symbol.Bid)) / (bidCount + _exponential);
                    prevBid = Symbol.Bid;
                    bidCount++;
                    candle = aveBid;
                }
                ResultHeart[index] = aveBid;
                ResultMovement[index] = aveAsk;
            }
        }

        private void oldCows(int bar)
        {
            // makes the dots for the previous bars by the same method but uses TimeFrame.Minute in stead of ticks, because history of ticks is not available in cAlgo.

            // if timeframe is minute, just put in the average of the minute.
            if (MarketSeries.TimeFrame == TimeFrame.Minute)
            {
                ResultHeart[bar] = MarketSeries.Median[bar];
                return;
            }

            DateTime start = MarketSeries.OpenTime[bar];
            DateTime eind = MarketSeries.OpenTime[bar + 1];
            int minindexstart = GetIndexByDate(minuut, start);
            int minindexeind = GetIndexByDate(minuut, eind);


            double av = 0;
            int avcount = 0;
            for (int i = minindexstart; i < minindexeind; i++)
            {
                if (minuut.OpenTime[i] < start)
                    continue;
                if (minuut.OpenTime[i] >= eind)
                    break;

                av = ((av * avcount) + (_exponential * minuut.Median[i])) / (avcount + _exponential);
                avcount++;
            }
            if (av > 0)
            {
                ResultHeart[bar] = av;
                ResultMovement[bar] = double.NaN;
            }
        }

        private int GetIndexByDate(MarketSeries series, DateTime time)
        {
            for (int i = series.Close.Count - 1; i > 0; i--)
            {
                if (time == series.OpenTime[i])
                    return i;
            }
            return -1;
        }
    }
}
