using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveSelect<TSource, TParameter, TResult> : ActiveSelectBase<TSource, TResult, TParameter, TResult>
	{
		public ActiveSelect(IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch)
			: this(source, selector, null, propertiesToWatch, null)
		{
		}

		public ActiveSelect(IActiveList<TSource> source, Func<TSource, TParameter, TResult> selector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, i => selector.Invoke(i, parameter.Value), parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveSelect(IActiveList<TSource> source, Func<TSource, TResult> selector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, selector, i => i, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			Initialize();
		}
	}

	internal class ActiveSelectValue<TSource, TParameter, TResult> : ActiveSelectBase<TSource, IActiveValue<TResult>, TParameter, TResult>
	{
		public ActiveSelectValue(IActiveList<TSource> source, Func<TSource, IActiveValue<TResult>> selector, IEnumerable<string> propertiesToWatch)
			: this(source, selector, null, propertiesToWatch, null)
		{
		}

		public ActiveSelectValue(IActiveList<TSource> source, Func<TSource, TParameter, IActiveValue<TResult>> selector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, i => selector.Invoke(i, parameter.Value), parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveSelectValue(IActiveList<TSource> source, Func<TSource, IActiveValue<TResult>> selector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, selector, i => i.Value, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			Initialize();
		}
	}
}
