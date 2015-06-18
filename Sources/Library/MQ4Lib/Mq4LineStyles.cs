
using cAlgo.API;

namespace cAlgo.MQ4
{
	static class Mq4LineStyles
	{
		public static LineStyle ToLineStyle(int style)
		{
			switch (style)
			{
				case 1:
					return LineStyle.Lines;
				case 2:
					return LineStyle.Dots;
				case 3:
				case 4:
					return LineStyle.LinesDots;
				default:
					return LineStyle.Solid;
			}
		}
	}


}
