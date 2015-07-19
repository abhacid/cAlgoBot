// ZigZag indicator, allowing the usage of multiple TimeFrames within one indicator
// In this sample three different TimeFrames, but it's expandable
// Based on kkostaki's ZigZag indicator, http://ctdn.com/algos/indicators/show/157
// Thanks kkostaki!
// Author: cmdpirx, creation date: 15.07.2015


using System;
using cAlgo.API;
using cAlgo.API.Internals;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MultiTimeFrameZigZag : Indicator
    {
        [Parameter(DefaultValue = 12)]
        public int Depth { get; set; }

        [Parameter(DefaultValue = 5)]
        public int Deviation { get; set; }

        [Parameter(DefaultValue = 3)]
        public int BackStep { get; set; }

        [Parameter(DefaultValue = false)]
        public bool ShowHistory { get; set; }

        [Parameter("Timeframe1", DefaultValue = "Hour")]
        public TimeFrame Timeframe1 { get; set; }

        [Parameter("Timeframe2", DefaultValue = "Hour4")]
        public TimeFrame Timeframe2 { get; set; }

        [Output("Result", Color = Colors.Blue)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Result1", Color = Colors.OrangeRed)]
        public IndicatorDataSeries Result1 { get; set; }

        [Output("Result2", Color = Colors.Yellow)]
        public IndicatorDataSeries Result2 { get; set; }

        [Output("Hist", Color = Colors.Blue, LineStyle = LineStyle.Dots)]
        public IndicatorDataSeries Hist { get; set; }

        [Output("Hist1", Color = Colors.OrangeRed, LineStyle = LineStyle.Dots)]
        public IndicatorDataSeries Hist1 { get; set; }

        [Output("Hist2", Color = Colors.Yellow, LineStyle = LineStyle.Dots)]
        public IndicatorDataSeries Hist2 { get; set; }

        #region Private fields

        private ZigZag zigzag;
        private ZigZag zigzag1;
        private ZigZag zigzag2;

        #endregion

        protected override void Initialize()
        {
            zigzag = new ZigZag(this, MarketSeries.TimeFrame, Depth, Deviation, BackStep, ShowHistory);
            zigzag1 = new ZigZag(this, Timeframe1, Depth, Deviation, BackStep, ShowHistory);
            zigzag2 = new ZigZag(this, Timeframe2, Depth, Deviation, BackStep, ShowHistory);
        }

        public override void Calculate(int index)
        {
            zigzag.Calculate(index, Result, Hist);
            zigzag1.Calculate(index, Result1, Hist1);
            zigzag2.Calculate(index, Result2, Hist2);
        }
    }

    public class ZigZag
    {

        private double m_lastLow;
        private double m_lastHigh;
        private double m_low;
        private double m_high;
        private int m_lastHighIndex;
        private int m_lastLowIndex;
        private int m_type;
        private double m_point;
        private int m_depth;
        private int m_deviation;
        private int m_backStep;
        private bool m_showHistory;

        private IndicatorDataSeries m_highZigZags;
        private IndicatorDataSeries m_lowZigZags;

        private MarketSeries m_ds;
        private MultiTimeFrameZigZag m_indicator;


        public ZigZag(MultiTimeFrameZigZag indicator, TimeFrame tm, int depth, int dev, int back, bool showHist)
        {
            m_indicator = indicator;
            m_depth = depth;
            m_deviation = dev;
            m_backStep = back;
            m_showHistory = showHist;

            m_type = 0;
            m_low = m_high = 0;

            Initialize(tm);
        }

        public void Initialize(TimeFrame tm)
        {
            m_ds = m_indicator.MarketData.GetSeries(tm);

            m_highZigZags = m_indicator.CreateDataSeries();
            m_lowZigZags = m_indicator.CreateDataSeries();
            m_point = m_indicator.Symbol.TickSize;
        }

        private double GetMax(MarketSeries series, int start, int lenght)
        {
            double m = 0;
            int len = lenght;
            while (len > 0)
            {
                if (series.High[start - len + 1] > m)
                    m = series.High[start - len + 1];
                len--;
            }
            return m;
        }

        private double GetMin(MarketSeries series, int start, int lenght)
        {
            int len = lenght;
            double m = series.Low[start - len + 1];
            while (len > 0)
            {
                if (series.Low[start - len + 1] < m)
                    m = series.Low[start - len + 1];
                len--;
            }
            return m;
        }

        private void StatusDisplay(string s)
        {
            if (m_indicator.IsRealTime)
            {
                m_indicator.ChartObjects.RemoveObject("State1");
                m_indicator.ChartObjects.DrawText("State1", s, StaticPosition.BottomRight, Colors.Yellow);
            }
        }

        public void Calculate(int ix, IndicatorDataSeries Result, IndicatorDataSeries Hist)
        {
            //m_indicator.ChartObjects.DrawText("index1" + ix.ToString(), ix.ToString(), ix, m_indicator.MarketSeries.High[ix], VerticalAlignment.Bottom, HorizontalAlignment.Center, Colors.Yellow);
            var index = m_ds.OpenTime.GetIndexByTime(m_indicator.MarketSeries.OpenTime[ix]);

            if (index == -1)
            {
                m_indicator.Print("-1");
                return;
            }

            if (index < m_depth)
            {
                return;
            }

            var _currentLow = GetMin(m_ds, index, m_depth);
            var _currentHigh = GetMax(m_ds, index, m_depth);

            //m_indicator.Print("A-{0:d4}/{1:d4}\t{6:d1}\t{2:f5}/{3:f5}\t{4:f5}/{5:f5}\t{7}/{8}\t{9}/{10}\t{11:f5}/{12:f5}", index, ix, m_lastLow, _currentLow, m_lastHigh, _currentHigh, m_type, m_low, m_high,
            //m_lastLowIndex, m_lastHighIndex, m_lowZigZags[index], m_highZigZags[index]);

            if (Math.Abs(_currentLow - m_lastLow) < double.Epsilon)
            {
                _currentLow = 0.0;
            }
            else
            {
                m_lastLow = _currentLow;

                if ((m_ds.Low[index] - _currentLow) > (m_deviation * m_point))
                    _currentLow = 0.0;
                else
                {
                    for (int i = 1; i <= m_backStep; i++)
                    {
                        if (Math.Abs(m_lowZigZags[index - i]) > double.Epsilon && m_lowZigZags[index - i] > _currentLow)
                            m_lowZigZags[index - i] = 0.0;
                    }
                }

            }

            if (Math.Abs(m_ds.Low[index] - _currentLow) < double.Epsilon)
            {
                m_lowZigZags[index] = m_ds.Low[index];
            }
            else
                m_lowZigZags[index] = 0.0;


            if (Math.Abs(_currentHigh - m_lastHigh) < double.Epsilon)
                _currentHigh = 0.0;
            else
            {
                m_lastHigh = _currentHigh;

                if ((_currentHigh - m_ds.High[index]) > (m_deviation * m_point))
                    _currentHigh = 0.0;
                else
                {
                    for (int i = 1; i <= m_backStep; i++)
                    {
                        if (Math.Abs(m_highZigZags[index - i]) > double.Epsilon && m_highZigZags[index - i] < _currentHigh)
                            m_highZigZags[index - i] = 0.0;
                    }
                }
            }

            if (Math.Abs(m_ds.High[index] - _currentHigh) < double.Epsilon)
            {
                m_highZigZags[index] = m_ds.High[index];
            }
            else
                m_highZigZags[index] = 0.0;

            //m_indicator.Print("B-{0}/{1}\t{6}\tL:{2}/{3}\tH:{4}/{5}\t{7}/{8}\t{9}/{10}\t{11}/{12}\t{13}/{14}", ix, index, m_lastLow, _currentLow, m_lastHigh, _currentHigh, m_type, m_low, m_high,
            //m_lastLowIndex, m_lastHighIndex, m_lowZigZags[index], m_highZigZags[index], m_indicator.MarketSeries.Low[ix], m_indicator.MarketSeries.High[ix]);

            switch (m_type)
            {
                case 0:
                    //StatusDisplay(string.Format("▼{0:F5}/{1:F5}\n▲{2:F5}/{3:F5}\n{4:F5}\n{5}", _currentHigh, Math.Abs(m_ds.Close[index] - _currentHigh), _currentLow, Math.Abs(m_ds.Close[index] - _currentLow), (m_deviation * m_point), m_type));
                    if (Math.Abs(m_low - 0) < double.Epsilon && Math.Abs(m_high - 0) < double.Epsilon)
                    {
                        if (Math.Abs(m_highZigZags[index]) > double.Epsilon)
                        {
                            m_high = m_ds.High[index];
                            m_lastHighIndex = ix;
                            m_type = -1;
                            Result[ix] = m_high;
                            //m_indicator.ChartObjects.DrawText("index1" + ix.ToString(), ix.ToString(), ix, m_high, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Red);
                        }
                        if (Math.Abs(m_lowZigZags[index]) > double.Epsilon)
                        {
                            m_low = m_ds.Low[index];
                            m_lastLowIndex = ix;
                            m_type = 1;
                            Result[ix] = m_low;
                            //m_indicator.ChartObjects.DrawText("index1" + ix.ToString(), ix.ToString(), ix, m_low, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Red);
                        }
                        Hist[ix] = m_showHistory == true ? Result[ix] : double.NaN;
                    }
                    break;
                case 1:
                    //StatusDisplay(string.Format("▼{0:F5}/{1:F5}\n{2:F5}\n{3}", _currentHigh, Math.Abs(m_ds.Close[index] - _currentHigh), (m_deviation * m_point), m_type));
                    if (Math.Abs(Result[m_lastLowIndex] - m_indicator.MarketSeries.Low[ix]) < double.Epsilon && m_ds.TimeFrame != m_indicator.MarketSeries.TimeFrame)
                    {
                        Result[m_lastLowIndex] = double.NaN;
                        Hist[m_lastLowIndex] = m_showHistory == true ? Hist[m_lastLowIndex] : double.NaN;
                        m_lastLowIndex = ix;
                        Result[ix] = m_low;
                        Hist[ix] = m_showHistory == true ? Result[ix] : double.NaN;
                    }

                    if (Math.Abs(m_lowZigZags[index]) > double.Epsilon && m_lowZigZags[index] < m_low && Math.Abs(m_highZigZags[index] - 0.0) < double.Epsilon)
                    {
                        Result[m_lastLowIndex] = double.NaN;
                        m_lastLowIndex = ix;
                        m_low = m_lowZigZags[index];
                        Result[ix] = m_low;
                        Hist[ix] = m_showHistory == true ? Result[ix] : double.NaN;
                        //m_indicator.ChartObjects.DrawText("index1" + ix.ToString(), ix.ToString(), ix, m_low, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Blue);
                    }
                    if (Math.Abs(m_highZigZags[index] - 0.0) > double.Epsilon && Math.Abs(m_lowZigZags[index] - 0.0) < double.Epsilon)
                    {
                        m_high = m_highZigZags[index];
                        m_lastHighIndex = ix;
                        Result[ix] = m_high;
                        m_type = -1;
                        Hist[ix] = m_showHistory == true ? Result[ix] : double.NaN;
                        //m_indicator.ChartObjects.DrawText("index1" + ix.ToString(), ix.ToString(), ix, m_high, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Red);
                    }
                    break;
                case -1:
                    //StatusDisplay(string.Format("▲{0:F5}/{1:F5}\n{2:F5}\n{3}", _currentLow, Math.Abs(m_ds.Close[index] - _currentLow), (m_deviation * m_point), m_type));
                    if (Math.Abs(Result[m_lastHighIndex] - m_indicator.MarketSeries.High[ix]) < double.Epsilon && m_ds.TimeFrame != m_indicator.MarketSeries.TimeFrame)
                    {
                        Result[m_lastHighIndex] = double.NaN;
                        Hist[m_lastHighIndex] = m_showHistory == true ? Hist[m_lastHighIndex] : double.NaN;
                        m_lastHighIndex = ix;
                        Result[ix] = m_high;
                        Hist[ix] = m_showHistory == true ? Result[ix] : double.NaN;
                    }
                    if (Math.Abs(m_highZigZags[index]) > double.Epsilon && m_highZigZags[index] > m_high && Math.Abs(m_lowZigZags[index] - 0.0) < double.Epsilon)
                    {
                        Result[m_lastHighIndex] = double.NaN;
                        m_lastHighIndex = ix;
                        m_high = m_highZigZags[index];
                        Result[ix] = m_high;
                        Hist[ix] = m_showHistory == true ? Result[ix] : double.NaN;
                        //m_indicator.ChartObjects.DrawText("index1" + ix.ToString(), ix.ToString(), ix, m_high, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Red);
                    }
                    if (Math.Abs(m_lowZigZags[index]) > double.Epsilon && Math.Abs(m_highZigZags[index]) < double.Epsilon)
                    {
                        m_low = m_lowZigZags[index];
                        m_lastLowIndex = ix;
                        Result[ix] = m_low;
                        m_type = 1;
                        Hist[ix] = m_showHistory == true ? Result[ix] : double.NaN;
                        //m_indicator.ChartObjects.DrawText("index1" + ix.ToString(), ix.ToString(), ix, m_low, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Red);
                    }
                    break;
                default:
                    return;
            }
        }
    }
}
