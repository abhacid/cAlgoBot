using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxProQuant.Lib
{
	/// <summary>
	/// Code qui émule bool? avec la possibilité de faire des opérations de comparaisons et boolééenne
	/// </summary>
    public class TriState
    {

		public static readonly TriState TriStateNull = new TriState(0);
		public static readonly TriState TriStateFalse = new TriState(-1);
		public static readonly TriState TriStateTrue = new TriState(1);
		sbyte value;

		TriState(int value)
		{
			this.value = (sbyte)value;
		}

		public bool IsTriStateNull
		{
			get
			{
				return value == 0;
			}
		}

		public static implicit operator TriState(bool x)
		{
			return x ? TriStateTrue : TriStateFalse;
		}

		public static implicit operator TriState(bool? x)
		{
			if (x.HasValue)
				return x.Value ? TriStateTrue : TriStateFalse;
			else
				return null;
		}

		public static TriState operator ==(TriState x, TriState y)
		{
			if(x.value == 0 || y.value == 0)
				return TriStateNull;

			return x.value == y.value ? TriStateTrue : TriStateFalse;
		}

		public static TriState operator !=(TriState x, TriState y)
		{
			if(x.value == 0 || y.value == 0)
				return TriStateNull;

			return x.value != y.value ? TriStateTrue : TriStateFalse;
		}

		public static TriState operator !(TriState x)
		{
			return new TriState(-x.value);
		}

		public static TriState operator &(TriState x, TriState y)
		{
			return new TriState(x.value < y.value ? x.value : y.value);
		}

		public static TriState operator |(TriState x, TriState y)
		{
			return new TriState(x.value > y.value ? x.value : y.value);
		}

		public static bool operator true(TriState x)
		{
			return x.value > 0;
		}

		public static bool operator false(TriState x)
		{
			return x.value < 0;
		}

		public static implicit operator bool(TriState x)
		{
			return x.value > 0;
		}

		public override bool Equals(object obj)
		{
			if(!(obj is TriState))
				return false;

			return value == ((TriState)obj).value;
		}

		public override int GetHashCode()
		{
			return value;
		}

		public override string ToString()
		{
			if(value > 0)
				return "TriStateTrue";

			if(value < 0)
				return "TriStateFalse";

			return "TriStateNull";
		}
	}
}

