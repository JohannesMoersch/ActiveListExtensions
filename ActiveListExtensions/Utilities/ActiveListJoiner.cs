using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveListJoiner<TLeft, TRight, TResult, TParameter> : IDisposable
	{
		public bool HasLeft { get; private set; }

		private IMutableActiveValue<TLeft> _left;

		private int _rightItemCount = 0;
		private readonly CollectionWrapper<TRight> _rightCollectionWrappper;

		private readonly ActiveListJoinBehaviour _joinBehaviour;

		private readonly Func<TLeft, TRight, TParameter, TResult> _resultSelector;

		private readonly ValueWatcher<TLeft> _leftWatcher;

		private readonly ValueWatcher<TParameter> _parameterWatcher;

		private TParameter ParameterValue => _parameterWatcher != null ? _parameterWatcher.Value : default(TParameter);

		private bool SupportsInner => _joinBehaviour.HasFlag(ActiveListJoinBehaviour.Inner);
		private bool SupportsLeft => _joinBehaviour.HasFlag(ActiveListJoinBehaviour.LeftExcluding);
		private bool SupportsRight => _joinBehaviour.HasFlag(ActiveListJoinBehaviour.RightExcluding);

		public ActiveListJoiner(ActiveListJoinBehaviour joinBehaviour, IActiveValue<TParameter> parameter, Func<TLeft, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
		{
			_joinBehaviour = joinBehaviour;

			_resultSelector = resultSelector;

			_left = ActiveValue.Create<TLeft>();
			_leftWatcher = new ValueWatcher<TLeft>(_left, leftResultSelectorPropertiesToWatch);
			_leftWatcher.ValuePropertyChanged += () => OnLeftChanged();

			if (parameter != null)
			{
				_parameterWatcher = new ValueWatcher<TParameter>(parameter, parameterPropertiesToWatch);
				_parameterWatcher.ValueOrValuePropertyChanged += () => OnParameterChanged();
			}

			_rightCollectionWrappper = new CollectionWrapper<TRight>(null, rightResultSelectorPropertiesToWatch?.ToArray());
			_rightCollectionWrappper.ItemModified += (s, i, v) => OnRightReplaced(i, v, v);
			_rightCollectionWrappper.ItemAdded += (s, i, v) => OnRightAdded(i, v);
			_rightCollectionWrappper.ItemRemoved += (s, i, v) => OnRightRemoved(i, v);
			_rightCollectionWrappper.ItemReplaced += (s, i, o, n) => OnRightReplaced(i, o, n);
			_rightCollectionWrappper.ItemMoved += (s, o, n, v) => OnRightMoved(o, n, v);
			_rightCollectionWrappper.ItemsReset += s => OnRightReset(s);
		}

		public void Dispose()
		{
			_rightCollectionWrappper.Dispose();

			_parameterWatcher?.Dispose();

			AddRequested = null;
			RemoveRequested = null;
			ReplaceRequested = null;
			ReplaceRangeRequested = null;
			MoveRequested = null;
			MoveRangeRequested = null;
			ResetRequested = null;
		}

		private void OnParameterChanged()
			=> Reset();

		private void OnLeftChanged()
			=> Reset();

		public void SetLeft(TLeft left)
		{
			HasLeft = true;
			_left.Value = left;
			Reset();
		}

		public void ClearLeft()
		{
			HasLeft = false;
			_left.Value = default(TLeft);
			Reset();
		}

		public void SetRight(IReadOnlyList<TRight> right)
			=> _rightCollectionWrappper.ReplaceCollection(right);

		public void SetBoth(TLeft left, IReadOnlyList<TRight> right)
		{
			HasLeft = true;
			_left.Value = left;
			_rightCollectionWrappper.ReplaceCollection(right);
		}

		public void ClearBoth()
		{
			HasLeft = false;
			_left.Value = default(TLeft);
			_rightCollectionWrappper.ReplaceCollection(new TRight[0]);
		}

		private void Reset()
		{
			_rightItemCount = _rightCollectionWrappper.Count;

			if (HasLeft)
			{
				if (_rightItemCount > 0)
				{
					if (SupportsInner)
					{
						Reset(_rightCollectionWrappper.Select(r => GetResult(_left.Value, r)));
						return;
					}
				}
				else if (SupportsLeft)
				{
					Reset(new[] { GetLeftResult(_left.Value) });
					return;
				}
			}
			else if (_rightItemCount > 0 && SupportsRight)
			{
				Reset(_rightCollectionWrappper.Select(r => GetRightResult(r)));
				return;
			}

			Reset(new TResult[0]);
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

		private void MoveRightValue(int oldIndex, int newIndex)
		{
			if (!SupportsRight)
				return;

			Move(oldIndex, newIndex);
		}

		private void AddRightIntersectionValue(int index, TRight value)
		{
			var rightItemCount = _rightItemCount;

			_rightItemCount = _rightCollectionWrappper.Count;

			if (rightItemCount > 0)
			{
				if (!SupportsInner)
					return;

				Add(index, GetResult(_left.Value, value));
			}
			else
			{
				var replaceWith = SupportsInner ? new[] { GetResult(_left.Value, value) } : new TResult[0];

				ReplaceRange(0, SupportsLeft ? 1 : 0, replaceWith);
			}
		}

		private void RemoveRightIntersectionValue(int index)
		{
			var rightItemCount = _rightItemCount;

			_rightItemCount = _rightCollectionWrappper.Count;

			if (rightItemCount == 1)
			{
				var replaceWith = SupportsLeft ? new[] { GetLeftResult(_left.Value) } : new TResult[0];

				ReplaceRange(0, SupportsInner ? 1 : 0, replaceWith);
			}
			else if (SupportsInner)
				Remove(index);
		}

		private void ReplaceRightIntersectionValue(int index, TRight value)
		{
			if (!SupportsInner)
				return;

			Replace(index, GetResult(_left.Value, value));
		}


		private void MoveRightIntersectionValue(int oldIndex, int newIndex)
		{
			if (!SupportsInner)
				return;

			Move(oldIndex, newIndex);
		}

		private TResult GetLeftResult(TLeft left)
			=> _resultSelector.Invoke(left, default(TRight), ParameterValue);

		private TResult GetRightResult(TRight right)
			=> _resultSelector.Invoke(default(TLeft), right, ParameterValue);

		private TResult GetResult(TLeft left, TRight right)
			=> _resultSelector.Invoke(left, right, ParameterValue);

		private void OnRightAdded(int index, TRight value)
		{
			if (HasLeft)
				AddRightIntersectionValue(index, value);
			else
				AddRightValue(index, value);
		}

		private void OnRightRemoved(int index, TRight value)
		{
			if (HasLeft)
				RemoveRightIntersectionValue(index);
			else
				RemoveRightValue(index);
		}

		private void OnRightReplaced(int index, TRight oldValue, TRight newValue)
		{
			if (HasLeft)
				ReplaceRightIntersectionValue(index, newValue);
			else
				ReplaceRightValue(index, newValue);
		}

		private void OnRightMoved(int oldIndex, int newIndex, TRight value)
		{
			if (HasLeft)
				MoveRightIntersectionValue(oldIndex, newIndex);
			else
				MoveRightValue(oldIndex, newIndex);
		}

		private void OnRightReset(IReadOnlyList<TRight> newItems)
			=> Reset();

		private void Add(int index, TResult result)
			=> AddRequested?.Invoke(index, result);

		private void Remove(int index)
			=> RemoveRequested?.Invoke(index);

		private void Replace(int index, TResult result)
			=> ReplaceRequested?.Invoke(index, result);

		private void ReplaceRange(int index, int oldCount, IReadOnlyList<TResult> results)
			=> ReplaceRangeRequested?.Invoke(index, oldCount, results);

		private void Move(int oldIndex, int newIndex)
			=> MoveRequested?.Invoke(oldIndex, newIndex);

		private void MoveRange(int oldIndex, int newIndex, int count)
			=> MoveRangeRequested?.Invoke(oldIndex, newIndex, count);

		private void Reset(IEnumerable<TResult> results)
			=> ResetRequested?.Invoke(results);

		public event Action<int, TResult> AddRequested;
		public event Action<int> RemoveRequested;
		public event Action<int, TResult> ReplaceRequested;
		public event Action<int, int, IReadOnlyList<TResult>> ReplaceRangeRequested;
		public event Action<int, int> MoveRequested;
		public event Action<int, int, int> MoveRangeRequested;
		public event Action<IEnumerable<TResult>> ResetRequested;
	}
}
