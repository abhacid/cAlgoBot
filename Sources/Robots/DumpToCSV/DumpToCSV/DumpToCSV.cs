using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class DumpToCSV : Robot
    {
        protected override void OnStop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var folderPath = Path.Combine(desktopFolder, "trendbars");
            Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, Symbol.Code + " " + TimeFrame + ".csv");
            using (var writer = File.CreateText(filePath))
            {
                for (var i = 0; i < MarketSeries.Close.Count; i++)
                {
                    writer.WriteLine(ConcatWithComma(MarketSeries.OpenTime[i], MarketSeries.Open[i], MarketSeries.High[i], MarketSeries.Low[i], MarketSeries.Close[i], MarketSeries.TickVolume[i]));
                }
            }
        }

        private string ConcatWithComma(params object[] parameters)
        {
            return string.Join(",", parameters.Select(p => p.ToString()));
        }
    }
}
