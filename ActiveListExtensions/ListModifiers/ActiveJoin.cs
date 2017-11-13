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
		private class JoinerData
		{
			public int? LeftSourceIndex { get; set; }

			public int? RightSourceIndex { get; set; }

			public int TargetIndex => LeftSourceIndex ?? RightSourceIndex ?? 0;

			public int Offset { get; set; }

			public int Count { get; set; }

			public ActiveListJoiner<TLeft, TRight, TResult, TParameter> Joiner { get; }

			public JoinerData(ActiveListJoiner<TLeft, TRight, TResult, TParameter> joiner)
				=> Joiner = joiner;
		}

		public override int Count => _resultList.Count;

		public override TResult this[int index] => _resultList[index];

		private readonly CollectionWrapper<IActiveGrouping<TKey, TLeft>> _leftGroups;

		private readonly CollectionWrapper<IActiveGrouping<TKey, TRight>> _rightGroups;

		private readonly QuickList<JoinerData> _leftJoiners = new QuickList<JoinerData>();

		private readonly QuickList<JoinerData> _rightJoiners = new QuickList<JoinerData>();

		private readonly IDictionary<TKey, JoinerData> _joiners = new Dictionary<TKey, JoinerData>();

		private readonly ActiveListJoinBehaviour _joinBehaviour;

		private readonly IActiveValue<TParameter> _parameter;

		private readonly Func<TLeft, TRight, TParameter, TResult> _resultSelector;

		private readonly IEnumerable<string> _leftResultSelectorPropertiesToWatch;
		private readonly IEnumerable<string> _rightResultSelectorPropertiesToWatch;
		private readonly IEnumerable<string> _resultSelectorParameterPropertiesToWatch;

		private readonly ObservableList<TResult> _resultList;

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
			_joinBehaviour = joinBehaviour;
			_parameter = parameter;
			_resultSelector = resultSelector;

			_leftResultSelectorPropertiesToWatch = leftResultSelectorPropertiesToWatch;
			_rightResultSelectorPropertiesToWatch = rightResultSelectorPropertiesToWatch;
			_resultSelectorParameterPropertiesToWatch = resultSelectorParameterPropertiesToWatch;

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

			_resultList = new ObservableList<TResult>();
			_resultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);
			_resultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
		}

		private void OnLeftAdded(int index, IActiveGrouping<TKey, TLeft> value)
		{
			if (!_joiners.TryGetValue(value.Key, out var joiner))
				_joiners.Add(value.Key, CreateJoiner(index, value, null, null));
			else
			{
				_leftJoiners.Add(joiner.LeftSourceIndex.Value, joiner);

				for (int i = joiner.LeftSourceIndex.Value + 1; i < _leftJoiners.Count; ++i)
					_leftJoiners[i].LeftSourceIndex = i;

				joiner.Joiner.SetLeft(value);
			}
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
			if (!_joiners.TryGetValue(value.Key, out var joiner))
				_joiners.Add(value.Key, CreateJoiner(null, null, index, value));
			else
			{
				_rightJoiners.Add(joiner.RightSourceIndex.Value, joiner);

				for (int i = joiner.RightSourceIndex.Value + 1; i < _rightJoiners.Count; ++i)
					_rightJoiners[i].RightSourceIndex = i;

				joiner.Joiner.SetRight(value);
			}
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

		private JoinerData CreateJoiner(int? leftSourceIndex, IReadOnlyList<TLeft> left, int? rightSourceIndex, IReadOnlyList<TRight> right)
		{
			var joiner = new ActiveListJoiner<TLeft, TRight, TResult, TParameter>(_joinBehaviour, _parameter, _resultSelector, _leftResultSelectorPropertiesToWatch, _rightResultSelectorPropertiesToWatch, _resultSelectorParameterPropertiesToWatch);

			var data = new JoinerData(joiner);

			joiner.AddRequested += (index, value) =>
			{
				++data.Count;

				UpdateIndices(data.TargetIndex, data.Offset);

				_resultList.Add(data.Offset + index, value);
			};
			joiner.RemoveRequested += index =>
			{
				--data.Count;

				UpdateIndices(data.TargetIndex, data.Offset);

				_resultList.Remove(data.Offset + index);
			};
			joiner.ReplaceRequested += (index, newValue) =>
			{
				_resultList.Replace(data.Offset, newValue);
			};
			joiner.ReplaceRangeRequested += (index, oldCount, values) =>
			{
				var diff = values.Count - oldCount;

				if (diff != 0)
				{
					data.Count += diff;

					UpdateIndices(data.TargetIndex, data.Offset);
				}

				_resultList.ReplaceRange(data.Offset + index, oldCount, values);
			};
			joiner.MoveRequested += (oldIndex, newIndex) =>
			{
				_resultList.Move(data.Offset + oldIndex, data.Offset + newIndex);
			};
			joiner.MoveRangeRequested += (oldIndex, newIndex, count) =>
			{
				_resultList.MoveRange(data.Offset + oldIndex, data.Offset + newIndex, count);
			};
			joiner.ResetRequested += values =>
			{
				var newValues = values.ToArray();

				var oldCount = data.Count;
				if (data.Count != newValues.Length)
				{
					data.Count = newValues.Length;

					UpdateIndices(data.TargetIndex, data.Offset);
				}

				_resultList.ReplaceRange(data.Offset, oldCount, newValues);
			};

			data.LeftSourceIndex = leftSourceIndex;
			data.RightSourceIndex = rightSourceIndex;

			if (data.LeftSourceIndex.HasValue)
			{
				_leftJoiners.Add(data.LeftSourceIndex.Value, data);

				for (int i = data.LeftSourceIndex.Value + 1; i < _leftJoiners.Count; ++i)
					_leftJoiners[i].LeftSourceIndex = i;
			}

			if (data.RightSourceIndex.HasValue)
			{
				_rightJoiners.Add(data.RightSourceIndex.Value, data);

				for (int i = data.RightSourceIndex.Value + 1; i < _rightJoiners.Count; ++i)
					_rightJoiners[i].RightSourceIndex = i;
			}

			UpdateIndices(0, 0);

			joiner.SetBoth(left, right);

			return data;
		}

		private void UpdateIndices(int startIndex, int offset)
		{
			for (int i = startIndex; i < _leftJoiners.Count; ++i)
			{
				_leftJoiners[i].Offset = offset;
				offset += _leftJoiners[i].Count;
			}

			startIndex -= _leftJoiners.Count;
			if (startIndex < 0)
				startIndex = 0;

			for (int i = startIndex; i < _rightJoiners.Count; ++i)
			{
				if (_rightJoiners[i].LeftSourceIndex.HasValue)
					continue;
				_rightJoiners[i].Offset = offset;
				offset += _rightJoiners[i].Count;
			}
		}

		public override IEnumerator<TResult> GetEnumerator()
			=> _resultList.GetEnumerator();
	}
}
