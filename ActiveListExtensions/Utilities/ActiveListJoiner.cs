﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveListJoiner<TLeft, TRight, TResult> : ObservableList<TResult>
	{
		private int _leftItemCount = 0;
		private readonly CollectionWrapper<TLeft> _leftCollectionWrappper;

		private int _rightItemCount = 0;
		private readonly CollectionWrapper<TRight> _rightCollectionWrappper;

		private readonly ActiveListJoinBehaviour _joinBehaviour;

		private readonly Func<TLeft, TRight, TResult> _resultSelector;

		public bool SupportsInner => _joinBehaviour.HasFlag(ActiveListJoinBehaviour.Inner);
		public bool SupportsLeft => _joinBehaviour.HasFlag(ActiveListJoinBehaviour.LeftExcluding);
		public bool SupportsRight => _joinBehaviour.HasFlag(ActiveListJoinBehaviour.RightExcluding);

		public ActiveListJoiner(ActiveListJoinBehaviour joinBehaviour, IReadOnlyList<TLeft> left, IReadOnlyList<TRight> right, Func<TLeft, TRight, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
		{
			_joinBehaviour = joinBehaviour;

			_resultSelector = resultSelector;

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

			Initialize();
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

		private void Initialize()
		{
			_leftItemCount = _leftCollectionWrappper.Count;
			_rightItemCount = _rightCollectionWrappper.Count;

			if (_leftItemCount > 0)
			{
				if (_rightItemCount > 0)
				{
					if (SupportsInner)
						Reset(_leftCollectionWrappper.SelectMany(l => _rightCollectionWrappper.Select(r => _resultSelector.Invoke(l, r))));
				}
				else if (SupportsLeft)
					Reset(_leftCollectionWrappper.Select(l => _resultSelector.Invoke(l, default(TRight))));
			}
			else if (_rightItemCount > 0 && SupportsRight)
				Reset(_rightCollectionWrappper.Select(r => _resultSelector.Invoke(default(TLeft), r)));
		}

		private void AddLeftValue(int index, TLeft value)
		{
			if (!SupportsLeft)
				return;

			_leftItemCount = _leftCollectionWrappper.Count;

			Add(index, GetLeftResult(value));
		}

		private void RemoveLeftValue(int index)
		{
			if (!SupportsLeft)
				return;

			_leftItemCount = _leftCollectionWrappper.Count;

			Remove(index);
		}

		private void ReplaceLeftValue(int index, TLeft value)
		{
			if (!SupportsLeft)
				return;

			Replace(index, GetLeftResult(value));
		}

		private void AddRightValue(int index, TRight value)
		{
			if (!SupportsRight)
				return;

			_rightItemCount = _rightCollectionWrappper.Count;

			Add(index, GetRightResult(value));
		}

		private void RemoveRightValue(int index)
		{
			if (!SupportsRight)
				return;

			_rightItemCount = _rightCollectionWrappper.Count;

			Remove(index);
		}

		private void ReplaceRightValue(int index, TRight value)
		{
			if (!SupportsRight)
				return;

			Replace(index, GetRightResult(value));
		}

		private void AddLeftIntersectionValue(int index, TLeft value)
		{
			var leftItemCount = _leftItemCount;

			var replaceCount = leftItemCount == 0 && SupportsRight ? _rightItemCount : 0;

			_leftItemCount = _leftCollectionWrappper.Count;

			if (!SupportsInner && leftItemCount > 0)
				return;

			var replaceWith = SupportsInner ? _rightCollectionWrappper.Select(r => GetResult(value, r)).ToArray() : new TResult[0];

			ReplaceRange(index * _rightItemCount, replaceCount, replaceWith);
		}

		private void RemoveLeftIntersectionValue(int index)
		{
			_leftItemCount = _leftCollectionWrappper.Count;

			var replaceWith = SupportsRight && _leftItemCount == 0 ? _rightCollectionWrappper.Select(r => GetRightResult(r)).ToArray() : new TResult[0];

			if (SupportsInner)
				ReplaceRange(index * _rightItemCount, _rightItemCount, replaceWith);
			else
				ReplaceRange(0, 0, replaceWith);
		}

		private void ReplaceLeftIntersectionValue(int index, TLeft value)
		{
			if (!SupportsInner)
				return;

			ReplaceRange(index * _rightItemCount, _rightItemCount, _rightCollectionWrappper.Select(r => GetResult(value, r)).ToArray());
		}

		private void AddRightIntersectionValue(int index, TRight value)
		{
			var rightItemCount = _rightItemCount;

			_rightItemCount = _rightCollectionWrappper.Count;

			if (rightItemCount > 0)
			{
				if (!SupportsInner)
					return;

				for (int i = _leftItemCount - 1; i >= 0; --i)
				{
					var newIndex = i * rightItemCount + index;

					Add(newIndex, GetResult(_leftCollectionWrappper[i], value));
				}
			}
			else
			{
				var replaceWith = SupportsInner ? _leftCollectionWrappper.Select(l => GetResult(l, value)).ToArray() : new TResult[0];

				ReplaceRange(0, SupportsLeft ? _leftItemCount : 0, replaceWith);
			}
		}

		private void RemoveRightIntersectionValue(int index)
		{
			var rightItemCount = _rightItemCount;

			_rightItemCount = _rightCollectionWrappper.Count;

			if (rightItemCount == 1)
			{
				var replaceWith = SupportsLeft ? _leftCollectionWrappper.Select(l => GetLeftResult(l)).ToArray() : new TResult[0];

				ReplaceRange(0, SupportsInner ? _leftItemCount : 0, replaceWith);
			}
			else if (SupportsInner)
			{
				for (int i = _leftItemCount - 1; i >= 0; --i)
				{
					var oldIndex = i * rightItemCount + index;

					Remove(oldIndex);
				}
			}
		}

		private void ReplaceRightIntersectionValue(int index, TRight value)
		{
			if (!SupportsInner)
				return;

			for (int i = 0; i < _leftItemCount; ++i)
			{
				var currentIndex = i * _rightItemCount + index;

				Replace(currentIndex, GetResult(_leftCollectionWrappper[i], value));
			}
		}

		private TResult GetLeftResult(TLeft left)
			=> _resultSelector.Invoke(left, default(TRight));

		private TResult GetRightResult(TRight right)
			=> _resultSelector.Invoke(default(TLeft), right);

		private TResult GetResult(TLeft left, TRight right)
			=> _resultSelector.Invoke(left, right);

		private void OnLeftAdded(int index, TLeft value)
		{
			if (_rightItemCount > 0)
				AddLeftIntersectionValue(index, value);
			else
				AddLeftValue(index, value);
		}

		private void OnLeftRemoved(int index, TLeft value)
		{
			if (_rightItemCount > 0)
				RemoveLeftIntersectionValue(index);
			else
				RemoveLeftValue(index);
		}

		private void OnLeftReplaced(int index, TLeft oldValue, TLeft newValue)
		{
			if (_rightItemCount > 0)
				ReplaceLeftIntersectionValue(index, newValue);
			else
				ReplaceLeftValue(index, newValue);
		}

		private void OnLeftMoved(int oldIndex, int newIndex, TLeft value)
		{
		}

		private void OnLeftReset(IReadOnlyList<TLeft> newItems)
		{
		}

		private void OnRightAdded(int index, TRight value)
		{
			if (_leftItemCount > 0)
				AddRightIntersectionValue(index, value);
			else
				AddRightValue(index, value);
		}

		private void OnRightRemoved(int index, TRight value)
		{
			if (_leftItemCount > 0)
				RemoveRightIntersectionValue(index);
			else
				RemoveRightValue(index);
		}

		private void OnRightReplaced(int index, TRight oldValue, TRight newValue)
		{
			if (_leftItemCount > 0)
				ReplaceRightIntersectionValue(index, newValue);
			else
				ReplaceRightValue(index, newValue);
		}

		private void OnRightMoved(int oldIndex, int newIndex, TRight value)
		{
		}

		private void OnRightReset(IReadOnlyList<TRight> newItems)
		{
		}
	}
}