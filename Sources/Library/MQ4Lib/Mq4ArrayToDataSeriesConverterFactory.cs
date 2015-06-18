using System;
using System.Collections.Generic;

using cAlgo.API;

namespace cAlgo.MQ4
{
	public class Mq4ArrayToDataSeriesConverterFactory
	{
		private readonly Dictionary<Mq4Array<Mq4Double>, IndicatorDataSeries> _cachedAdapters = new Dictionary<Mq4Array<Mq4Double>, IndicatorDataSeries>();
		private Func<IndicatorDataSeries> _dataSeriesFactory;

		public Mq4ArrayToDataSeriesConverterFactory(Func<IndicatorDataSeries> dataSeriesFactory)
		{
			_dataSeriesFactory = dataSeriesFactory;
		}

		public DataSeries Create(Mq4Array<Mq4Double> mq4Array)
		{
			IndicatorDataSeries dataSeries;

			if (_cachedAdapters.TryGetValue(mq4Array, out dataSeries))
				return dataSeries;

			dataSeries = _dataSeriesFactory();
			new Mq4ArrayToDataSeriesConverter(mq4Array, dataSeries);
			_cachedAdapters[mq4Array] = dataSeries;

			return dataSeries;
		}
	}

	public class Mq4ArrayToDataSeriesConverter
	{
		private readonly Mq4Array<Mq4Double> _mq4Array;
		private readonly IndicatorDataSeries _dataSeries;

		public Mq4ArrayToDataSeriesConverter(Mq4Array<Mq4Double> mq4Array, IndicatorDataSeries dataSeries)
		{
			_mq4Array = mq4Array;
			_dataSeries = dataSeries;
			_mq4Array.Changed += OnValueChanged;
			CopyAllValues();
		}

		private void CopyAllValues()
		{
			for (var i = 0; i < _mq4Array.Length; i++)
			{
				if (_mq4Array.IsInverted)
					_dataSeries[_mq4Array.Length - i] = _mq4Array[i];
				else
					_dataSeries[i] = _mq4Array[i];
			}
		}

		private void OnValueChanged(int index, Mq4Double value)
		{
			int indexToSet;
			if (_mq4Array.IsInverted)
				indexToSet = _mq4Array.Length - index;
			else
				indexToSet = index;

			if (indexToSet < 0)
				return;

			_dataSeries[indexToSet] = value;
		}
	}



}
