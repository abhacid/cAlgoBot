using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ZephynScalper : Robot
    {
        [Parameter("Periods SMA", DefaultValue = 35)]
        public int Periods_SMA { get; set; }

        [Parameter("Source SMA")]
        public DataSeries Source_SMA { get; set; }

        [Parameter("TakeProfit", DefaultValue = 30, MinValue = 1)]
        public int TakeProfit { get; set; }

        [Parameter("Stop Loss", DefaultValue = 50, MinValue = 1)]
        public int StopLoss { get; set; }

        [Parameter("Trail start", DefaultValue = 12, MinValue = 1)]
        public int Trail_start { get; set; }
        [Parameter("Trail", DefaultValue = 8, MinValue = 1)]
        public int Trail { get; set; }

        [Parameter("Volume", DefaultValue = 10000, MinValue = 10000)]
        public int Volume { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Parameter("K Periods", DefaultValue = 5)]
        public int KPeriods { get; set; }

        [Parameter("D Periods", DefaultValue = 3)]
        public int DPeriods { get; set; }

        [Parameter("K Slowing", DefaultValue = 3)]
        public int K_Slowing { get; set; }

        private bool Sellingongoing = false;
        private bool Buyingongoing = false;
        private int OpenTrades = 0;
        double tradePrice;

        private SimpleMovingAverage _SMA;
        private StochasticOscillator _SOC;

        protected override void OnStart()
        {
            _SMA = Indicators.SimpleMovingAverage(Source_SMA, Periods_SMA);
            _SOC = Indicators.StochasticOscillator(KPeriods, K_Slowing, DPeriods, MaType);

            Positions.Closed += PositionsOnClosed;
        }


        protected override void OnTick()
        {
            //Print("Ask: " + Symbol.Ask);
            //købs pris

            //Print("Bid: " + Symbol.Bid);
            //salgs pris

            //Print(_SMA.Result.LastValue);
            //Print("D" + _SOC.PercentD.LastValue);
            //Print("K" + _SOC.PercentK.LastValue);


            foreach (var _p in Positions.FindAll("ZephynScalper", Symbol, TradeType.Buy))
            {
                if (_p.Pips > Trail_start)
                {
                    if (tradePrice < Symbol.Bid)
                    {
                        double NewStopLoss = Symbol.Ask - Trail * Symbol.PipSize;
                        ModifyPosition(_p, NewStopLoss, _p.TakeProfit.Value);
                        tradePrice = Symbol.Ask;

                        //Print("Stop loss: " + NewStopLoss);
                        //Print("Current price: " + Symbol.Ask);
                    }
                }
            }

            foreach (var _p2 in Positions.FindAll("ZephynScalper", Symbol, TradeType.Sell))
            {
                if (_p2.Pips > Trail_start)
                {
                    if (tradePrice > Symbol.Ask)
                    {
                        double NewStopLoss = Symbol.Bid + Trail * Symbol.PipSize;
                        ModifyPosition(_p2, NewStopLoss, _p2.TakeProfit.Value);
                        tradePrice = Symbol.Bid;
                    }
                }
            }

            if (_SMA.Result.LastValue < Symbol.Bid)
            {

                if (_SOC.PercentD.LastValue < 20)
                {
                    if (_SOC.PercentK.LastValue < 20)
                    {
                        if (_SOC.PercentK.LastValue > _SOC.PercentD.LastValue)
                        {
                            if (Buyingongoing == false)
                            {
                                Open(TradeType.Buy);
                                tradePrice = Symbol.Bid;
                                Buyingongoing = true;
                            }
                        }
                    }
                }

            }

            else if (_SMA.Result.LastValue > Symbol.Ask)
            {

                if (_SOC.PercentD.LastValue > 80)
                {
                    if (_SOC.PercentK.LastValue > 80)
                    {
                        if (_SOC.PercentK.LastValue < _SOC.PercentD.LastValue)
                        {
                            if (Sellingongoing == false)
                            {
                                Open(TradeType.Sell);
                                tradePrice = Symbol.Ask;
                                Sellingongoing = true;
                            }
                        }
                    }
                }



            }

        }

        private void Open(TradeType tradeType)
        {
            // var position = Positions.Find("ZephynScalper", Symbol, tradeType);

            // if (position == null)
            //     ExecuteMarketOrder(tradeType, Symbol, Volume, "ZephynScalper", StopLoss, TakeProfit);
            if (OpenTrades < 2)
            {
                ExecuteMarketOrder(tradeType, Symbol, Volume, "ZephynScalper", null, TakeProfit);
                OpenTrades++;
            }

        }

        private void PositionsOnClosed(PositionClosedEventArgs args)
        {
            var position = args.Position;
            //Print("Position closed with {0} profit", position.GrossProfit);
            if (position.Label.ToString() == "ZephynScalper")
            {
                OpenTrades--;
            }

            if (position.TradeType == TradeType.Sell)
            {
                Sellingongoing = false;
            }

            if (position.TradeType == TradeType.Buy)
            {
                Buyingongoing = false;
            }

            if (OpenTrades < 0)
                OpenTrades = 0;

        }


        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
