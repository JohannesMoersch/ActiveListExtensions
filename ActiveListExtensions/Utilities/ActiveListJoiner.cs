using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveListJoiner<TLeft, TRight, TResult> : ObservableList<TResult>
	{
		private readonly CollectionWrapper<TLeft> _leftCollectionWrappper;

		private readonly CollectionWrapper<TRight> _rightCollectionWrappper;

		private readonly ActiveListJoinBehaviour _joinBehaviour;

		public ActiveListJoiner(ActiveListJoinBehaviour joinBehaviour, IReadOnlyList<TLeft> left, IReadOnlyList<TRight> right, Func<TLeft, TRight, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
		{
			_joinBehaviour = joinBehaviour;

			_leftCollectionWrappper = new CollectionWrapper<TLeft>(left, leftResultSelectorPropertiesToWatch?.ToArray());
			_leftCollectionWrappper.ItemModified += (s, i, v) => OnLeftReplaced(i, v, v);
			_leftCollectionWrappper.ItemAdded += (s, i, v) => OnLeftAdded(i, v);
			_leftCollectionWrappper.ItemRemoved += (s, i, v) => OnLeftRemoved(i, v);
			_leftCollectionWrappper.ItemReplaced += (s, i, o, n) => OnLeftReplaced(i, o, n);
			_leftCollectionWrappper.ItemMoved += (s, o, n, v) => OnLeftMoved(o, n, v);
			_leftCollectionWrappper.ItemsReset += s => OnLeftReset(s);

			_rightCollectionWrappper = new CollectionWrapper<TRight>(right, rightResultSelectorPropertiesToWatch?.ToArray());
			_rightCollectionWrappper.ItemModified += (s, i, v) => OnRightReplaced(i, v, v);
			_rightCollectionWrappper.ItemAdded += (s, i, v) => OnRightAdded(i, v);
			_rightCollectionWrappper.ItemRemoved += (s, i, v) => OnRightRemoved(i, v);
			_rightCollectionWrappper.ItemReplaced += (s, i, o, n) => OnRightReplaced(i, o, n);
			_rightCollectionWrappper.ItemMoved += (s, o, n, v) => OnRightMoved(o, n, v);
			_rightCollectionWrappper.ItemsReset += s => OnRightReset(s);
		}

		public override void Dispose()
		{
			_leftCollectionWrappper.Dispose();
			_rightCollectionWrappper.Dispose();

			base.Dispose();
		}

		public void SetLeft(IReadOnlyList<TLeft> left)
			=> _leftCollectionWrappper.ReplaceCollection(left);

		public void SetRight(IReadOnlyList<TRight> right)
			=> _rightCollectionWrappper.ReplaceCollection(right);

		private void OnLeftAdded(int index, TLeft value)
		{
		}

		private void OnLeftRemoved(int index, TLeft value)
		{
		}

		private void OnLeftReplaced(int index, TLeft oldValue, TLeft newValue)
		{
		}

		private void OnLeftMoved(int oldIndex, int newIndex, TLeft value)
		{
		}

		private void OnLeftReset(IReadOnlyList<TLeft> newItems)
		{
		}

		private void OnRightAdded(int index, TRight value)
		{
		}

		private void OnRightRemoved(int index, TRight value)
		{
		}

		private void OnRightReplaced(int index, TRight oldValue, TRight newValue)
		{
		}

		private void OnRightMoved(int oldIndex, int newIndex, TRight value)
		{
		}

		private void OnRightReset(IReadOnlyList<TRight> newItems)
		{
		}
	}
}
