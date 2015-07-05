// Golden Dragon v2.0
// Released 20th May 2015
// Created by Craig Stone (except for trailing stops code which is drawn from Spotware's sample code)

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.API.Requests;
using cAlgo.Indicators;


namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC)]
    public class GoldenDragon : Robot
    {
        // Dragon instance number
        [Parameter("Dragon Number", DefaultValue = 1, MinValue = 1)]
        public int DragonNumber { get; set; }

        // Polynomial regression degree
        [Parameter("COG Polynomial Degree", DefaultValue = 3, MinValue = 1, MaxValue = 5)]
        public int cogDegree { get; set; }

        // Number of history bars to use for calculation of primary regression calculation
        [Parameter("1st COG Period", DefaultValue = 110, MinValue = 1)]
        public int cog1Periods { get; set; }

        [Parameter("2nd COG Period", DefaultValue = 250, MinValue = 0)]
        public int cog2Periods { get; set; }

        [Parameter("3rd COG Period", DefaultValue = 465, MinValue = 0)]
        public int cog3Periods { get; set; }

        // Inner polynomial envelope offset, optionally used to set TP level
        [Parameter("1st Channel Deviation", DefaultValue = 1.4, MinValue = 0.1)]
        public double Inner { get; set; }

        // Middle polynomial envelope offset, used to open a position
        [Parameter("2nd Channel Deviation", DefaultValue = 1.8, MinValue = 0.1)]
        public double Middle { get; set; }

        // Outer polynomial envelope offset, optionally used to set SL
        [Parameter("3rd Channel Deviation", DefaultValue = 2.2, MinValue = 0.1)]
        public double Outer { get; set; }

        // Centre of gravity trade filtering
        [Parameter("COG Trade Biasing", DefaultValue = 3, MinValue = 0, MaxValue = 3)]
        public int cogBias { get; set; }

        // Default trade volume
        [Parameter("Trade Volume", DefaultValue = 10, MinValue = 1)]
        public int TradeVolume { get; set; }

        // Maximum slippage
        [Parameter("Slippage (pips)", DefaultValue = 10, MinValue = 0)]
        public int Slippage { get; set; }
        // Catastrophic Stop Loss
        [Parameter("Stop Loss (pips)", DefaultValue = 1100, MinValue = 0)]
        public int StopLoss { get; set; }

        // Minimum Profit
        [Parameter("Minimum Profit (pips)", DefaultValue = 50, MinValue = 0)]
        public int MinimumPips { get; set; }

        [Parameter("Trailing Stops", DefaultValue = false)]
        public bool TrailingStops { get; set; }

        [Parameter("Trailing Stop Trigger (pips)", DefaultValue = 500)]
        public int TrailingStopTrigger { get; set; }

        [Parameter("Trailing Stop Distance (pips)", DefaultValue = 500)]
        public int TrailingStopDistance { get; set; }

        // Equity Protection
        [Parameter("Equity Stop Loss (ratio)", DefaultValue = 0.2, MinValue = 0, MaxValue = 1)]
        public double EquityStop { get; set; }

        // Balance Protection
        [Parameter("Balance Stop Loss (ratio)", DefaultValue = 0.2, MinValue = 0, MaxValue = 1)]
        public double BalanceStop { get; set; }

        // Maximum simultaneous long positions
        [Parameter("Long Trades", DefaultValue = 2, MinValue = 0)]
        public int MaxLongTrades { get; set; }

        // Maximum simultaneous short positions
        [Parameter("Short Trades", DefaultValue = 2, MinValue = 0)]
        public int MaxShortTrades { get; set; }

        // Minimum time delay between trades
        [Parameter("Trade Delay (bars)", DefaultValue = 1, MinValue = 1)]
        public int TradeDelay { get; set; }

        // Trade suspension period on loss
        [Parameter("Wait on Loss (bars)", DefaultValue = 0, MinValue = 0)]
        public int WaitOnLoss { get; set; }

        // Enables trade volume doubling when a stop loss is hit
        [Parameter("Martingale Enabled", DefaultValue = false)]
        public bool Martingale { get; set; }

        // Enables Belkhayate entry filtering
        [Parameter("Belkhayate Timing", DefaultValue = false)]
        public bool BelkhayateTimingFilter { get; set; }

        // Enables Hull MA filtering
        [Parameter("Hull Trade Filter", DefaultValue = true)]
        public bool HullFilter { get; set; }

        // Hull MA Indicator Period
        [Parameter("Hull Period", DefaultValue = 3, MinValue = 1)]
        public int HullPeriod { get; set; }

        private int LongPositions = 0, ShortPositions = 0, MaxLong = 0, MaxShort = 0;
        private int BuyVolume = 0, TakeProfit = 0, Count = 0;

        private double bid = 0, ask = 0, spread = 0, pipsize = 0, OpeningBalance = 0;

        private int t1ix, t2ix, t3ix;

        private double t1h3, t1h2, t1h1, t1c0, t1l1, t1l2, t1l3;
        private double t2h3, t2h2, t2h1, t2c0, t2l1, t2l2, t2l3;
        private double t3h3, t3h2, t3h1, t3c0, t3l1, t3l2, t3l3;

        private int MartingaleActive = 0;
        private bool TradeSafe = true, BuySafe = true, SellSafe = true, isTrigerred = false;

        private BelkhayatePolynomialRegression cog1, cog2, cog3;
		private BelkhayateTiming timing;
        private HMA hull;

        private string DragonID;

        protected override void OnStart()
        {
            DragonID = "Golden Dragon " + DragonNumber + " - " + Symbol.Code;

            Positions.Closed += PositionsOnClosed;

            BuyVolume = TradeVolume;

            OpeningBalance = Account.Balance;

            MaxLong = MaxLongTrades;
            MaxShort = MaxShortTrades;

            cog1 = Indicators.GetIndicator<BelkhayatePolynomialRegression>(cogDegree, cog1Periods, Inner, Middle, Outer);

            if (cog2Periods > 0)
                cog2 = Indicators.GetIndicator<BelkhayatePolynomialRegression>(cogDegree, cog2Periods, Inner, Middle, Outer);

            if (cog3Periods > 0)
                cog3 = Indicators.GetIndicator<BelkhayatePolynomialRegression>(cogDegree, cog3Periods, Inner, Middle, Outer);

            if (BelkhayateTimingFilter)
                timing = Indicators.GetIndicator<BelkhayateTiming>();

            if (HullFilter)
                hull = Indicators.GetIndicator<HMA>(HullPeriod);

            // Identify existing trades from this instance
            foreach (var position in Positions)
            {
                if (position.Label == DragonID)
                    switch (position.TradeType)
                    {
                        case TradeType.Buy:
                            LongPositions++;
                            break;
                        case TradeType.Sell:
                            ShortPositions++;
                            break;
                    }
            }

        }

        protected override void OnBar()
        {
            Count++;

            // COGx1
            t1ix = (int)cog1.ix;
            t1h3 = (double)cog1.sqh3.LastValue;
            t1h2 = (double)cog1.sqh2.LastValue;
            t1h1 = (double)cog1.sqh.LastValue;
            t1c0 = (double)cog1.prc.LastValue;
            t1l1 = (double)cog1.sql.LastValue;
            t1l2 = (double)cog1.sql2.LastValue;
            t1l3 = (double)cog1.sql3.LastValue;

            // COGx2
            if (cog2Periods > 0)
            {
                t2ix = (int)cog2.ix;
                t2h3 = (double)cog2.sqh3.LastValue;
                t2h2 = (double)cog2.sqh2.LastValue;
                t2h1 = (double)cog2.sqh.LastValue;
                t2c0 = (double)cog2.prc.LastValue;
                t2l1 = (double)cog2.sql.LastValue;
                t2l2 = (double)cog2.sql2.LastValue;
                t2l3 = (double)cog2.sql3.LastValue;
            }

            // COGx4
            if (cog3Periods > 0)
            {
                t3ix = (int)cog3.ix;
                t3h3 = (double)cog3.sqh3.LastValue;
                t3h2 = (double)cog3.sqh2.LastValue;
                t3h1 = (double)cog3.sqh.LastValue;
                t3c0 = (double)cog3.prc.LastValue;
                t3l1 = (double)cog3.sql.LastValue;
                t3l2 = (double)cog3.sql2.LastValue;
                t3l3 = (double)cog3.sql3.LastValue;
            }

        }


        protected override void OnTick()
        {
            bid = Symbol.Bid;
            ask = Symbol.Ask;
            spread = Symbol.Spread;
            pipsize = Symbol.PipSize;

            TradeSafe = true;
            BuySafe = true;
            SellSafe = true;

            if (Account.Equity / Account.Balance < EquityStop)
            {
                Print("Equity protection stop triggered. All positions closed.");
                ClosePositions();
                Stop();
            }

            if (Account.Equity < Account.Balance * BalanceStop)
            {
                Print("Account balance protection triggered. All positions closed.");
                ClosePositions();
                Stop();
            }

            if (BelkhayateTimingFilter)
            {
                if (timing.Close.LastValue > timing.BuyLine1.LastValue)
                    BuySafe = false;
                if (timing.Close.LastValue < timing.SellLine1.LastValue)
                    SellSafe = false;
            }

            if (HullFilter)
            {
                if (hull.hma.IsFalling())
                    BuySafe = false;
                if (hull.hma.IsRising())
                    SellSafe = false;
            }

            if (cogBias > 0)
            {
                switch (cogBias)
                {
                    case 1:
                        if (cog1.prc.IsRising())
                            SellSafe = false;
                        if (cog1.prc.IsFalling())
                            BuySafe = false;
                        break;
                    case 2:
                        if (cog2.prc.IsRising())
                            SellSafe = false;
                        if (cog2.prc.IsFalling())
                            BuySafe = false;
                        break;
                    case 3:
                        if (cog3.prc.IsRising())
                            SellSafe = false;
                        if (cog3.prc.IsFalling())
                            BuySafe = false;
                        break;
                }
            }

            if (TrailingStops)
                AdjustTrailingStops();

            if (Count > TradeDelay && LongPositions < MaxLong && BuySafe == true && TradeSafe == true)
            {
                if ((cog2Periods == 0 && cog3Periods == 0 && ask < t1l3) || (cog2Periods > 0 && cog3Periods == 0 && ask < t1l3 && ask < t2l3) || (cog2Periods > 0 && cog3Periods > 0 && ask < t1l3 && ask < t2l3 && ask < t3l3))
                {
                    OpenPosition(TradeType.Buy, BuyVolume * 3);
                }
                else if ((cog2Periods == 0 && cog3Periods == 0 && ask < t1l2) || (cog2Periods > 0 && cog3Periods == 0 && ask < t1l2 && ask < t2l2) || (cog2Periods > 0 && cog3Periods > 0 && ask < t1l2 && ask < t2l2 && ask < t3l2))
                {
                    OpenPosition(TradeType.Buy, BuyVolume * 2);
                }
                else if ((cog2Periods == 0 && cog3Periods == 0 && ask < t1l1) || (cog2Periods > 0 && cog3Periods == 0 && ask < t1l1 && ask < t2l1) || (cog2Periods > 0 && cog3Periods > 0 && ask < t1l1 && ask < t2l1 && ask < t3l1))
                {
                    OpenPosition(TradeType.Buy, BuyVolume * 1);
                }
            }


            if (Count > TradeDelay && ShortPositions < MaxShort && SellSafe == true && TradeSafe == true)
            {
                if ((cog2Periods == 0 && cog3Periods == 0 && bid > t1h3) || (cog2Periods > 0 && cog3Periods == 0 && bid > t1h3 && bid > t2h3) || (cog2Periods > 0 && cog3Periods > 0 && bid > t1h3 && bid > t2h3 && bid > t3h3))
                {
                    OpenPosition(TradeType.Sell, BuyVolume * 3);
                }
                if ((cog2Periods == 0 && cog3Periods == 0 && bid > t1h2) || (cog2Periods > 0 && cog3Periods == 0 && bid > t1h2 && bid > t2h2) || (cog2Periods > 0 && cog3Periods > 0 && bid > t1h2 && bid > t2h2 && bid > t3h2))
                {
                    OpenPosition(TradeType.Sell, BuyVolume * 2);
                }
                if ((cog2Periods == 0 && cog3Periods == 0 && bid > t1h1) || (cog2Periods > 0 && cog3Periods == 0 && bid > t1h1 && bid > t2h1) || (cog2Periods > 0 && cog3Periods > 0 && bid > t1h1 && bid > t2h1 && bid > t3h1))
                {
                    OpenPosition(TradeType.Sell, BuyVolume * 1);
                }
            }

        }

        private void OpenPosition(TradeType tradetype, int quantity)
        {
            // check that the tube is ready
            if (t1ix > cog1Periods)
            {

                switch (tradetype)
                {
                    case TradeType.Buy:
                        if (cog3Periods > 0)
                            TakeProfit = (int)((t3c0 - ask) / pipsize);
                        else if (cog2Periods > 0)
                            TakeProfit = (int)((t2c0 - ask) / pipsize);
                        else
                            TakeProfit = (int)((t1c0 - ask) / pipsize);
                        LongPositions++;
                        Print("Opened LONG position {0} of {1}", LongPositions, MaxLong);
                        break;
                    case TradeType.Sell:
                        if (cog3Periods > 0)
                            TakeProfit = (int)((bid - t3c0) / pipsize);
                        else if (cog2Periods > 0)
                            TakeProfit = (int)((bid - t2c0) / pipsize);
                        else
                            TakeProfit = (int)((bid - t1c0) / pipsize);
                        ShortPositions++;
                        Print("Opened SHORT position {0} of {1}", ShortPositions, MaxShort);
                        break;
                }

                if (MartingaleActive > 0)
                    TakeProfit = (int)(StopLoss / 2);

                ExecuteMarketOrder(tradetype, Symbol, quantity, DragonID, StopLoss, TakeProfit, Slippage, DragonID);

                Count = 0;
            }
            Print("Period {0}, Long {1} of {2}, Short {3} of {4}", t1ix, LongPositions, MaxLong, ShortPositions, MaxShort);
        }

        protected override void OnPositionOpened(Position openedPosition)
        {
            switch (openedPosition.TradeType)
            {
                case TradeType.Buy:
                    LongPositions++;
                    break;
                case TradeType.Sell:
                    ShortPositions++;
                    break;
            }
        }


        private void ClosePositions()
        {
            foreach (var position in Positions)
            {
                if (position.Label == DragonID && position.GrossProfit < -10)
                    ClosePosition(position);
            }
        }


        private void PositionsOnClosed(PositionClosedEventArgs args)
        {
            var position = args.Position;
            switch (position.TradeType)
            {
                case TradeType.Buy:
                    LongPositions--;
                    Print("Closed LONG position. {0} of {1} remain open.", LongPositions, MaxLong);
                    break;
                case TradeType.Sell:
                    ShortPositions--;
                    Print("Closed SHORT position. {0} of {1} remain open.", ShortPositions, MaxShort);
                    break;
            }


            if (position.GrossProfit < 0)
            {

                if (WaitOnLoss > 0)
                    Count = -WaitOnLoss;

                if (Martingale)
                {
                    MartingaleActive++;
                    BuyVolume = BuyVolume * 2;
                }

            }
            else if (MartingaleActive > 0)
            {

                MartingaleActive--;
                BuyVolume = (int)(BuyVolume / 2);

            }
            Print("Period {0}, Long {1} of {2}, Short {3} of {4}", t1ix, LongPositions, MaxLong, ShortPositions, MaxShort);
        }

        private void AdjustTrailingStops()
        {
            foreach (var position in Positions)
            {
                if (position.Label == DragonID)
                {
                    if (position.TradeType == TradeType.Buy)
                    {
                        double distance = Symbol.Bid - position.EntryPrice;

                        if (distance >= TrailingStopTrigger * Symbol.PipSize)
                        {
                            if (!isTrigerred)
                            {
                                isTrigerred = true;
                                Print("Trailing stop loss triggered on position {0}", position.Id);
                            }

                            double newStopLossPrice = Math.Round(Symbol.Bid - TrailingStopDistance * Symbol.PipSize, Symbol.Digits);

                            if (position.StopLoss == null || newStopLossPrice > position.StopLoss)
                            {
                                ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                            }
                        }
                    }
                    else
                    {
                        double distance = position.EntryPrice - Symbol.Ask;

                        if (distance >= TrailingStopTrigger * Symbol.PipSize)
                        {
                            if (!isTrigerred)
                            {
                                isTrigerred = true;
                                Print("Trailing stop loss triggered on position {0}", position.Id);
                            }

                            double newStopLossPrice = Math.Round(Symbol.Ask + TrailingStopDistance * Symbol.PipSize, Symbol.Digits);

                            if (position.StopLoss == null || newStopLossPrice < position.StopLoss)
                            {
                                ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                            }
                        }
                    }

                }
            }
        }

        protected void Message(int level, string message)
        {
            Print("[{0}] {1}", level, message);
        }


        protected override void OnStop()
        {
            Print("Dragon Sleeping");
        }

    }
}
