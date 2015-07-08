// ------------------------------------------------------------                   
// Paste this code into your cAlgo editor. 
// -----------------------------------------------------------
using System;
using System.Collections.Generic;

using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.MQ4;

using System.Linq;

// ---------------------------------------------------------------------------                   
// Converted from MQ4 to cAlgo with http://2calgo.com
// ---------------------------------------------------------------------------

// ZigZag Kwan MBFX Timing ou Beta
// http://fxfree.co/threads/kwan-zigzag-beta-mbfx-timing-indicators-chi-bao-xu-huong-thi-thuong.1230/

namespace cAlgo.Indicators
{

	[Indicator(ScalePrecision = 2, AutoRescale = false, IsOverlay = false)]
	[Levels(70, 30, 50)]
	public class ZigzagKwanMBFXTiming : Indicator
	{

		#region cBot Parameters
		[Parameter("Len", DefaultValue = 4)]
		public int len { get; set; }

		[Parameter("Filter", DefaultValue = -1.0)]
		public double filter { get; set; }

		[Output("Stand", Color = Colors.Yellow, PlotType = PlotType.DiscontinuousLine)]
		public IndicatorDataSeries standIndicatorDataSeries { get; set; }

		[Output("Buy", Color = Colors.Green, PlotType = PlotType.DiscontinuousLine)]
		public IndicatorDataSeries buyIndicatorDataSeries { get; set; }

		[Output("Sell", Color = Colors.Red, PlotType = PlotType.DiscontinuousLine)]
		public IndicatorDataSeries sellIndicatorDataSeries { get; set; }

		[Output("Trade", Color = Colors.SteelBlue, PlotType = PlotType.DiscontinuousLine)]
		public IndicatorDataSeries tradeActionIndicatorDataSeries { get; set; }
		#endregion

		#region globals

		public const int ceilTradeAction = 10;
		public const int tradeActionNeutral = -20;

		public Mq4OutputDataSeries tradeActionMq4Output;
		Mq4OutputDataSeries standMq4Output;
		Mq4OutputDataSeries buyMq4Output;
		Mq4OutputDataSeries sellMq4Output;

		int _indicatorCounted;
		int _currentIndex;

		CachedStandardIndicators _cachedStandardIndicators;
		Mq4ChartObjects _mq4ChartObjects;
		Mq4ArrayToDataSeriesConverterFactory _mq4ArrayToDataSeriesConverterFactory;
		Mq4MarketDataSeries Open;
		Mq4MarketDataSeries High;
		Mq4MarketDataSeries Low;
		Mq4MarketDataSeries Close;
		Mq4MarketDataSeries Median;
		Mq4MarketDataSeries Volume;

		new Mq4TimeSeries  Time;

		static Dictionary<int, string> ArrowByIndex = new Dictionary<int, string>
			{ { 0, MQ4Const.xArrow }, { 1, MQ4Const.xArrow }, { 2, MQ4Const.xArrow }, { 3, MQ4Const.xArrow }, { 4, MQ4Const.xArrow }, { 5, MQ4Const.xArrow }, { 6, MQ4Const.xArrow }, { 7, MQ4Const.xArrow } };

		static List<Mq4OutputDataSeries> AllBuffers = new List<Mq4OutputDataSeries>();

		List<DataSeries> AllOutputDataSeries = new List<DataSeries>();

		#endregion

		protected override void Initialize()
		{
			if (tradeActionIndicatorDataSeries == null)
				tradeActionIndicatorDataSeries = CreateDataSeries();
			tradeActionMq4Output = new Mq4OutputDataSeries(this, tradeActionIndicatorDataSeries, ChartObjects, 0, 0, () => CreateDataSeries(), 2, Colors.Blue);
			AllBuffers.Add(tradeActionMq4Output);

			if (standIndicatorDataSeries == null)
				standIndicatorDataSeries = CreateDataSeries();
			standMq4Output = new Mq4OutputDataSeries(this, standIndicatorDataSeries, ChartObjects, 0, 0, () => CreateDataSeries(), 2, Colors.Yellow);
			AllBuffers.Add(standMq4Output);

			if (buyIndicatorDataSeries == null)
				buyIndicatorDataSeries = CreateDataSeries();
			buyMq4Output = new Mq4OutputDataSeries(this, buyIndicatorDataSeries, ChartObjects, 0, 1, () => CreateDataSeries(), 2, Colors.Green);
			AllBuffers.Add(buyMq4Output);

			if (sellIndicatorDataSeries == null)
				sellIndicatorDataSeries = CreateDataSeries();
			sellMq4Output = new Mq4OutputDataSeries(this, sellIndicatorDataSeries, ChartObjects, 0, 2, () => CreateDataSeries(), 2, Colors.Orange);
			AllBuffers.Add(sellMq4Output);

			AllOutputDataSeries.Add(standIndicatorDataSeries);
			AllOutputDataSeries.Add(buyIndicatorDataSeries);
			AllOutputDataSeries.Add(sellIndicatorDataSeries);

			Open = new Mq4MarketDataSeries(MarketSeries.Open);
			High = new Mq4MarketDataSeries(MarketSeries.High);
			Low = new Mq4MarketDataSeries(MarketSeries.Low);
			Close = new Mq4MarketDataSeries(MarketSeries.Close);
			Volume = new Mq4MarketDataSeries(MarketSeries.TickVolume);
			Median = new Mq4MarketDataSeries(MarketSeries.Median);
			Time = new Mq4TimeSeries(MarketSeries.OpenTime);

			_cachedStandardIndicators = new CachedStandardIndicators(Indicators);
			_mq4ChartObjects = new Mq4ChartObjects(ChartObjects, MarketSeries.OpenTime);
			_mq4ArrayToDataSeriesConverterFactory = new Mq4ArrayToDataSeriesConverterFactory(() => CreateDataSeries());

		}

		Mq4Double calculateZigzagKwanMBFXTiming()
		{
			Mq4Double index = 0;
			Mq4Double debut = 0;
			Mq4Double ld_208 = 0;
			Mq4Double ld_200 = 0;
			Mq4Double ld_192 = 0;
			Mq4Double ld_184 = 0;
			Mq4Double ld_176 = 0;
			Mq4Double ld_168 = 0;
			Mq4Double ld_160 = 0;
			Mq4Double ld_152 = 0;
			Mq4Double ld_144 = 0;
			Mq4Double ld_136 = 0;
			Mq4Double ld_128 = 0;
			Mq4Double ld_120 = 0;
			Mq4Double ld_112 = 0;
			Mq4Double coFactor = 0;
			Mq4Double factor = 0;
			Mq4Double oldAverage = 0;
			Mq4Double average = 0;
			Mq4Double ld_72 = 0;
			Mq4Double ld_64 = 0;
			Mq4Double ld_56 = 0;
			Mq4Double ld_48 = 0;
			Mq4Double ld_40 = 0;
			Mq4Double priceProgression = 0;
			Mq4Double result = 0;
			Mq4Double secondCycle = 0;
			Mq4Double firstCycle = 0;
			Mq4Double lenBase = 0;

			debut = MarketSeries.Close.Count - len - 1;
			for (index = debut; index >= 0; index--)
			{
				if (firstCycle == 0.0)
				{
					firstCycle = 1.0;
					secondCycle = 0.0;

					lenBase = Math.Max(len - 1.0,5);

					average = 100.0 * ((High[index] + Low[index] + Close[index]) / 3.0);
					factor = 3.0 / (len + 2.0);
					coFactor = 1.0 - factor;
				}
				else
				{
					firstCycle = (firstCycle < lenBase) ? firstCycle+1 : lenBase + 1.0 ;

					oldAverage = average;
					average = 100.0 * ((High[index] + Low[index] + Close[index]) / 3.0);
					priceProgression = average - oldAverage;

					ld_112 = coFactor * ld_112 + factor * priceProgression;
					ld_120 = factor * ld_112 + coFactor * ld_120;
					ld_40 = 1.5 * ld_112 - ld_120 / 2.0;

					ld_128 = coFactor * ld_128 + factor * ld_40;
					ld_208 = factor * ld_128 + coFactor * ld_208;
					ld_48 = 1.5 * ld_128 - ld_208 / 2.0;

					ld_136 = coFactor * ld_136 + factor * ld_48;
					ld_152 = factor * ld_136 + coFactor * ld_152;
					ld_56 = 1.5 * ld_136 - ld_152 / 2.0;

					ld_160 = coFactor * ld_160 + factor * Math.Abs((double)priceProgression);
					ld_168 = factor * ld_160 + coFactor * ld_168;
					ld_64 = 1.5 * ld_160 - ld_168 / 2.0;

					ld_176 = coFactor * ld_176 + factor * ld_64;
					ld_184 = factor * ld_176 + coFactor * ld_184;
					ld_144 = 1.5 * ld_176 - ld_184 / 2.0;

					ld_192 = coFactor * ld_192 + factor * ld_144;
					ld_200 = factor * ld_192 + coFactor * ld_200;
					ld_72 = 1.5 * ld_192 - ld_200 / 2.0;

					if (firstCycle <= lenBase && average != oldAverage)
						secondCycle = 1.0;

					if (firstCycle == lenBase  && secondCycle == 0.0)
						firstCycle = 0.0;
				}

				if (firstCycle > lenBase && ld_72 > 1E-10)
				{
					result = 50.0 * (ld_56 / ld_72 + 1.0);

					if (result > 100.0)
						result = 100.0;

					if (result < 0.0)
						result = 0.0;
				}
				else
					result = 50.0;

				standMq4Output[index] = result;
				buyMq4Output[index] = result;
				sellMq4Output[index] = result;
				tradeActionMq4Output[index] = tradeActionNeutral;

				if (standMq4Output[index] > standMq4Output[index + 1] - filter)
				{
					sellMq4Output[index] = MQ4Const.EMPTY_VALUE;
					tradeActionMq4Output[index] = tradeActionNeutral+ ceilTradeAction;
				}
				else if (standMq4Output[index] < standMq4Output[index + 1] + filter)
				{
					buyMq4Output[index] = MQ4Const.EMPTY_VALUE;
					tradeActionMq4Output[index] += tradeActionNeutral - ceilTradeAction;

				}
				else if (standMq4Output[index] == standMq4Output[index + 1] + filter)
				{
					buyMq4Output[index] = MQ4Const.EMPTY_VALUE;
					sellMq4Output[index] = MQ4Const.EMPTY_VALUE;
					tradeActionMq4Output[index] = tradeActionNeutral;
				}
			}
			return 0;
		}

		public override void Calculate(int index)
		{
			try
			{
				_currentIndex = index;
				tradeActionMq4Output.CurrentIndex = index;
				standMq4Output.CurrentIndex= index;
				buyMq4Output.CurrentIndex=index;
				sellMq4Output.CurrentIndex=index;

				if (IsLastBar)
				{
					calculateZigzagKwanMBFXTiming();
					_indicatorCounted = index;
				}
			}
			catch (Exception e)
			{
				if (e.Source != null)
					Console.WriteLine("IOException source: {0}", e.Source);
				throw;
			}
		}


		public class Mq4OutputDataSeries : IMq4Array<Mq4Double>
		{
			public IndicatorDataSeries OutputDataSeries { get; private set; }
			private readonly IndicatorDataSeries _originalValues;

			private readonly ChartObjects _chartObjects;
			private readonly int _style;
			private readonly int _bufferIndex;
			private readonly Indicator _indicator;

			public Mq4OutputDataSeries(ZigzagKwanMBFXTiming zigzagKwanMBFXTiming, IndicatorDataSeries outputDataSeries, ChartObjects chartObjects, int style, int bufferIndex, Func<IndicatorDataSeries> dataSeriesFactory, int lineWidth, Colors? color = null)
			{
				OutputDataSeries = outputDataSeries;
				_chartObjects = chartObjects;
				_style = style;
				_bufferIndex = bufferIndex;
				_indicator = zigzagKwanMBFXTiming;
				Color = color;
				_originalValues = dataSeriesFactory();
				LineWidth = lineWidth;
			}

			public int LineWidth { get; private set; }
			public Colors? Color { get; private set; }

			public int Length
			{
				get { return OutputDataSeries.Count; }
			}

			public int CurrentIndex
			{
				get;
				set;
			}

			public int Shift
			{
				get;
				set;
			}

			public Mq4Double this[int index]
			{
				get
				{
					var indexToGetFrom = CurrentIndex - index + Shift;
					if (indexToGetFrom < 0 || indexToGetFrom > CurrentIndex)
						return 0;
					if (indexToGetFrom >= _originalValues.Count)
						return MQ4Const.EMPTY_VALUE;

					return _originalValues[indexToGetFrom];
				}
				set
				{
					var indexToSet = CurrentIndex - index + Shift;
					if (indexToSet < 0)
						return;

					_originalValues[indexToSet] = value;

					var valueToSet = value;
					if (valueToSet == MQ4Const.EMPTY_VALUE)
						valueToSet = double.NaN;

					if (indexToSet < 0)
						return;

					OutputDataSeries[indexToSet] = valueToSet;

					switch (_style)
					{
						case MQ4Const.DRAW_ARROW:
							var arrowName = GetArrowName(indexToSet);
							if (double.IsNaN(valueToSet))
								_chartObjects.RemoveObject(arrowName);
							else
							{
								var color = Color.HasValue ? Color.Value : Colors.Red;
								_chartObjects.DrawText(arrowName, ArrowByIndex[_bufferIndex], indexToSet, valueToSet, VerticalAlignment.Center, HorizontalAlignment.Center, color);
							}
							break;
						case MQ4Const.DRAW_HISTOGRAM:
							//if (false)
							{
								var anotherLine = AllBuffers.FirstOrDefault(b => b.LineWidth == LineWidth && b != this);
								if (anotherLine != null)
								{
									var name = GetNameOfHistogramLineOnChartWindow(indexToSet);
									Colors color;
									if (this[index] > anotherLine[index])
										color = Color ?? Colors.Green;
									else
										color = anotherLine.Color ?? Colors.Green;
									var lineWidth = LineWidth;
									if (lineWidth != 1 && lineWidth < 5)
										lineWidth = 5;

									_chartObjects.DrawLine(name, indexToSet, this[index], indexToSet, anotherLine[index], color, lineWidth);
								}
							}
							break;
					}
				}
			}

			private string GetNameOfHistogramLineOnChartWindow(int index)
			{
				return string.Format("Histogram on chart window {0} {1}", LineWidth, index);
			}

			private string GetArrowName(int index)
			{
				return string.Format("Arrow {0} {1}", GetHashCode(), index);
			}


		}

	}

}