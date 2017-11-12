using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveJoin<TLeft, TRight, TResult, TKey, TParameter> : ActiveBase<TResult>
	{
		private List<CollectionWrapper<IActiveGrouping<TKey, TLeft>>> _leftGroups = new List<CollectionWrapper<IActiveGrouping<TKey, TLeft>>>();

		private List<CollectionWrapper<IActiveGrouping<TKey, TRight>>> _rightGroups = new List<CollectionWrapper<IActiveGrouping<TKey, TRight>>>();

		public ActiveJoin(ActiveListJoinBehaviour joinBehaviour, IActiveList<TLeft> source, IReadOnlyList<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			: this(
				joinBehaviour,
				source.ToActiveLookup(leftKeySelector, leftKeySelectorPropertiesToWatch),
				join.ToActiveList().ToActiveLookup(rightKeySelector, rightKeySelectorPropertiesToWatch),
				null,
				(l, r, p) => resultSelector.Invoke(l, r),
				leftResultSelectorPropertiesToWatch,
				rightResultSelectorPropertiesToWatch,
				null)
		{
			;
			;
		}

		public ActiveJoin(ActiveListJoinBehaviour joinBehaviour, IActiveList<TLeft> source, IReadOnlyList<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<TLeft, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> leftKeySelectorParameterPropertiesToWatch, IEnumerable<string> rightKeySelectorParameterPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
			: this(
				joinBehaviour,
				source.ToActiveLookup(parameter, leftKeySelector, leftKeySelectorPropertiesToWatch, leftKeySelectorParameterPropertiesToWatch),
				join.ToActiveList().ToActiveLookup(parameter, rightKeySelector, rightKeySelectorPropertiesToWatch, rightKeySelectorParameterPropertiesToWatch),
				parameter,
				resultSelector, 
				leftResultSelectorPropertiesToWatch,
				rightResultSelectorPropertiesToWatch,
				resultSelectorParameterPropertiesToWatch)
		{
		}

		private ActiveJoin(ActiveListJoinBehaviour joinBehaviour, IActiveLookup<TKey, TLeft> left, IActiveLookup<TKey, TRight> right, IActiveValue<TParameter> parameter, Func<TLeft, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
		{
		}

		public override TResult this[int index] => throw new NotImplementedException();

		public override int Count => throw new NotImplementedException();

		public override IEnumerator<TResult> GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
