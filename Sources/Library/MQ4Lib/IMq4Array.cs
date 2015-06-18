

namespace cAlgo.MQ4
{
	public interface IMq4Array<T>
	{
		T this[int index] { get; set; }
		int Length { get; }
	}


}
