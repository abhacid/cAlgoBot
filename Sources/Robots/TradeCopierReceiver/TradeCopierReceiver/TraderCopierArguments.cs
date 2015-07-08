using System;
using cAlgo.API;

namespace cAlgo
{
    internal class TraderCopierArguments
    {
        private const int TicketIndex = 0;
        private const int SymbolIndex = 1;
        private const int VolumeIndex = 2;
        private const int StopLossIndex = 4;
        private const int TakeprofitIndex = 5;
        private const int TradeTypeIndex = 3;

        private TraderCopierArguments(string ticket, string symbolCode, double volume, TradeType tradeType,
            double stopLossAbs, double takeProfitAbs)
        {
            Ticket = ticket;
            SymbolCode = symbolCode;
            Volume = volume;
            TradeType = tradeType;
            StopLossAbs = stopLossAbs == 0 ? (double?) null : stopLossAbs;
            TakeProfitAbs = takeProfitAbs == 0 ? (double?) null : takeProfitAbs;
        }

        public string Ticket { get; private set; }
        public string SymbolCode { get; private set; }
        public double Volume { get; private set; }
        public TradeType TradeType { get; private set; }
        public double? StopLossAbs { get; private set; }
        public double? TakeProfitAbs { get; private set; }

        public static TraderCopierArguments Parse(string line)
        {
            var args = line.Split(',');
            if (args.Length != 6)
                throw new ArgumentException("Arguments mismatch at line: " + line);

            return new TraderCopierArguments(
                args[TicketIndex],
                args[SymbolIndex],
                double.Parse(args[VolumeIndex]),
                GetTradeType(args),
                double.Parse(args[StopLossIndex]),
                double.Parse(args[TakeprofitIndex]));
        }

        public static TradeType GetTradeType(string[] arguments)
        {
            var tradeTypeValue = arguments[TradeTypeIndex];
            switch (tradeTypeValue)
            {
                case "0":
                    return TradeType.Buy;
                case "1":
                    return TradeType.Sell;
                default:
                    throw new ArgumentOutOfRangeException("Unknown type of trade type: '" + tradeTypeValue + "'");
            }
        }
    }
}