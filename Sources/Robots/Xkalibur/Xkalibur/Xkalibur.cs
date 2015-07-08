//Date: 02/12/2014
//Country: Chile
//Copyright: Felipe Sepulveda Maldonado 
//LinkedIn: https://cl.linkedin.com/in/felipesepulvedamaldonado
//Facebook: https://www.facebook.com/mymagicflight1
//Whats Up: +56 9 58786321
//Donations Wallet: wallet.google.com felipe.sepulveda@gmail.com
//
//Recomended Timeframe: Minute, for more frequency and accuracy.
//Cheers!

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Xkalibur : Robot
    {
        [Parameter("Volume", DefaultValue = 2000, MinValue = 1000)]
        public int InitialVolume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 16, MinValue = 4, MaxValue = 32)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 4, MinValue = 3, MaxValue = 12)]
        public int TakeProfit { get; set; }

        [Parameter("WilliamsR High", DefaultValue = 85, MinValue = 80, MaxValue = 99)]
        public int wHigh { get; set; }

        [Parameter("WilliamsR Low", DefaultValue = 15, MinValue = 1, MaxValue = 20)]
        public int wLow { get; set; }

        [Parameter("WilliamsR Period", DefaultValue = 14, MinValue = 7, MaxValue = 21)]
        public int wPeriod { get; set; }

        [Parameter("True Range Impact", DefaultValue = 20, MinValue = 15, MaxValue = 100)]
        public int trImpact { get; set; }

        protected override void OnStart()
        {
            // Put your initialization logic here
        }

        protected override void OnTick()
        {
            // Put your core logic here
            double will = 100 + Indicators.WilliamsPctR(wPeriod).Result.Last(0);
            double impact = 100000 * Indicators.TrueRange(MarketSeries).Result.Last(0);

            var text = "Impacto: " + impact.ToString();
            text += "\nWilliamsR%: " + will.ToString() + "\n Timeframe: " + MarketSeries.TimeFrame.ToString();

            ChartObjects.DrawText("", text, StaticPosition.TopLeft, Colors.White);

            if ((will < wLow) && (impact > trImpact))
            {
                var result = ExecuteMarketOrder(TradeType.Buy, Symbol, InitialVolume, "bot", StopLoss, TakeProfit, 2, "comentario");
                if (!result.IsSuccessful)
                    Print("error : {0}, {1}", result.Error, InitialVolume);
            }

            if ((will > wHigh) && (impact > trImpact))
            {
                var result = ExecuteMarketOrder(TradeType.Sell, Symbol, InitialVolume, "bot", StopLoss, TakeProfit, 2, "comentario");
                if (!result.IsSuccessful)
                    Print("error : {0}, {1}", result.Error, InitialVolume);
            }

            System.Threading.Thread.Sleep(2000);
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
