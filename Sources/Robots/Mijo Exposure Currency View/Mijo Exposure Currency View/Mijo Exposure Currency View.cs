using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MijoExposureCurrencyView : Robot
    {
        private int MAX_PAIRS = 100;

        protected override void OnStart()
        {
            Timer.Start(1);
            //Calc_Exposure();
        }

        protected override void OnTick()
        {
            //Calc_Exposure();
        }

        protected override void OnTimer()
        {
            Calc_Exposure();
        }

        void Calc_Exposure()
        {
            string _used = "";
            string n = "\n";
            int _max_pairs = 0;
            int i = 0;
            double[] _exposure = new double[MAX_PAIRS];

            for (i = 0; i < MAX_PAIRS; i++)
            {
                _exposure[i] = 0;
            }

            for (i = (Positions.Count - 1); i >= 0; i--)
            {

                Symbol symbol = MarketData.GetSymbol(Positions[i].SymbolCode);
                string _pair = Positions[i].SymbolCode.Substring(0, 3);
                int _pi = _used.IndexOf(_pair, 0, _used.Length);

                if (_pi >= 0)
                {
                    _pi /= 3;
                }
                else
                {
                    _pi = _used.Length / 3;
                    _used = string.Concat(_used, _pair);
                    _max_pairs++;
                }

                if (Positions[i].TradeType == TradeType.Buy)
                {
                    _exposure[_pi] += Positions[i].Volume * symbol.TickSize;
                }
                else
                {
                    _exposure[_pi] -= Positions[i].Volume * symbol.TickSize;
                }

                _pair = Positions[i].SymbolCode.Substring(3, 3);
                _pi = _used.IndexOf(_pair, 0, _used.Length);

                if (_pi >= 0)
                {
                    _pi /= 3;
                }
                else
                {
                    _pi = _used.Length / 3;
                    _used = string.Concat(_used, _pair);
                    _max_pairs++;
                }

                if (Positions[i].TradeType == TradeType.Sell)
                {
                    _exposure[_pi] += Positions[i].Volume * symbol.TickSize * symbol.Bid;
                }
                else
                {
                    _exposure[_pi] -= Positions[i].Volume * symbol.TickSize * symbol.Ask;
                }
            }

            ChartObjects.DrawText("Mijo_Exposure", n + "-=Exposure=-", StaticPosition.TopLeft, Colors.Yellow);

            for (i = 0; i < _max_pairs; i++)
            {
                n = n + "\n";
                _exposure[i] = _exposure[i] * 1000;

                if (_used.Substring(i * 3, 3) == "JPY")
                {
                    _exposure[i] = _exposure[i] * 0.01;
                }

                if (_exposure[i] > 0)
                {
                    ChartObjects.DrawText("Mijo_Exposure_" + _used.Substring(i * 3, 3), n + " " + _used.Substring(i * 3, 3) + ": " + _exposure[i].ToString("0"), StaticPosition.TopLeft, Colors.Lime);
                }
                else
                {
                    if (_exposure[i] < 0)
                    {
                        ChartObjects.DrawText("Mijo_Exposure_" + _used.Substring(i * 3, 3), n + " " + _used.Substring(i * 3, 3) + ": " + _exposure[i].ToString("0"), StaticPosition.TopLeft, Colors.BlueViolet);
                    }
                    else
                    {
                        ChartObjects.DrawText("Mijo_Exposure_" + _used.Substring(i * 3, 3), n + " " + _used.Substring(i * 3, 3) + ": " + _exposure[i].ToString("0"), StaticPosition.TopLeft, Colors.Yellow);
                    }
                }
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
