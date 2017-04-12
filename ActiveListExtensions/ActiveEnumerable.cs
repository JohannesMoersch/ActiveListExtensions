using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public static class ActiveEnumerable
	{
		public static IActiveList<int> ActiveRange(int start, IActiveValue<int> count) => ActiveRange(new ActiveValueWrapper<int>(start), count);

		public static IActiveList<int> ActiveRange(IActiveValue<int> start, int count) => ActiveRange(start, new ActiveValueWrapper<int>(count));

		public static IActiveList<int> ActiveRange(IActiveValue<int> start, IActiveValue<int> count) => new ActiveRange(start, count);

		public static IActiveList<TValue> ActiveRepeat<TValue>(TValue value, IActiveValue<int> count) => ActiveRepeat(new ActiveValueWrapper<TValue>(value) as IActiveValue<TValue>, count);

		public static IActiveList<TValue> ActiveRepeat<TValue>(IActiveValue<TValue> value, int count) => ActiveRepeat(value, new ActiveValueWrapper<int>(count));

		public static IActiveList<TValue> ActiveRepeat<TValue>(IActiveValue<TValue> value, IActiveValue<int> count) => new ActiveRepeat<TValue>(value, count);
	}
}
