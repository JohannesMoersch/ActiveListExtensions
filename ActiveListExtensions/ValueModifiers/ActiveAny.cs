using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveAny<TSource, TParameter> : ActiveListPredicateBase<TSource, TParameter>
	{
		public ActiveAny(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			: this(source, null, predicate, propertiesToWatch, null)
		{
		}

		public ActiveAny(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, parameter, i => predicate.Invoke(i, parameter.Value), sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveAny(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) 
			: base(source, parameter, predicate, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			Initialize();
		}

		protected override bool GetValue(bool predicateMet) => predicateMet;
	}
}