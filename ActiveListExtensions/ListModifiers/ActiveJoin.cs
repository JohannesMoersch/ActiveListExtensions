using ActiveListExtensions.ListModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
	public enum ActiveJoinBehaviour
	{
		Inner,
		Left,
		LeftExcluding,
		Right,
		RightExcluding,
		Outer,
		OuterExcluding
	}

	public class ActiveJoin<TLeft, TRight, TResult, TKey, TParameter> : ActiveBase<TResult>
	{
		public ActiveJoin(ActiveJoinBehaviour joinBehaviour, IActiveList<TLeft> source, IReadOnlyList<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			: this(joinBehaviour, source, join, null, (l, p) => leftKeySelector.Invoke(l), (r, p) => rightKeySelector.Invoke(r), (l, r, p) => resultSelector.Invoke(l, r), leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, null)
		{
		}

		public ActiveJoin(ActiveJoinBehaviour joinBehaviour, IActiveList<TLeft> source, IReadOnlyList<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<TLeft, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
		{
		}
	}
}
