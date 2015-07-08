using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class XRMultiEntries : Robot
    {
        [Parameter("Stop loss(pips)", DefaultValue = 5, MinValue = 1)]
        public int SLoss { get; set; }
        [Parameter("Take Profit(pips)", DefaultValue = 100, MinValue = 0)]
        public double TakeProfit { get; set; }
        [Parameter("Max Tries", DefaultValue = 20, MinValue = 2)]
        public double _MaxTries { get; set; }
        [Parameter("Go Long", DefaultValue = true)]
        public bool _GoLong { get; set; }
        [Parameter("Long Start Level", DefaultValue = 1.0)]
        public double _LongPrice { get; set; }
        [Parameter("Go Short", DefaultValue = true)]
        public bool _GoShort { get; set; }
        [Parameter("Short Start Level", DefaultValue = 1.0)]
        public double _ShortPrice { get; set; }
        [Parameter("Position Size", DefaultValue = 10000)]
        public int _Volume { get; set; }
        [Parameter("Increment Position Size x number of times", DefaultValue = false)]
        public bool _IPS { get; set; }
        //Increment Position Size
        [Parameter("Increment Volume Each # of try", DefaultValue = 4)]
        public int _TryN { get; set; }
        [Parameter("OCO Mode", DefaultValue = false)]
        public bool _OCOMode { get; set; }
        [Parameter("Close Orders on Bot Stop", DefaultValue = true)]
        public bool _CloseOnStop { get; set; }

        int _LCurrentTries = 0, _SCurrentTries = 0;

        int _LTryN;
        int _STryN;
        int _SVolume;
        int _LVolume;

        int _LongX = 0, _ShortX = 0;
        // Long and Short Auxiliar
        int _LongLX = 0, _ShortLX = 0;
        // Long and Short Limit Auxiliar
        int x;
        double _LTCProfit, _STCProfit;
        //For total profit
        string _LLabel = "Llabel", _SLabel = "_SLabel";
        Random rn = new Random();
        bool _LModeS = false, _LModeL = false;
        // Limit Mode


        protected override void OnStart()
        {

            _LTryN = _TryN;
            _STryN = _TryN;
            _SVolume = _Volume;
            _LVolume = _Volume;

            x = rn.Next(99, 9999);
            _SLabel = "S" + Symbol.Code + x.ToString();
            _LLabel = "L" + Symbol.Code + x.ToString();

            if (_GoShort)
            {
                var _SCLD = History.FindLast(_SLabel);
                var _SCLO = Positions.Find(_SLabel);

                while (_SCLD != null || _SCLO != null)
                {
                    x++;
                    _SLabel = "S" + Symbol.Code + x.ToString();
                    _SCLD = History.FindLast(_SLabel);
                    _SCLO = Positions.Find(_SLabel);
                    Print("Theres a duplicated Label, finding another one.");
                }

                Print("Short Label is: ", _SLabel);
                ChartObjects.DrawHorizontalLine(_SLabel, _ShortPrice, Colors.Red, 2, LineStyle.DotsVeryRare);
            }

            if (_GoLong)
            {

                var _LCLD = History.FindLast(_LLabel);
                var _LCLO = Positions.Find(_LLabel);

                while (_LCLD != null || _LCLO != null)
                {
                    x++;
                    _LLabel = "L" + Symbol.Code + x.ToString();
                    _LCLD = History.FindLast(_LLabel);
                    _LCLO = Positions.Find(_LLabel);
                    Print("Theres a duplicated Label, finding another one.");
                }

                Print("Long Label is: ", _LLabel);
                ChartObjects.DrawHorizontalLine(_LLabel, _LongPrice, Colors.SteelBlue, 2, LineStyle.DotsVeryRare);
            }

            if (Symbol.Ask > _LongPrice)
            {
                _LModeL = true;
                Print("Long Limit Mode is On Play");
            }

            if (Symbol.Bid < _ShortPrice)
            {
                _LModeS = true;
                Print("Short Limit Mode is On Play");
            }
        }

        protected override void OnTick()
        {
            if (_GoLong)
            {
                // Find Number of Open Positions With defined Label (Stop Mode)
                var _LNOP = Positions.FindAll(_LLabel);
                if (_LNOP != null)
                {
                    foreach (var lnop in _LNOP)
                    {
                        _LongX++;
                    }
                }
                // Find Number of Pending Orders with defined Label (Limit Mode)
                if (PendingOrders.Count != 0)
                {
                    foreach (var _FPP in PendingOrders)
                    {
                        if (_FPP != null)
                        {
                            if (_FPP.Label == _LLabel)
                            {
                                _LongLX++;
                            }
                        }
                        else
                        {
                            _LongLX++;
                        }
                    }
                }
                else
                {
                    _LongLX++;
                }
                // Find if last position with this Label was profitable
                var _FLPP = History.FindLast(_LLabel);
                //Find Last Position Profit
                if (_FLPP != null)
                {
                    if (_FLPP.NetProfit > 0)
                    {
                        _LongX++;
                        _LongLX++;
                        Stop();
                    }
                }
                //If there is no Position then Draw a line (Stop Mode)
                if (_LongX == 0 && Symbol.Ask <= _LongPrice && !_LModeL)
                {
                    ChartObjects.DrawHorizontalLine(_LLabel, _LongPrice, Colors.SteelBlue, 2, LineStyle.DotsVeryRare);
                }
                //If there is no Position then Draw a line (Limit Mode)
                if (_LongLX == 0 && Symbol.Ask >= _LongPrice && _LModeL)
                {
                    ChartObjects.DrawHorizontalLine(_LLabel, _LongPrice, Colors.SteelBlue, 2, LineStyle.DotsVeryRare);
                }
                //Execute Order - First the Stop Mode
                if (Symbol.Ask >= _LongPrice && _LongX == 0 && _LCurrentTries < _MaxTries && !_LModeL)
                {
                    ExecuteMarketOrder(TradeType.Buy, Symbol, _LVolume, _LLabel, SLoss, TakeProfit);
                    _LCurrentTries++;
                    ChartObjects.RemoveObject(_LLabel);
                    if (_OCOMode)
                    {
                        _GoShort = false;
                        ChartObjects.RemoveObject(_SLabel);
                    }
                }
                //Execute Order - Limit Mode
                if (Symbol.Ask >= _LongPrice && _LongLX == 0 && _LCurrentTries < _MaxTries && _LModeL)
                {
                    PlaceLimitOrder(TradeType.Buy, Symbol, _LVolume, _LongPrice, _LLabel, SLoss, TakeProfit);
                    _LCurrentTries++;
                    ChartObjects.RemoveObject(_LLabel);
                    if (_OCOMode)
                    {
                        _GoShort = false;
                        ChartObjects.RemoveObject(_SLabel);
                    }
                }

                //Find Total Profit on Positions Closed
                var _LPCO = History.FindAll(_LLabel);
                foreach (var lpco in _LPCO)
                {
                    _LTCProfit += lpco.NetProfit;
                }
                //Add profit of current position
                if (_LongLX == 1 || _LongX == 1)
                {
                    var _LFCP = Positions.Find(_LLabel);
                    if (_LFCP != null)
                    {
                        _LTCProfit += _LFCP.NetProfit;
                    }
                }

                ChartObjects.DrawText("Long Current Profit", "Long: Current Net Profit: " + Math.Round(_LTCProfit, 2).ToString() + " || Number of Tries: " + _LCurrentTries.ToString(), StaticPosition.TopRight, Colors.SteelBlue);
                _LTCProfit = 0;
                _LongX = 0;
                _LongLX = 0;
            }

            // Short Mode

            if (_GoShort)
            {
                //Find Number of Positions
                var _SNOP = Positions.FindAll(_SLabel);
                if (_SNOP != null)
                {
                    foreach (var snop in _SNOP)
                    {
                        _ShortX++;
                    }
                }
                //Find Number of Pending Orders
                //Find Number of Short Pendings
                if (PendingOrders.Count != 0)
                {
                    foreach (var _FNSP in PendingOrders)
                    {
                        if (_FNSP != null)
                        {
                            if (_FNSP.Label == _SLabel)
                            {
                                _ShortLX++;
                                //Print("ShortLX is {0}", _ShortLX);
                            }
                        }
                        else
                        {
                            _ShortLX++;
                            //Print("ShortLX is {0}", _ShortLX);
                        }
                    }
                }
                else
                {
                    _ShortLX++;
                }
                // Find if last position with this Label was profitable
                var _FLPPS = History.FindLast(_SLabel);
                //Find Last Position Profit
                if (_FLPPS != null)
                {
                    if (_FLPPS.NetProfit > 0)
                    {
                        _ShortX++;
                        _ShortLX++;
                        Stop();
                    }
                }

                if (_ShortX == 0 && Symbol.Bid >= _ShortPrice && !_LModeS)
                {
                    ChartObjects.DrawHorizontalLine(_SLabel, _ShortPrice, Colors.Red, 2, LineStyle.DotsVeryRare);
                }

                if (_ShortLX == 0 && Symbol.Bid <= _ShortPrice && _LModeS)
                {
                    ChartObjects.DrawHorizontalLine(_SLabel, _ShortPrice, Colors.Red, 2, LineStyle.DotsVeryRare);
                }

                //First Stop Mode
                if (Symbol.Bid <= _ShortPrice && _ShortX == 0 && _SCurrentTries < _MaxTries && !_LModeS)
                {
                    ExecuteMarketOrder(TradeType.Sell, Symbol, _SVolume, _SLabel, SLoss, TakeProfit);
                    _SCurrentTries++;
                    ChartObjects.RemoveObject(_SLabel);
                    if (_OCOMode)
                    {
                        _GoLong = false;
                        ChartObjects.RemoveObject(_LLabel);
                    }
                }
                //Limit Mode
                if (Symbol.Bid >= _ShortPrice && _ShortLX == 0 && _SCurrentTries < _MaxTries && !_LModeS)
                {
                    PlaceLimitOrder(TradeType.Sell, Symbol, _SVolume, _ShortPrice, _SLabel, SLoss, TakeProfit);
                    _SCurrentTries++;
                    ChartObjects.RemoveObject(_SLabel);
                    if (_OCOMode)
                    {
                        _GoLong = false;
                        ChartObjects.RemoveObject(_LLabel);
                    }
                }

                var _SPCO = History.FindAll(_SLabel);
                foreach (var spco in _SPCO)
                {
                    _STCProfit += spco.NetProfit;
                }

                // Short - Find Current Profit
                if (_ShortX == 0 || _ShortLX == 0)
                {
                    var _SFCP = Positions.Find(_SLabel);
                    if (_SFCP != null)
                    {
                        _STCProfit += _SFCP.NetProfit;
                    }
                }

                ChartObjects.DrawText("Short Current Profit", "Short: Current Net Profit: " + Math.Round(_STCProfit, 2).ToString() + " || Number of Tries: " + _SCurrentTries.ToString(), StaticPosition.TopLeft, Colors.Red);

                _STCProfit = 0;
                _ShortX = 0;
                _ShortLX = 0;
            }

            if (_IPS)
            {
                if (_SCurrentTries == _STryN)
                {
                    _SVolume += _Volume;
                    _STryN += _TryN;
                }

                if (_LCurrentTries == _LTryN)
                {
                    _LVolume += _Volume;
                    _LTryN += _TryN;
                }
            }
        }


        protected override void OnStop()
        {
            /*try
            {
                OnTick();
            } catch (Exception e)
            {
                Print("{0} ", e.StackTrace);
            }*/

            if (_CloseOnStop)
            {
                var Lclose = Positions.FindAll(_LLabel);
                if (Lclose != null)
                {
                    foreach (var Lclosin in Lclose)
                    {
                        ClosePosition(Lclosin);
                    }
                }

                var Sclose = Positions.FindAll(_SLabel);
                if (Sclose != null)
                {
                    foreach (var Sclosin in Sclose)
                    {
                        ClosePosition(Sclosin);
                    }
                }

                //Close All Pending Orders
                foreach (var _CAPO in PendingOrders)
                {
                    if (_CAPO.Label == _SLabel || _CAPO.Label == _LLabel)
                    {
                        CancelPendingOrder(_CAPO);
                    }
                }
            }
        }
    }
}
