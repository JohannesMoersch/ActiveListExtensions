using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.ListModifiers.Bases;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveUnion<TKey, TSource, TParameter> : ActiveSetBase<TKey, TSource, TParameter> 
	{
		public ActiveUnion(IActiveList<TSource> leftSource, IReadOnlyList<TSource> rightSource, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			: this(leftSource, rightSource, keySelector, null, propertiesToWatch, null)
		{
		}

		public ActiveUnion(IActiveList<TSource> leftSource, IReadOnlyList<TSource> rightSource, Func<TSource, TParameter, TKey> keySelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) 
			: this(leftSource, rightSource, i => keySelector.Invoke(i, parameter.Value), parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveUnion(IActiveList<TSource> leftSource, IReadOnlyList<TSource> rightSource, Func<TSource, TKey> keySelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(leftSource, rightSource, keySelector, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		protected override SetAction OnAddedToLeft(bool existsInRight) => !existsInRight ? SetAction.Add : SetAction.None;

		protected override SetAction OnAddedToRight(bool existsInLeft) => !existsInLeft ? SetAction.Add : SetAction.None;

		protected override SetAction OnRemovedFromLeft(bool existsInRight) => !existsInRight ? SetAction.Remove : SetAction.None;

		protected override SetAction OnRemovedFromRight(bool existsInLeft) => !existsInLeft ? SetAction.Remove : SetAction.None;
	}
}
