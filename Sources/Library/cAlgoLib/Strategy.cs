using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Strategies
{
	public abstract class Strategy : IStrategy
	{

		protected Strategy(Robot robot)
		{
			Robot = robot;
		}
		public Robot Robot
		{
			get;
			private set;
		}

		public abstract TradeType? signal();
		protected virtual void Initialize() {}


	}
}
