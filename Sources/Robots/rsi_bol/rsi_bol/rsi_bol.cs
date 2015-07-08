using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class rsi_bol : Robot
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }
        [Parameter("lotsize", DefaultValue = 10000)]
        public int lotsize { get; set; }


        [Parameter("sl", DefaultValue = -16)]
        public int sl { get; set; }
        [Parameter("tp", DefaultValue = 18)]
        public int tp { get; set; }
        [Parameter("semiTrendSL", DefaultValue = -16)]
        public int semiTrendSL { get; set; }
        [Parameter("semiTrendTP", DefaultValue = 18)]
        public int semiTrendTP { get; set; }

        [Parameter("Period0", DefaultValue = 12)]
        public int Period0 { get; set; }

        [Parameter("Period1", DefaultValue = 18)]
        public int Period1 { get; set; }
        [Parameter("stdev", DefaultValue = 3)]
        public double stdev { get; set; }
        [Parameter("rsitop", DefaultValue = 67)]
        public int rsitop { get; set; }
        [Parameter("rsibottom", DefaultValue = 36)]
        public int rsibottom { get; set; }

        [Parameter("size", DefaultValue = 30)]
        public int size { get; set; }
        [Parameter("threshold", DefaultValue = 3.0)]
        public double threshold { get; set; }

        //Boundaries are percentages between 1 and -1 and be about 0
        [Parameter("upperbound", DefaultValue = 0.02)]
        public double upperbound { get; set; }
        [Parameter("lowerbound", DefaultValue = -0.02)]
        public double lowerbound { get; set; }

        [Parameter("Position Label", DefaultValue = "trend")]
        public string Label1 { get; set; }
        [Parameter("Position Label", DefaultValue = "rsibol")]
        public string Label2 { get; set; }

        //for trend
        [Parameter("Trigger (pips)", DefaultValue = 100)]
        public int Trigger { get; set; }
        [Parameter("Trailing Stop (pips)", DefaultValue = 50)]
        public int TrailingStop { get; set; }

        //for reversion
        [Parameter("reversion Trigger (pips)", DefaultValue = 100)]
        public int revTrigger { get; set; }
        [Parameter("reversion Trailing Stop (pips)", DefaultValue = 50)]
        public int revTrailingStop { get; set; }


        private bool _isTrigerred;

        private double[] BOL, oneBOL, twoBOL;
        private RelativeStrengthIndex rsi;
        private BollingerBands bol;

        protected override void OnStart()
        {

            rsi = Indicators.RelativeStrengthIndex(Source, Period0);
            bol = Indicators.BollingerBands(Source, Period1, stdev, MovingAverageType.Simple);

            //obtain high and low of initial start bar 
            double max = MarketSeries.High.LastValue;
            double min = MarketSeries.Low.LastValue;
        }

        protected override void OnTick()
        {

            //Computing objective/first order/second order derivative functions
            int oneSize = size - 1;
            int twoSize = oneSize - 1;

            BOL = new double[size];
            for (int i = 0; i < BOL.Length; i++)
            {
                BOL[i] = bol.Top.Last(i) - bol.Bottom.Last(i);
                //Print(BOL[i]);
            }
            //First derivative : Change in bollinger band value
            oneBOL = new double[oneSize];
            for (int i = 0; i < oneBOL.Length; i++)
            {
                //change in bollinger bands in % terms
                oneBOL[i] = BOL[i + 1] / BOL[0] - 1;
            }
            //Second derivative: Rate of change in bollinger band value
            twoBOL = new double[twoSize];
            for (int i = 0; i < twoBOL.Length; i++)
            {
                twoBOL[i] = oneBOL[i + 1] / oneBOL[i] - 1;
            }



            actual(BOL);
            gradientDifference(oneBOL);
//Reversion trading algo
            if (actual(BOL) > threshold && (gradientDifference(oneBOL) > upperbound || gradientDifference(oneBOL) < lowerbound))
            {
                if (rsi.Result.LastValue > rsitop && Symbol.Bid > bol.Top.LastValue)
                {
                    Open(TradeType.Sell);
                    //Close(TradeType.Buy);
                }
                else if (rsi.Result.LastValue < rsibottom && Symbol.Ask < bol.Bottom.LastValue)
                {
                    Open(TradeType.Buy);
                    //Close(TradeType.Sell);
                }
            }
//Semi trend trading algo
            if (gradientDifference(oneBOL) > upperbound || gradientDifference(oneBOL) < lowerbound)
            {
                //need to add close posn condition else will screw up
                if (Symbol.Ask > bol.Main.LastValue)
                {
                    trendOpen(TradeType.Buy);
                }
                if (Symbol.Bid < bol.Main.LastValue)
                {
                    trendOpen(TradeType.Sell);
                }

            }
            //trailing stoploss for trend
            var position = Positions.Find(Label1);

            if (position == null)
                return;

            if (position.TradeType == TradeType.Buy)
            {
                double distance = Symbol.Bid - position.EntryPrice;

                if (distance >= Trigger * Symbol.PipSize)
                {
                    if (!_isTrigerred)
                    {
                        _isTrigerred = true;
                        Print("Trailing Stop Loss triggered...");
                    }

                    double newStopLossPrice = Math.Round(Symbol.Bid - TrailingStop * Symbol.PipSize, Symbol.Digits);

                    if (position.StopLoss == null || newStopLossPrice > position.StopLoss)
                    {
                        ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                    }
                }
            }
            else
            {
                double distance = position.EntryPrice - Symbol.Ask;

                if (distance >= Trigger * Symbol.PipSize)
                {
                    if (!_isTrigerred)
                    {
                        _isTrigerred = true;
                        Print("Trailing Stop Loss triggered...");
                    }

                    double newStopLossPrice = Math.Round(Symbol.Ask + TrailingStop * Symbol.PipSize, Symbol.Digits);

                    if (position.StopLoss == null || newStopLossPrice < position.StopLoss)
                    {
                        ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                    }
                }
            }
            //for reversion
            var revPosition = Positions.Find(Label2);

            if (revPosition == null)
                return;

            if (revPosition.TradeType == TradeType.Buy)
            {
                double distance = Symbol.Bid - revPosition.EntryPrice;

                if (distance >= revTrigger * Symbol.PipSize)
                {
                    if (!_isTrigerred)
                    {
                        _isTrigerred = true;
                        Print("Trailing Stop Loss triggered...");
                    }

                    double newStopLossPrice = Math.Round(Symbol.Bid - revTrailingStop * Symbol.PipSize, Symbol.Digits);

                    if (revPosition.StopLoss == null || newStopLossPrice > revPosition.StopLoss)
                    {
                        ModifyPosition(revPosition, newStopLossPrice, revPosition.TakeProfit);
                    }
                }
            }
            else
            {
                double distance = revPosition.EntryPrice - Symbol.Ask;

                if (distance >= revTrigger * Symbol.PipSize)
                {
                    if (!_isTrigerred)
                    {
                        _isTrigerred = true;
                        Print("Trailing Stop Loss triggered...");
                    }

                    double newStopLossPrice = Math.Round(Symbol.Ask + revTrailingStop * Symbol.PipSize, Symbol.Digits);

                    if (revPosition.StopLoss == null || newStopLossPrice < revPosition.StopLoss)
                    {
                        ModifyPosition(revPosition, newStopLossPrice, revPosition.TakeProfit);
                    }
                }
            }

        }
        protected override void OnBar()
        {

        }
        private void Close(TradeType tradeType)
        {
            foreach (var position in Positions.FindAll("rsibol", Symbol, tradeType))
                ClosePosition(position);
        }

        private void Open(TradeType tradeType)
        {
            var position = Positions.Find("rsibol", Symbol, tradeType);

            if (position == null)
                ExecuteMarketOrder(tradeType, Symbol, lotsize, "rsibol", sl, tp);
        }
        private void trendOpen(TradeType tradeType)
        {
            var position = Positions.Find("trend", Symbol, tradeType);

            if (position == null)
                ExecuteMarketOrder(tradeType, Symbol, lotsize, "trend", semiTrendSL, semiTrendTP);
        }

        private int LotScale(int lotsize)
        {
            //mod the inside.. from 0 onwards. return what value
            return 0;
        }

        //Compute pattern of top - bottom bollinger bands
        private double actual(double[] x)
        {
            double tot = 0.0;

            for (int i = 0; i < x.Length; i++)
            {
                tot += x[i];
            }
            double avg = tot / x.Length;
            return (avg);

        }
        private double gradientDifference(double[] x)
        {
            //Note: array values are in %, aim to sum to 0
            double sum = 0.0;

            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i];
            }
            return (sum);
        }

        //Sample patterns

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }


    }
}
