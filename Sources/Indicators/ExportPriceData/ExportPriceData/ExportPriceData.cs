using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using System.IO;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class ExportPriceData : Indicator
    {
        private StreamWriter _fileWriter;
        private System.DateTime _lastOpen;

        protected override void Initialize()
        {
            // Initialize and create nested indicators
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = path + "\\cTrader\\" + Symbol.Code + MarketSeries.TimeFrame + ".csv";
            _fileWriter = File.CreateText(fileName);
            _fileWriter.AutoFlush = true;
        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index
            if (_lastOpen != MarketSeries.OpenTime[index - 1])
            {
                _lastOpen = MarketSeries.OpenTime[index - 1];
                _fileWriter.WriteLine(dateTimeString() + "," + MarketSeries.Open[index - 1] + "," + MarketSeries.High[index - 1] + "," + MarketSeries.Low[index - 1] + "," + MarketSeries.Close[index - 1] + "," + MarketSeries.TickVolume[index - 1]);
            }
        }

        private string dateTimeString()
        {
            return _lastOpen.Year + "." + twoChars(_lastOpen.Month) + "." + twoChars(_lastOpen.Day) + "," + twoChars(_lastOpen.Hour) + ":" + twoChars(_lastOpen.Minute);
        }

        private string twoChars(int it)
        {
            string str = "" + it;

            if (str.Length < 2)
            {
                str = "0" + str;
            }
            return str;
        }
    }
}
