using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveSum<TSource, TParameter, TValue> : ActiveAccumulatorBase<TSource, TParameter, TValue, TValue>
	{
		public ActiveSum(IActiveList<TSource> source, Func<TSource, TValue> selector, Func<TValue, TValue, TValue> adder, Func<TValue, TValue, TValue> subtractor, IEnumerable<string> propertiesToWatch)
			: this(source, null, selector, adder, subtractor, propertiesToWatch, null)
		{
		}

		public ActiveSum(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TValue> selector, Func<TValue, TValue, TValue> adder, Func<TValue, TValue, TValue> subtractor, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, parameter, i => selector.Invoke(i, parameter.Value), adder, subtractor, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveSum(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TValue> selector, Func<TValue, TValue, TValue> adder, Func<TValue, TValue, TValue> subtractor, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, parameter, selector, adder, subtractor, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			Initialize();
		}

		protected override TValue GetValueFromSum(TValue sum) => sum;
	}
}