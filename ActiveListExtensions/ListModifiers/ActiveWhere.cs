using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.ListModifiers.Bases;

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
}
