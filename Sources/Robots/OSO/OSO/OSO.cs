using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class OSO : Robot
    {
        [Parameter("Order #1 Type")]
        public string _ORType1 { get; set; }
        [Parameter("Order #1 Price", DefaultValue = 1.0)]
        public double _Price1 { get; set; }
        [Parameter("Order #1 Size", DefaultValue = 10000)]
        public int _Size1 { get; set; }
        [Parameter("Order #1 SL", DefaultValue = 20)]
        public int _SL1 { get; set; }
        [Parameter("Order #1 TP", DefaultValue = 40)]
        public int _TP1 { get; set; }

        //---

        [Parameter("Order #2 Type")]
        public string _ORType2 { get; set; }
        [Parameter("Order #2 Price", DefaultValue = 1.0)]
        public double _Price2 { get; set; }
        [Parameter("Order #2 Size", DefaultValue = 10000)]
        public int _Size2 { get; set; }
        [Parameter("Order #2 SL", DefaultValue = 20)]
        public int _SL2 { get; set; }
        [Parameter("Order #1 TP", DefaultValue = 40)]
        public int _TP2 { get; set; }


        string _Label = "mylabel";

        protected string GetLabel(string _TType)
        {
            Random rn = new Random(10);
            int x = rn.Next(100, 10000);
            _Label = _TType + x.ToString();

            var _CLD = History.FindLast(_Label);
            var _CLO = Positions.Find(_Label);
            while (_CLD != null || _CLO != null)
            {
                x++;
                _Label = _TType + x.ToString();
                _CLD = History.FindLast(_Label);
                _CLO = Positions.Find(_Label);
                //Theres a duplicated Label, finding another one
            }
            return _Label;
        }

        protected override void OnStart()
        {
            _Label = GetLabel(Symbol.Code);

            if (_ORType1 == "sell" || _ORType1 == "Sell" || _ORType1 == "SELL")
            {
                if (_Price1 > MarketSeries.Close.LastValue)
                {
                    PlaceLimitOrder(TradeType.Sell, Symbol, _Size1, _Price1, _Label, _SL1, _TP1);
                }
                else
                {
                    PlaceStopOrder(TradeType.Sell, Symbol, _Size1, _Price1, _Label, _SL1, _TP1);
                }
            }
            else if (_ORType1 == "buy" || _ORType1 == "Buy" || _ORType1 == "BUY")
            {
                if (_Price1 > MarketSeries.Close.LastValue)
                {
                    PlaceStopOrder(TradeType.Buy, Symbol, _Size1, _Price1, _Label, _SL1, _TP1);
                }
                else
                {
                    PlaceLimitOrder(TradeType.Buy, Symbol, _Size1, _Price1, _Label, _SL1, _TP1);
                }

            }
            else
            {
                Print("Parameter not recognized, bot will stop");
                Stop();
            }

        }

        protected override void OnTick()
        {
            Position _GetPos = Positions.Find(_Label);

            if (_GetPos != null)
            {
                if (_ORType2 == "sell" || _ORType2 == "Sell" || _ORType2 == "SELL")
                {
                    if (_Price2 > Symbol.Bid)
                    {
                        PlaceLimitOrder(TradeType.Sell, Symbol, _Size2, _Price2, _Label, _SL2, _TP2);
                    }
                    else
                    {
                        PlaceStopOrder(TradeType.Sell, Symbol, _Size2, _Price2, _Label, _SL2, _TP2);
                    }
                }
                else if (_ORType2 == "buy" || _ORType2 == "Buy" || _ORType2 == "BUY")
                {
                    if (_Price2 > Symbol.Bid)
                    {
                        PlaceStopOrder(TradeType.Buy, Symbol, _Size2, _Price2, _Label, _SL2, _TP2);
                    }
                    else
                    {
                        PlaceLimitOrder(TradeType.Buy, Symbol, _Size2, _Price2, _Label, _SL2, _TP2);
                    }
                }
                Stop();
            }
        }
    }
}
