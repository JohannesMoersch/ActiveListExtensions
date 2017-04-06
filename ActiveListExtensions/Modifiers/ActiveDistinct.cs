using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveDistinct<TKey, TSource, TParameter> : ActiveSetBase<TKey, TSource, TParameter>
	{
		public ActiveDistinct(IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) 
			: this(source, keySelector, null, propertiesToWatch, null)
		{
		}

		public ActiveDistinct(IActiveList<TSource> source, Func<TSource, TParameter, TKey> keySelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, i => keySelector.Invoke(i, parameter.Value), parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveDistinct(IActiveList<TSource> source, Func<TSource, TKey> keySelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, null, keySelector, null, sourcePropertiesToWatch, null)
		{
		}

		protected override SetAction OnAddedToLeft(bool existsInRight) => SetAction.Add;

		protected override SetAction OnAddedToRight(bool existsInLeft) { throw new NotSupportedException(); }

		protected override SetAction OnRemovedFromLeft(bool existsInRight) => SetAction.Remove;

		protected override SetAction OnRemovedFromRight(bool existsInLeft) { throw new NotSupportedException(); }
	}
}
