using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.MQ4
{
	abstract class Mq4Object : IDisposable
	{
		private readonly ChartObjects _chartObjects;

		protected Mq4Object(string name, int type, ChartObjects chartObjects)
		{
			Name = name;
			Type = type;
			_chartObjects = chartObjects;
		}

		public int Type { get; private set; }

		public string Name { get; private set; }

		protected DateTime Time1
		{
			get
			{
				int seconds = Get(MQ4Const.OBJPROP_TIME1);
				return Mq4TimeSeries.ToDateTime(seconds);
			}
		}

		protected double Price1
		{
			get { return Get(MQ4Const.OBJPROP_PRICE1); }
		}

		protected DateTime Time2
		{
			get
			{
				int seconds = Get(MQ4Const.OBJPROP_TIME2);
				return Mq4TimeSeries.ToDateTime(seconds);
			}
		}

		protected double Price2
		{
			get { return Get(MQ4Const.OBJPROP_PRICE2); }
		}

		protected Colors Color
		{
			get
			{
				int intColor = Get(MQ4Const.OBJPROP_COLOR);
				if (intColor != MQ4Const.CLR_NONE)
					return Mq4Colors.GetColorByInteger(intColor);

				return Colors.Yellow;
			}
		}

		protected int Width
		{
			get { return Get(MQ4Const.OBJPROP_WIDTH); }
		}

		protected int Style
		{
			get { return Get(MQ4Const.OBJPROP_STYLE); }
		}

		public abstract void Draw();

		private readonly Dictionary<int, Mq4Double> _properties = new Dictionary<int, Mq4Double>
        {
            {MQ4Const.OBJPROP_WIDTH,new Mq4Double(1)},
            {MQ4Const.OBJPROP_COLOR,new Mq4Double(MQ4Const.CLR_NONE)},
            {
                MQ4Const.OBJPROP_RAY,
                new Mq4Double(1)
            },

            {
                MQ4Const.OBJPROP_LEVELCOLOR,
                new Mq4Double(MQ4Const.CLR_NONE)
            },
            {
                MQ4Const.OBJPROP_LEVELSTYLE,
                new Mq4Double(0)
            },
            {
                MQ4Const.OBJPROP_LEVELWIDTH,
                new Mq4Double(1)
            },
            {
                MQ4Const.OBJPROP_FIBOLEVELS,
                new Mq4Double(9)
            },
            {
                MQ4Const.OBJPROP_FIRSTLEVEL + 0,
                new Mq4Double(0)
            },
            {
                MQ4Const.OBJPROP_FIRSTLEVEL + 1,
                new Mq4Double(0.236)
            },
            {
                MQ4Const.OBJPROP_FIRSTLEVEL + 2,
                new Mq4Double(0.382)
            },
            {
                MQ4Const.OBJPROP_FIRSTLEVEL + 3,
                new Mq4Double(0.5)
            },
            {
                MQ4Const.OBJPROP_FIRSTLEVEL + 4,
                new Mq4Double(0.618)
            },
            {
                MQ4Const.OBJPROP_FIRSTLEVEL + 5,
                new Mq4Double(1)
            },
            {
                MQ4Const.OBJPROP_FIRSTLEVEL + 6,
                new Mq4Double(1.618)
            },
            {
                MQ4Const.OBJPROP_FIRSTLEVEL + 7,
                new Mq4Double(2.618)
            },
            {
                MQ4Const.OBJPROP_FIRSTLEVEL + 8,
                new Mq4Double(4.236)
            }
        };

		public virtual void Set(int index, Mq4Double value)
		{
			_properties[index] = value;
		}

		public Mq4Double Get(int index)
		{
			return _properties.ContainsKey(index) ? _properties[index] : new Mq4Double(0);
		}

		private readonly List<string> _addedAlgoChartObjects = new List<string>();

		protected void DrawText(string objectName, string text, int index, double yValue, VerticalAlignment verticalAlignment = VerticalAlignment.Center, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center, Colors? color = null)
		{
			_addedAlgoChartObjects.Add(objectName);
			_chartObjects.DrawText(objectName, text, index, yValue, verticalAlignment, horizontalAlignment, color);
		}

		protected void DrawText(string objectName, string text, StaticPosition position, Colors? color = null)
		{
			_addedAlgoChartObjects.Add(objectName);
			_chartObjects.DrawText(objectName, text, position, color);
		}

		protected void DrawLine(string objectName, int index1, double y1, int index2, double y2, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
		{
			_addedAlgoChartObjects.Add(objectName);
			_chartObjects.DrawLine(objectName, index1, y1, index2, y2, color, thickness, style);
		}

		protected void DrawLine(string objectName, DateTime date1, double y1, DateTime date2, double y2, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
		{
			_addedAlgoChartObjects.Add(objectName);
			_chartObjects.DrawLine(objectName, date1, y1, date2, y2, color, thickness, style);
		}

		protected void DrawVerticalLine(string objectName, DateTime date, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
		{
			_addedAlgoChartObjects.Add(objectName);
			_chartObjects.DrawVerticalLine(objectName, date, color, thickness, style);
		}

		protected void DrawVerticalLine(string objectName, int index, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
		{
			_addedAlgoChartObjects.Add(objectName);
			_chartObjects.DrawVerticalLine(objectName, index, color, thickness, style);
		}

		protected void DrawHorizontalLine(string objectName, double y, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
		{
			_addedAlgoChartObjects.Add(objectName);
			_chartObjects.DrawHorizontalLine(objectName, y, color, thickness, style);
		}

		public void Dispose()
		{
			foreach (var name in _addedAlgoChartObjects)
			{
				_chartObjects.RemoveObject(name);
			}
		}
	}


}
