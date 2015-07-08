// This is the VWAP Version 1.1
// Come back to
// http://ctdn.com/algos/indicators/show/792
// to get the latest version

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class VWAPforeveryBar : Indicator
    {
        [Parameter("Volume Down Color", DefaultValue = "Yellow")]
        public string VolumeDownColor { get; set; }

        [Parameter("Volume Up Color", DefaultValue = "Blue")]
        public string VolumeUpColor { get; set; }

        [Parameter("Coloring based on real volume?", DefaultValue = "false")]
        public bool UseRealVolume { get; set; }

        [Parameter("Marker type (1-6)", DefaultValue = 5, MinValue = 1, MaxValue = 6)]
        public int MarkerType { get; set; }

        [Parameter("Lookback", DefaultValue = 150)]
        public int Lookback { get; set; }

        private string[] Markers = new string[7] 
        {
            "",
            "▬",
            "─",
            "──",
            "────",
            "⚊",
            "➖"
        };
        public double[,] RealtimeVwap = new double[700, 2];
        public MarketSeries M1;
        public int BarCount;

        protected override void Initialize()
        {
            // Calculate all VWAPS once
            if (MarketSeries.TimeFrame < TimeFrame.Daily)
                M1 = MarketData.GetSeries(TimeFrame.Minute);
            else
                M1 = MarketData.GetSeries(TimeFrame.Minute10);

            for (int idx = (MarketSeries.Close.Count - 2) - Lookback; idx <= MarketSeries.Close.Count - 2; idx++)
            {
                double M1Vwap = 0;
                int M1FirstIndex = M1.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idx]);
                if (M1FirstIndex != -1)
                {
                    for (int i = M1FirstIndex; i < M1.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idx + 1]); i++)
                    {
                        M1Vwap += M1.Median[i] * M1.TickVolume[i];
                    }
                }
                else
                    continue;
                M1Vwap = M1Vwap / MarketSeries.TickVolume[idx];
                if (M1Vwap <= MarketSeries.High[idx] && M1Vwap >= MarketSeries.Low[idx])
                    ChartObjects.DrawText("vwap" + idx, Markers[MarkerType], idx, M1Vwap, VerticalAlignment.Center, HorizontalAlignment.Center, MarketSeries.TickVolume[idx] >= MarketSeries.TickVolume[idx - 1] ? (Colors)Enum.Parse(typeof(Colors), VolumeUpColor, true) : (Colors)Enum.Parse(typeof(Colors), VolumeDownColor, true));
            }
            // Prepare vwap of the current bar with minute data for incoming realtime tick data
            for (int i = M1.OpenTime.GetIndexByTime(MarketSeries.OpenTime.LastValue); i <= M1.Close.Count - 1; i++)
            {
                RealtimeVwap[0, 0] += M1.Median[i] * M1.TickVolume[i];
            }
            RealtimeVwap[0, 1] = MarketSeries.TickVolume.LastValue;
            BarCount = MarketSeries.Close.Count - 1;
        }

        public override void Calculate(int index)
        {
            // Realtime data collection, calculation and display of current VWAP
            if (IsLastBar)
            {
                int RealtimeIndex = index - BarCount;
                RealtimeVwap[RealtimeIndex, 1]++;
                RealtimeVwap[RealtimeIndex, 0] += MarketSeries.Close[index];
                bool TickVolumeHigher;
                if (UseRealVolume == true)
                {
                    if (RealtimeIndex > 0)
                        if (RealtimeVwap[RealtimeIndex, 1] > RealtimeVwap[RealtimeIndex - 1, 1])
                            TickVolumeHigher = true;
                        else
                            TickVolumeHigher = false;
                    else if (RealtimeVwap[RealtimeIndex, 1] > MarketSeries.TickVolume[index - 1])
                        TickVolumeHigher = true;
                    else
                        TickVolumeHigher = false;
                }
                else if (MarketSeries.TickVolume[index] > MarketSeries.TickVolume[index - 1])
                    TickVolumeHigher = true;
                else
                    TickVolumeHigher = false;

                ChartObjects.DrawText("vwap" + index, Markers[MarkerType], index, RealtimeVwap[RealtimeIndex, 0] / RealtimeVwap[RealtimeIndex, 1], VerticalAlignment.Center, HorizontalAlignment.Center, TickVolumeHigher == true ? (Colors)Enum.Parse(typeof(Colors), VolumeUpColor, true) : (Colors)Enum.Parse(typeof(Colors), VolumeDownColor, true));
            }
        }
    }
}
