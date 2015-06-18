using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.MQ4
{
	public class Mq4ChartObjects
	{
		private readonly ChartObjects _algoChartObjects;
		private readonly TimeSeries _timeSeries;

		private readonly Dictionary<string, Mq4Object> _mq4ObjectByName = new Dictionary<string, Mq4Object>();
		private readonly List<string> _mq4ObjectNameByIndex = new List<string>();

		public Mq4ChartObjects(ChartObjects chartObjects, TimeSeries timeSeries)
		{
			_algoChartObjects = chartObjects;
			_timeSeries = timeSeries;
		}

		public void Set(string name, int index, Mq4Double value)
		{
			if (!_mq4ObjectByName.ContainsKey(name))
				return;
			_mq4ObjectByName[name].Set(index, value);
			_mq4ObjectByName[name].Draw();
		}
		public void SetText(string name, string text, int font_size, string font, int color)
		{
			if (!_mq4ObjectByName.ContainsKey(name))
				return;

			Set(name, MQ4Const.OBJPROP_COLOR, color);
		}


		private T GetObject<T>(string name) where T : Mq4Object
		{
			Mq4Object mq4Object;
			if (!_mq4ObjectByName.TryGetValue(name, out mq4Object))
				return null;
			return mq4Object as T;
		}

	}

}
