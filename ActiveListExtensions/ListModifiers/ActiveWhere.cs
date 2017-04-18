using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveWhere<TSource, TParameter> : ActiveWhereBase<TSource, TParameter, TSource>
	{
		public ActiveWhere(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> sourcePropertiesToWatch)
			: this(source, predicate, null, sourcePropertiesToWatch, null)
		{
		}

		public ActiveWhere(IActiveList<TSource> source, Func<TSource, TParameter, bool> predicate, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) 
			: this(source, i => predicate.Invoke(i, parameter.Value), parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveWhere(IActiveList<TSource> source, Func<TSource, bool> predicate, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, predicate, i => i, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			Initialize();
		}
	}

	internal class ActiveWhereValue<TSource, TParameter> : ActiveWhereBase<KeyValuePair<bool, TSource>, TParameter, TSource>
	{
		public ActiveWhereValue(IActiveList<TSource> source, Func<TSource, IActiveValue<bool>> predicate, IEnumerable<string> sourcePropertiesToWatch)
			: this(source, predicate, null, sourcePropertiesToWatch, null)
		{
		}

		public ActiveWhereValue(IActiveList<TSource> source, Func<TSource, TParameter, IActiveValue<bool>> predicate, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, i => predicate.Invoke(i, parameter.Value), parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveWhereValue(IActiveList<TSource> source, Func<TSource, IActiveValue<bool>> predicate, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(new ActiveValueListUnwrapper<TSource, TParameter, KeyValuePair<bool, TSource>>(source, parameter, value => predicate.Invoke(value).ActiveMutate(o => new KeyValuePair<bool, TSource>(o, value)), sourcePropertiesToWatch, parameterPropertiesToWatch), kvp => kvp.Key, kvp => kvp.Value, null, null, null)
		{
			Initialize();
		}
	}
}
