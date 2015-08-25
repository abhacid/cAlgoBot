// --------------------------------------
// MARGIN REQUIRED
//---------------------------------------

// Copyright:   Copyright 2015, Forex Vitals
// Link:        http://www.ForexVitals.com
// Date:        08/07/2015
// Version:     1.0

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MarginRequired : Indicator
    {
        [Parameter("Lots", DefaultValue = 0.01)]
        public double lots { get; set; }

        [Parameter("Text Color", DefaultValue = "White")]
        public string text_color { get; set; }

        protected override void Initialize()
        {

        }

        public override void Calculate(int index)
        {
            int error_code = 0;
            Colors font_color;

            // are the color input strings valid?    
            try
            {
                font_color = (Colors)Enum.Parse(typeof(Colors), text_color, true);
            } catch
            {
                error_code = 1;
                font_color = Colors.Red;
            }

            string home_currency = Account.Currency;
            string symbol = Symbol.Code;
            string base_currency = symbol.Substring(0, 3);
            string quote_currency = symbol.Substring(3, 3);
            double home_rate;
            int margin_ratio;
            double units;
            double margin_required;

            if (home_currency == base_currency || home_currency == quote_currency)
            {
                home_rate = MarketData.GetSymbol(symbol).Bid;
            }
            else
            {
                home_rate = GetHomeRate(home_currency, base_currency);
            }

            if (error_code == 0)
            {
                margin_ratio = Account.Leverage;
                units = (double)Symbol.LotSize * lots;

                margin_required = RoundUp(((home_rate) * units) / margin_ratio, 2);

                ChartObjects.DrawText("marginRequired", "Margin required for " + lots + " lots is " + margin_required, StaticPosition.TopCenter, font_color);
            }
            else
            {
                if (error_code == 1)
                {
                    ChartObjects.DrawText("marginRequired", "Error Code 1: Specified Text Color is not a valid option", StaticPosition.TopCenter, font_color);
                }
            }
        }

        public static double RoundUp(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }

        private double GetHomeRate(string fromCurrency, string toCurrency)
        {
            Symbol symbol = TryGetSymbol(fromCurrency + toCurrency);

            if (symbol != null)
            {
                return symbol.Bid;
            }

            symbol = TryGetSymbol(toCurrency + fromCurrency);
            return symbol.Bid;
        }

        private Symbol TryGetSymbol(string symbolCode)
        {
            try
            {
                Symbol symbol = MarketData.GetSymbol(symbolCode);
                if (symbol.Bid == 0.0)
                    return null;
                return symbol;
            } catch
            {
                return null;
            }
        }
    }
}
