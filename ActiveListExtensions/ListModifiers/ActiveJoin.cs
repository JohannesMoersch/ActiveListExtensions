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
		private readonly CollectionWrapper<IActiveGrouping<TKey, TLeft>> _leftGroups;

		private readonly CollectionWrapper<IActiveGrouping<TKey, TRight>> _rightGroups;

		private readonly IDictionary<TKey, ActiveListJoiner<TLeft, TRight, TResult, TParameter>> _joiners;

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
			_joiners = new Dictionary<TKey, ActiveListJoiner<TLeft, TRight, TResult, TParameter>>();

			_leftGroups = new CollectionWrapper<IActiveGrouping<TKey, TLeft>>(left);
			_leftGroups.ItemModified += (s, i, v) => OnLeftReplaced(i, v, v);
			_leftGroups.ItemAdded += (s, i, v) => OnLeftAdded(i, v);
			_leftGroups.ItemRemoved += (s, i, v) => OnLeftRemoved(i, v);
			_leftGroups.ItemReplaced += (s, i, o, n) => OnLeftReplaced(i, o, n);
			_leftGroups.ItemMoved += (s, o, n, v) => OnLeftMoved(o, n, v);
			_leftGroups.ItemsReset += s => OnLeftReset(s);

			_rightGroups = new CollectionWrapper<IActiveGrouping<TKey, TRight>>(right);
			_rightGroups.ItemModified += (s, i, v) => OnRightReplaced(i, v, v);
			_rightGroups.ItemAdded += (s, i, v) => OnRightAdded(i, v);
			_rightGroups.ItemRemoved += (s, i, v) => OnRightRemoved(i, v);
			_rightGroups.ItemReplaced += (s, i, o, n) => OnRightReplaced(i, o, n);
			_rightGroups.ItemMoved += (s, o, n, v) => OnRightMoved(o, n, v);
			_rightGroups.ItemsReset += s => OnRightReset(s);
		}

		private void OnLeftAdded(int index, IActiveGrouping<TKey, TLeft> value)
		{
		}

		private void OnLeftRemoved(int index, IActiveGrouping<TKey, TLeft> value)
		{
		}

		private void OnLeftReplaced(int index, IActiveGrouping<TKey, TLeft> oldValue, IActiveGrouping<TKey, TLeft> newValue)
		{
		}

		private void OnLeftMoved(int oldIndex, int newIndex, IActiveGrouping<TKey, TLeft> value)
		{
		}

		private void OnLeftReset(IReadOnlyList<IActiveGrouping<TKey, TLeft>> newItems)
		{
		}

		private void OnRightAdded(int index, IActiveGrouping<TKey, TRight> value)
		{
		}

		private void OnRightRemoved(int index, IActiveGrouping<TKey, TRight> value)
		{
		}

		private void OnRightReplaced(int index, IActiveGrouping<TKey, TRight> oldValue, IActiveGrouping<TKey, TRight> newValue)
		{
		}

		private void OnRightMoved(int oldIndex, int newIndex, IActiveGrouping<TKey, TRight> value)
		{
		}

		private void OnRightReset(IReadOnlyList<IActiveGrouping<TKey, TRight>> newItems)
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
