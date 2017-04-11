using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveMinOrDefault<TSource, TParameter, TValue> : ActiveListCompareBase<TSource, TParameter, TValue>
	{
		private readonly Comparer<TValue> _comparer;

		public ActiveMinOrDefault(IActiveList<TSource> source, Func<TSource, TValue> selector, IEnumerable<string> propertiesToWatch)
			: this(source, null, selector, propertiesToWatch, null)
		{
		}

		public ActiveMinOrDefault(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TValue> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, parameter, i => selector.Invoke(i, parameter.Value), sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveMinOrDefault(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TValue> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) 
			: base(source, parameter, selector, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_comparer = Comparer<TValue>.Default;

			Initialize();
		}

		protected override int Compare(TValue leftValue, TValue rightValue) => _comparer.Compare(rightValue, leftValue);
	}
}
