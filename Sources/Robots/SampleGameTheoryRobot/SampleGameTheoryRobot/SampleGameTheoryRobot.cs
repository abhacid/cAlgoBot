// -------------------------------------------------------------------------------
//
//   Simple example with use of  "Games Theory".
//   Vsoft(c).
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Requests;
using cAlgo.Indicators;

namespace cAlgo.Robots
{

    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SampleGameTheoryRobot : Robot
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("Band Height", DefaultValue = 1.0, MinValue = 0)]
        public double BandHeight { get; set; }

        [Parameter("Stop Loss", DefaultValue = 10, MinValue = 1)]
        public int StopLossInPips { get; set; }

        [Parameter("Take Profit", DefaultValue = 10, MinValue = 1)]
        public int TakeProfitInPips { get; set; }

        [Parameter("Volume", DefaultValue = 10000, MinValue = 1000)]
        public int Volume { get; set; }

        [Parameter("Bollinger Bands Deviations", DefaultValue = 2)]
        public int Deviations { get; set; }
        // BB
        [Parameter("Bollinger Bands Periods", DefaultValue = 20)]
        public int Periods { get; set; }
        // BB
        [Parameter("Bollinger Bands MA Type")]
        public MovingAverageType MAType { get; set; }
        // BB
        [Parameter("Domination Periods", DefaultValue = 1)]
        public int DominationPeriods { get; set; }
        // Definition a dominant.



        private BollingerBands bollingerBands;
        private int Domination;


        protected override void OnStart()
        {
            bollingerBands = Indicators.BollingerBands(Source, Periods, Deviations, MAType);
        }

        protected override void OnBar()
        {
            double top = bollingerBands.Top.LastValue;
            double bottom = bollingerBands.Bottom.LastValue;


            if ((top - bottom) / Symbol.PipSize <= BandHeight)
            {
                Domination = Domination + 1;
            }
            else
            {
                Domination = 0;
            }

            if (Domination >= DominationPeriods)
            {
                if (Symbol.Ask > top && MarketSeries.Close[MarketSeries.Close.Count - 2] > MarketSeries.Open[MarketSeries.Close.Count - 2])
                {
                    var request = new MarketOrderRequest(TradeType.Buy, Volume) 
                    {
                        Label = "SampleGameTheory",
                        StopLossPips = StopLossInPips,
                        TakeProfitPips = TakeProfitInPips
                    };

                    Trade.Send(request);

                    Domination = 0;
                }
                else if (Symbol.Bid < bottom && MarketSeries.Close[MarketSeries.Close.Count - 2] < MarketSeries.Open[MarketSeries.Close.Count - 2])
                {
                    var request = new MarketOrderRequest(TradeType.Sell, Volume) 
                    {
                        Label = "SampleGameTheory",
                        StopLossPips = StopLossInPips,
                        TakeProfitPips = TakeProfitInPips
                    };

                    Trade.Send(request);

                    Domination = 0;
                }
            }
        }


    }

}
