using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveAverageOrDefault<TSource, TParameter, TValue, TResult> : ActiveAccumulatorBase<TSource, TParameter, TValue, TResult>
	{
		private Func<TValue, int, TResult> _divider;

		public ActiveAverageOrDefault(IActiveList<TSource> source, Func<TSource, TValue> selector, Func<TValue, TValue, TValue> adder, Func<TValue, TValue, TValue> subtractor, Func<TValue, int, TResult> divider, IEnumerable<string> propertiesToWatch)
			: this(source, null, selector, adder, subtractor, divider, propertiesToWatch, null)
		{
		}

		public ActiveAverageOrDefault(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TValue> selector, Func<TValue, TValue, TValue> adder, Func<TValue, TValue, TValue> subtractor, Func<TValue, int, TResult> divider, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, parameter, i => selector.Invoke(i, parameter.Value), adder, subtractor, divider, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveAverageOrDefault(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TValue> selector, Func<TValue, TValue, TValue> adder, Func<TValue, TValue, TValue> subtractor, Func<TValue, int, TResult> divider, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, parameter, selector, adder, subtractor, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_divider = divider;

			Initialize();
		}

		protected override TResult GetValueFromSum(TValue sum) => SourceList.Count > 0 ? _divider.Invoke(sum, SourceList.Count) : default(TResult);
	}
}
