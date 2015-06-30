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

#region cBot Infos
// -------------------------------------------------------------------------------
//
//		ZephynScalper 
//
// -------------------------------------------------------------------------------

#endregion

#region advertisement
// -------------------------------------------------------------------------------
//			Trading using leverage carries a high degree of risk to your capital, and it is possible to lose more than
//			your initial investment. Only speculate with money you can afford to lose.
// -------------------------------------------------------------------------------
#endregion


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

        [Parameter("Periods SMA", DefaultValue = 35)]
        public int Periods_SMA { get; set; }

        [Parameter("Source SMA")]
        public DataSeries Source_SMA { get; set; }

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
        //private int OpenTrades = 0;
        double tradePrice;

        private SimpleMovingAverage _SMA;
        private StochasticOscillator _SOC;

        protected override void OnStart()
        {
            _SMA = Indicators.SimpleMovingAverage(Source_SMA, Periods_SMA);
            _SOC = Indicators.StochasticOscillator(KPeriods, K_Slowing, DPeriods, MaType);

            Positions.Closed += OnPositionClosed;
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


            foreach (var position in Positions.FindAll("ZephynScalper", Symbol, TradeType.Buy))
            {
                if (position.StopLoss.HasValue && (position.Pips > Trail_start))
                {
                    if (tradePrice < Symbol.Bid)
                    {
                        //double NewStopLoss = Symbol.Ask - Trail * Symbol.PipSize;   // c'est un buy et le stop est reculé, alors que ce devrait être le contraire
                        // D'autre part c'est le stop qui doit être rapproché de Trail points d'où la mofification effectuée. https://www.facebook.com/ab.hacid 

                        double NewStopLoss = position.StopLoss.Value + Trail * Symbol.PipSize;
                        ModifyPosition(position, NewStopLoss, position.TakeProfit.Value);
                        tradePrice = Symbol.Ask;

                        //Print("Stop loss: " + NewStopLoss);
                        //Print("Current price: " + Symbol.Ask);
                    }
                }
            }

            foreach (var position in Positions.FindAll("ZephynScalper", Symbol, TradeType.Sell))
            {
                if (position.StopLoss.HasValue && (position.Pips > Trail_start))
                {
                    if (tradePrice > Symbol.Ask)
                    {
                        //double NewStopLoss = Symbol.Bid + Trail * Symbol.PipSize;

                        double NewStopLoss = position.StopLoss.Value - Trail * Symbol.PipSize;
                        ModifyPosition(position, NewStopLoss, position.TakeProfit.Value);
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
            if (Positions.Count < 2)
                ExecuteMarketOrder(tradeType, Symbol, Volume, "ZephynScalper", StopLoss, TakeProfit, null, "ZephynScalper");


        }

        private void OnPositionClosed(PositionClosedEventArgs args)
        {
            var position = args.Position;
            //Print("Position closed with {0} profit", position.GrossProfit);
            //if (position.Label.ToString() == "ZephynScalper")
            //{
            //    OpenTrades--;
            //}

            if (position.TradeType == TradeType.Sell)
            {
                Sellingongoing = false;
            }

            if (position.TradeType == TradeType.Buy)
            {
                Buyingongoing = false;
            }

            //if (OpenTrades < 0)
            //    OpenTrades = 0;

        }


        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
