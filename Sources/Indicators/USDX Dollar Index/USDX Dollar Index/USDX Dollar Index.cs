using System;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Internals;

// (C) 2014 marekfx

//ICE USDX contract - https://www.theice.com/publicdocs/futures_us/USDX_Futures_Contract.pdf
//ICE EURX contract - https://www.theice.com/publicdocs/rulebooks/futures_us/24_ICE_Futures_EURO_Index.pdf

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AutoRescale = true, AccessRights = AccessRights.None)]
    public class USDXDollarIndex : Indicator
    {
        [Parameter("Show USDX", DefaultValue = true)]
        public bool ShowUSDX { get; set; }

        [Parameter("Show EURX", DefaultValue = false)]
        public bool ShowEURX { get; set; }

        [Output("USDX")]
        public IndicatorDataSeries USDX { get; set; }

        [Output("EURX")]
        public IndicatorDataSeries EURX { get; set; }

        private Index _usdxIndex;

        private Index _eurxIndex;

        protected override void Initialize()
        {
            _usdxIndex = new Index 
            {
                Name = "USDX",
                Multiplier = 50.14348112,
                Constituents = new List<Constituent> 
                {
                    //wieght is negative when USD is not the base currency (EURUSD and GBPUSD)

                    new Constituent("EURUSD", -0.576),
                    new Constituent("USDJPY", 0.136),
                    new Constituent("GBPUSD", -0.119),
                    new Constituent("USDCAD", 0.091),
                    new Constituent("USDSEK", 0.042),
                    new Constituent("USDCHF", 0.036)
                }
            };

            _eurxIndex = new Index 
            {
                Name = "EURX",
                Multiplier = 34.38805726,
                Constituents = new List<Constituent> 
                {
                    new Constituent("EURUSD", 0.3155),
                    new Constituent("EURJPY", 0.1891),
                    new Constituent("EURGBP", 0.3056),
                    new Constituent("EURSEK", 0.0785),
                    new Constituent("EURCHF", 0.1113)
                }
            };
        }

        public override void Calculate(int index)
        {
            var date = MarketSeries.OpenTime[index];

            if (ShowUSDX)
            {
                USDX[index] = CalculateIndex(_usdxIndex, date);
            }

            if (ShowEURX)
            {
                EURX[index] = CalculateIndex(_eurxIndex, date);
            }
        }

        private double CalculateIndex(Index index, DateTime date)
        {
            //index is calculated as a weighted geometric mean of its constituents' close prices

            double result = index.Multiplier;

            foreach (var weight in index.Constituents)
            {
                var series = MarketData.GetSeries(weight.Symbol, TimeFrame);
                if (series == null)
                {
                    return double.NaN;
                }
                double close = GetCloseByDate(date, series);
                result *= Math.Pow(close, weight.Weight);
            }
            return result;
        }

        private double GetCloseByDate(DateTime date, MarketSeries series)
        {
            var idx = series.OpenTime.GetIndexByExactTime(date);
            if (idx == -1)
            {
                return double.NaN;
            }
            return series.Close[idx];
        }
    }

    public class Index
    {
        public string Name { get; set; }

        /// <summary>
        /// Constant multiplier as defined in ICE contract spec
        /// </summary>
        public double Multiplier { get; set; }

        /// <summary>
        /// List of index constituents
        /// </summary>
        public List<Constituent> Constituents { get; set; }
    }

    public class Constituent
    {
        public Constituent(string symbol, double cx)
        {
            Symbol = symbol;
            Weight = cx;
        }

        public string Symbol { get; private set; }

        /// <summary>
        /// Constituent Weight
        /// </summary>
        public double Weight { get; private set; }
    }
}
