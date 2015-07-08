using System.Collections.Generic;
using System.Linq;
using cAlgo.API;

namespace cAlgo
{
    internal class ExecutionService
    {
        private const string MT4CopierPrefix = "MT4Copier_";

        private readonly TradeCopierReceiver _robot;
        private readonly string[] _symbolsFilterArray;
        private IEnumerable<TraderCopierArguments> _copierArgumentses;
        private bool _noMoney = false;

        public ExecutionService(TradeCopierReceiver robot)
        {
            _robot = robot;
            _symbolsFilterArray = string.IsNullOrWhiteSpace(robot.SymbolsFilter)
                                      ? new string[0]
                                      : robot.SymbolsFilter.Split(',')
                                             .Select(x => x.ToLowerInvariant().Trim())
                                             .ToArray();
        }

        public int ExecutedOperationsCount { get; private set; }

        public bool Execute(IEnumerable<TraderCopierArguments> copierArgumentses)
        {
            _copierArgumentses = copierArgumentses;
            ExecutedOperationsCount = 0;

            var executedSuccessfully = true;
            var positionsMap = GetPositionsMapByLabel();

            foreach (var arg in _copierArgumentses)
            {
                if (!ValidateFilter(arg))
                    continue;

                Position[] positionsToModify;
                if (positionsMap.TryGetValue(arg.Ticket, out positionsToModify))
                {
                    foreach (var position in positionsToModify)
                        executedSuccessfully &= UpdatePositionIfNeeded(position, arg);
                    positionsMap.Remove(arg.Ticket);
                }
                else
                {
                    executedSuccessfully &= ExecuteAndModify(arg);
                    if (_noMoney)
                    {
                        _robot.Stop();
                        return false;
                    }
                }
            }

            var positionToCloses = positionsMap.Values.SelectMany(positions => positions);
            foreach (var positionToClose in positionToCloses)
            {
                executedSuccessfully &= _robot.ClosePosition(positionToClose).IsSuccessful;
            }

            return executedSuccessfully;
        }

        private bool ValidateFilter(TraderCopierArguments arg)
        {
            var tradeTypeEnabled = (arg.TradeType == TradeType.Buy && _robot.CopyBuy) ||
                                   (arg.TradeType == TradeType.Sell && _robot.CopySell);
            if (!tradeTypeEnabled)
                return false;

            return _robot.SymbolsFilter.Length == 0 || _symbolsFilterArray.Contains(arg.SymbolCode.ToLowerInvariant());
        }

        private bool ExecuteAndModify(TraderCopierArguments arg)
        {
            var historicalTrade = _robot.History.FindLast(arg.Ticket);
            if (historicalTrade != null)
                return true;

            var symbol = _robot.MarketData.GetSymbol(arg.SymbolCode);
            var normalizedVolume = symbol.NormalizeVolume(arg.Volume);
            var tradeResult = _robot.ExecuteMarketOrder(arg.TradeType, symbol, normalizedVolume,
                                                        MT4CopierPrefix + arg.Ticket);
            if (tradeResult.Error == ErrorCode.NoMoney)
            {
                _noMoney = true;
                return false;
            }
            ExecutedOperationsCount++;
            var success = tradeResult.IsSuccessful;
            if (tradeResult.IsSuccessful)
            {
                success &= UpdatePositionIfNeeded(tradeResult.Position, arg);
            }
            return success;
        }

        private bool UpdatePositionIfNeeded(Position position, TraderCopierArguments arguments)
        {
            if (!_robot.CopyProtectionEnabled)
                return true;

            var stopLossAbs = arguments.StopLossAbs;
            var takeProfitAbs = arguments.TakeProfitAbs;

            var sltpChanged = position.TakeProfit != takeProfitAbs || position.StopLoss != stopLossAbs;

            if (sltpChanged)
            {
                ExecutedOperationsCount++;
                return _robot.ModifyPosition(position, stopLossAbs, takeProfitAbs).IsSuccessful;
            }
            return true;
        }

        private Dictionary<string, Position[]> GetPositionsMapByLabel()
        {
            return
                _robot.Positions.Where(position => position.Label != null && position.Label.StartsWith(MT4CopierPrefix))
                      .ToLookup(position => position.Label.Substring(MT4CopierPrefix.Length), position => position)
                      .ToDictionary(x => x.Key, x => x.ToArray());
        }
    }
}