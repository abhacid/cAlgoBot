//#reference: bin\LumenWorks.Framework.IO.dll
//#reference: C:\Windows\Microsoft.Net\assembly\GAC_64\System.Data\v4.0_4.0.0.0__b77a5c561934e089\System.Data.dll

// (C) 2014 marekfx

//Csv Reader (c) http://www.codeproject.com/Articles/9258/A-Fast-CSV-Reader


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using cAlgo.API;
using LumenWorks.Framework.IO.Csv;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.Internet)]
    public class YahooFinance : Indicator
    {
        [Parameter("Ticker", DefaultValue = "^VIX")]
        public string Ticker { get; set; }

        [Parameter("Lookback In Years", DefaultValue = 15)]
        public int LookbackInYears { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        private Dictionary<DateTime, StockData> _stockData;

        private const string UrlBase = "https://ichart.finance.yahoo.com/table.csv?";

        protected override void Initialize()
        {
            try
            {
                _stockData = DownloadAndParse().ToDictionary(x => x.Date);
            } catch (Exception e)
            {
                Print(e.Message + e.StackTrace);
            }
        }

        public override void Calculate(int index)
        {
            if (_stockData == null)
                return;

            DateTime date = MarketSeries.OpenTime[index].Date;
            StockData value;

            if (_stockData.TryGetValue(date, out value))
            {
                Result[index] = value.Close;
            }
        }

        private List<StockData> DownloadAndParse()
        {
            string csvData = DownloadYahooData();
            var result = ParseCsvData(csvData);

            return result;
        }

        private List<StockData> ParseCsvData(string csvData)
        {
            Print("Parsing");
            var result = new List<StockData>();

            using (var reader = new StringReader(csvData))
            {
                using (var fields = new CsvReader(reader, true))
                {
                    while (fields.ReadNextRecord())
                    {
                        try
                        {
                            int i = 0;
                            var x = new StockData();

                            x.Date = DateTime.ParseExact(fields[i++], "yyyy-MM-dd", null);
                            x.Open = double.Parse(fields[i++]);
                            x.High = double.Parse(fields[i++]);
                            x.Low = double.Parse(fields[i++]);
                            x.Close = double.Parse(fields[i++]);
                            x.Volume = double.Parse(fields[i++]);
                            x.AdjClose = double.Parse(fields[i++]);

                            result.Add(x);
                        } catch (Exception e)
                        {
                            Print(e.Message + e.StackTrace);
                        }
                    }
                }
            }

            Print("Parsing completed. {0} items downloaded", result.Count);
            return result;
        }

        private string DownloadYahooData()
        {
            string csvData;

            //sample url
            //https://ichart.finance.yahoo.com/table.csv?s=%5EVIX&a=00&b=2&c=1990&d=03&e=21&f=2014&g=d&ignore=.csv

            var dateFrom = DateTime.Now.AddYears(-LookbackInYears);
            var dateTo = DateTime.Now;

            //prepare parameters
            Dictionary<string, object> urlParams = new Dictionary<string, object> 
            {
                {
                    "s",
                    Ticker
                },
                {
                    "a",
                    "00"
                },
                {
                    "b",
                    dateFrom.Month
                },
                {
                    "c",
                    dateFrom.Year
                },
                {
                    "d",
                    dateTo.Day
                },
                {
                    "e",
                    dateTo.Month
                },
                {
                    "f",
                    dateTo.Year
                },
                {
                    "g",
                    "d"
                },
                {
                    "ignore",
                    ".csv"
                }
            };

            //build url
            string[] paramArray = urlParams.Select(x => string.Format("{0}={1}", x.Key, x.Value)).ToArray();
            string url = UrlBase + string.Join("&", paramArray);

            //download CSV
            using (var wc = new WebClient())
            {
                Print("Downloading {0}", url);
                csvData = wc.DownloadString(url);
                Print("Download completed");
            }
            return csvData;
        }
    }

    public class StockData
    {
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public double AdjClose { get; set; }
    }
}


