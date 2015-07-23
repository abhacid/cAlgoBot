// -------------------------------------------------------------------------------------------------
//
//    This code is a cAlgo API sample.
//
//    This cBot is intended to be used as a sample and does not guarantee any particular outcome or
//    profit of any kind. Use it at your own risk
//
//    The "Sample Martingale cBot" creates a random Sell or Buy order. If the Stop loss is hit, a new 
//    order of the same type (Buy / Sell) is created with double the Initial Volume amount. The cBot will 
//    continue to double the volume amount for  all orders created until one of them hits the take Profit. 
//    After a Take Profit is hit, a new random Buy or Sell order is created with the Initial Volume amount.
//
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Alembex : Robot
    {
        [Parameter("Initial Volume", DefaultValue = 10000, MinValue = 0)]
        public int InitialVolume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 40)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 40)]
        public int TakeProfit { get; set; }

        private Random random = new Random();
        private long pertesVolume = 0;
        private int nombreDeGain = 0;

        protected override void OnStart()
        {
            Positions.Closed += OnPositionsClosed;
            pertesVolume = InitialVolume;

            ExecuteOrder(InitialVolume, GetRandomTradeType());
        }

        private void ExecuteOrder(long volume, TradeType tradeType)
        {
            var result = ExecuteMarketOrder(tradeType, Symbol, volume, "Alembex", StopLoss, TakeProfit);

            if (result.Error == ErrorCode.NoMoney)
                Stop();
        }

        private void OnPositionsClosed(PositionClosedEventArgs args)
        {
            var position = args.Position;

            if (position.Label != "Alembex" || position.SymbolCode != Symbol.Code)
                return;

            if (position.GrossProfit > 0)
            {
                nombreDeGain += 1;
                Print("Gain");

                if (nombreDeGain == 1)
                {
                    Print("Premier Gain (Cumul de pertes depuis le dernier gain : ", pertesVolume, ")");
                    ExecuteOrder(pertesVolume, GetRandomTradeType());
                }
                else
                {
                    ExecuteOrder(InitialVolume, GetRandomTradeType());
                }

                pertesVolume = InitialVolume;
            }
            else
            {
                nombreDeGain = 0;
                Print("Perte");
                pertesVolume += position.Volume;
                ExecuteOrder((int)position.Volume, position.TradeType);
            }

            Print("Volume : ", position.Volume);
            Print("Volume pertes : ", pertesVolume);
            Print("---");
        }

        private TradeType GetRandomTradeType()
        {
            return random.Next(2) == 0 ? TradeType.Buy : TradeType.Sell;
        }
    }
}
