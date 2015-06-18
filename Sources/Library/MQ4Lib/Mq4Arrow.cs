
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.MQ4
{
	class Mq4Arrow : Mq4Object
	{
		private readonly TimeSeries _timeSeries;
		private int _index;

		public Mq4Arrow(string name, int type, ChartObjects chartObjects, TimeSeries timeSeries): base(name, type, chartObjects)
		{
			_timeSeries = timeSeries;
		}

		public override void Set(int index, Mq4Double value)
		{
			base.Set(index, value);
			switch (index)
			{
				case MQ4Const.OBJPROP_TIME1:
					_index = _timeSeries.GetIndexByTime(Time1);
					break;
			}
		}

		private int ArrowCode
		{
			get { return Get(MQ4Const.OBJPROP_ARROWCODE); }
		}

		public override void Draw()
		{
			string arrowString;
			HorizontalAlignment horizontalAlignment;
			switch (ArrowCode)
			{
				case MQ4Const.SYMBOL_RIGHTPRICE:
					horizontalAlignment = HorizontalAlignment.Right;
					arrowString = Price1.ToString();
					break;
				case MQ4Const.SYMBOL_LEFTPRICE:
					horizontalAlignment = HorizontalAlignment.Left;
					arrowString = Price1.ToString();
					break;
				default:
					arrowString = MQ4Const.GetArrowByCode(ArrowCode);
					horizontalAlignment = HorizontalAlignment.Center;
					break;
			}
			DrawText(Name, arrowString, _index, Price1, VerticalAlignment.Center, horizontalAlignment, Color);
		}
	}


}
