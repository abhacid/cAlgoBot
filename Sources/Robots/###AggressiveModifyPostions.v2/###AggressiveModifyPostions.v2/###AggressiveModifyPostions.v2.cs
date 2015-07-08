
// -------------------------------------------------------------------------------
//
//    Modify all TakeProfit and StopLoss by Average Price. "Aggressive Trading"
//    salr22@hotmail.com, www.borsat.net
//    AggressiveModifyPositions.v2
// -------------------------------------------------------------------------------


using System;
using cAlgo.API;
using System.Linq;


namespace cAlgo.Robots
{
    [Robot()]
    public class AggressiveModifyPostions : Robot
    {

        [Parameter("TakeProfit (pips)", DefaultValue = 10.0)]
        public int _takeProfit { get; set; }

        [Parameter("StopLoss (pips)", DefaultValue = 50.0)]
        public int _stopLoss { get; set; }

        [Parameter("TakeProfit_Buy (price)", DefaultValue = 0)]
        public double _takeProfitPrBuy { get; set; }

        [Parameter("StopLoss_Buy (price)", DefaultValue = 0)]
        public double _stopLossPrBuy { get; set; }

        [Parameter("TakeProfit_Sell (price)", DefaultValue = 0)]
        public double _takeProfitPrSell { get; set; }

        [Parameter("StopLoss_Sell (price)", DefaultValue = 0)]
        public double _stopLossPrSell { get; set; }


        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;

            double _avrgB = 0;
            double _avrgS = 0;
            double _lotsB = 0;
            double _lotsS = 0;

            double _tpB = 0;
            double _slB = 0;
            double _tpS = 0;
            double _slS = 0;

            // get average 
            foreach (var position1 in Account.Positions)
            {

                if (Symbol.Code == position1.SymbolCode)
                {

                    if (position1.TradeType == TradeType.Buy)
                    {
                        _avrgB = _avrgB + (position1.Volume * position1.EntryPrice);
                        _lotsB = _lotsB + position1.Volume;
                    }

                    if (position1.TradeType == TradeType.Sell)
                    {
                        _avrgS = _avrgS + (position1.Volume * position1.EntryPrice);
                        _lotsS = _lotsS + position1.Volume;
                    }

                }

            }

            _avrgB = Math.Round(_avrgB / _lotsB, Symbol.Digits);
            _avrgS = Math.Round(_avrgS / _lotsS, Symbol.Digits);

            string text = string.Format("{0}", "\n\n" + "Average Buy:  " + _avrgB + "\n\n" + "Average Sell:  " + _avrgS);
            ChartObjects.DrawText("avrg", text.PadLeft(10), StaticPosition.TopLeft, Colors.White);

            if (_takeProfit != 0)
            {
                _tpB = Math.Round(_avrgB + Symbol.PipSize * _takeProfit, Symbol.Digits);
                _tpS = Math.Round(_avrgS - Symbol.PipSize * _takeProfit, Symbol.Digits);
            }
            if (_stopLoss != 0)
            {
                _slB = Math.Round(_avrgB - Symbol.PipSize * _stopLoss, Symbol.Digits);
                _slS = Math.Round(_avrgS + Symbol.PipSize * _stopLoss, Symbol.Digits);
            }

            if (_takeProfitPrBuy != 0)
            {
                _tpB = _takeProfitPrBuy;
            }
            if (_stopLossPrBuy != 0)
            {
                _slB = _stopLossPrBuy;
            }
            if (_takeProfitPrSell != 0)
            {
                _tpS = _takeProfitPrSell;
            }
            if (_stopLossPrSell != 0)
            {
                _slS = _stopLossPrSell;
            }



            // modify 
            foreach (Position position in Account.Positions.Where(position => Symbol.Code == position.SymbolCode))
            {
                double sl = position.TradeType == TradeType.Buy ? _slB : _slS;
                double tp = position.TradeType == TradeType.Buy ? _tpB : _tpS;

                if (position.TakeProfit == tp && position.StopLoss == sl)
                    continue;

                if (position.TakeProfit != tp && position.StopLoss != sl)
                {
                    if (!tp.Equals(0.0) && !sl.Equals(0.0))
                        Trade.ModifyPosition(position, sl, tp);

                    else if (tp.Equals(0.0) && sl.Equals(0.0))
                        Trade.ModifyPosition(position, null, null);

                    else if (tp.Equals(0.0) && !sl.Equals(0.0))
                        Trade.ModifyPosition(position, sl, null);
                    else
                        Trade.ModifyPosition(position, null, tp);
                    continue;
                }
                // md tp
                if (position.TakeProfit != tp && position.StopLoss == sl)
                {
                    if (!tp.Equals(0.0))
                        Trade.ModifyPosition(position, position.StopLoss, tp);
                    else
                        Trade.ModifyPosition(position, position.StopLoss, null);
                    continue;
                }

                //md sl
                if (position.TakeProfit == tp && position.StopLoss != sl)
                    if (!sl.Equals(0.0))
                        Trade.ModifyPosition(position, sl, position.TakeProfit);
                    else if (tp.Equals(0.0) && sl.Equals(0.0))
                        Trade.ModifyPosition(position, null, position.TakeProfit);

            }


        }

    }
}
