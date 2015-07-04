using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class CounterTrade : Robot
    {
        private const string Label = "CounterTrade";

        protected override void OnStart()
        {
            Positions.Opened += OnPositionsOpened;
        }

        void OnPositionsOpened(PositionOpenedEventArgs args)
        {
            var originalPosition = args.Position;
            if (originalPosition.Label != Label)
            {
                var tradeType = originalPosition.TradeType == TradeType.Buy ? TradeType.Sell : TradeType.Buy;
                ExecuteMarketOrder(tradeType, Symbol, originalPosition.Volume, Label);
            }
        }
    }
}
