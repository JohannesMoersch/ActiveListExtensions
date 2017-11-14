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

			public int RightSourceIndex { get; set; }

			public int GetTargetIndex(int leftCount)
				=> LeftSourceIndex ?? (RightSourceIndex + leftCount);

			public int Offset { get; set; }

			public int Count { get; set; }

			public ActiveListJoiner<TLeft, TRight, TResult, TParameter> Joiner { get; }

			public JoinerData(ActiveListJoiner<TLeft, TRight, TResult, TParameter> joiner)
				=> Joiner = joiner;
		}

		public override int Count => _resultList.Count;

		public override TResult this[int index] => _resultList[index];

		private readonly CollectionWrapper<KeyValuePair<TKey, TLeft>> _leftItems;

		private readonly IActiveLookup<TKey, TRight> _rightItems;

		private readonly CollectionWrapper<IActiveGrouping<TKey, TRight>> _rightGroups;

		private readonly QuickList<JoinerData> _leftJoiners;

		private readonly QuickList<JoinerData> _rightJoiners;

		private readonly IDictionary<TKey, IList<JoinerData>> _joinerLookup;

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
				source.ActiveSelect(l => new KeyValuePair<TKey, TLeft>(leftKeySelector.Invoke(l), l), leftKeySelectorPropertiesToWatch),
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
				source.ActiveSelect(parameter, (l, p) => new KeyValuePair<TKey, TLeft>(leftKeySelector.Invoke(l, p), l), leftKeySelectorPropertiesToWatch, leftKeySelectorParameterPropertiesToWatch),
				join.ToActiveList().ToActiveLookup(parameter, rightKeySelector, rightKeySelectorPropertiesToWatch, rightKeySelectorParameterPropertiesToWatch),
				parameter,
				resultSelector, 
				leftResultSelectorPropertiesToWatch,
				rightResultSelectorPropertiesToWatch,
				resultSelectorParameterPropertiesToWatch)
		{
		}

		private ActiveJoin(ActiveListJoinBehaviour joinBehaviour, IActiveList<KeyValuePair<TKey, TLeft>> left, IActiveLookup<TKey, TRight> right, IActiveValue<TParameter> parameter, Func<TLeft, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
		{
			_joinBehaviour = joinBehaviour;
			_parameter = parameter;
			_resultSelector = resultSelector;

			_leftResultSelectorPropertiesToWatch = leftResultSelectorPropertiesToWatch;
			_rightResultSelectorPropertiesToWatch = rightResultSelectorPropertiesToWatch;
			_resultSelectorParameterPropertiesToWatch = resultSelectorParameterPropertiesToWatch;

			_leftJoiners = new QuickList<JoinerData>();
			_rightJoiners = new QuickList<JoinerData>();
			_joinerLookup = new Dictionary<TKey, IList<JoinerData>>();

			_leftItems = new CollectionWrapper<KeyValuePair<TKey, TLeft>>(left);
			_leftItems.ItemModified += (s, i, v) => OnLeftReplaced(i, v, v);
			_leftItems.ItemAdded += (s, i, v) => OnLeftAdded(i, v);
			_leftItems.ItemRemoved += (s, i, v) => OnLeftRemoved(i, v);
			_leftItems.ItemReplaced += (s, i, o, n) => OnLeftReplaced(i, o, n);
			_leftItems.ItemMoved += (s, o, n, v) => OnLeftMoved(o, n, v);
			_leftItems.ItemsReset += s => OnLeftReset(s);

			_rightItems = right;

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

			OnLeftReset(_leftItems);
		}

		private void OnLeftAdded(int index, KeyValuePair<TKey, TLeft> value)
		{
			var joiner = AddLeftJoiner(index, value.Key);

			joiner.Joiner.SetBoth(value.Value, _rightItems[value.Key]);
		}

		private void OnLeftRemoved(int index, KeyValuePair<TKey, TLeft> value)
		{
			var oldJoiner = _leftJoiners[index];

			_leftJoiners.Remove(index);

			for (int i = index; i < _leftJoiners.Count; ++i)
				_leftJoiners[i].LeftSourceIndex = i;

			UpdateIndices(index);

			oldJoiner.Joiner.Dispose();

			_resultList.ReplaceRange(oldJoiner.Offset, oldJoiner.Count, new TResult[0]);
		}

		private void OnLeftReplaced(int index, KeyValuePair<TKey, TLeft> oldValue, KeyValuePair<TKey, TLeft> newValue)
		{
			OnLeftRemoved(index, oldValue);
			OnLeftAdded(index, newValue);
		}

		private void OnLeftMoved(int oldIndex, int newIndex, KeyValuePair<TKey, TLeft> value)
		{
			var startIndex = _leftJoiners[oldIndex].Offset;
			int endIndex;
			if (oldIndex < newIndex)
				endIndex = (_leftJoiners[newIndex].Offset + _leftJoiners[newIndex].Count) - _leftJoiners[oldIndex].Count;
			else
				endIndex = _leftJoiners[newIndex].Offset;
			var count = _leftJoiners[oldIndex].Count;

			var min = oldIndex < newIndex ? oldIndex : newIndex;
			var max = oldIndex > newIndex ? oldIndex : newIndex;

			_leftJoiners.Move(oldIndex, newIndex);

			for (int i = min; i <= max; ++i)
				_leftJoiners[i].LeftSourceIndex = i;

			UpdateIndices(min);

			_resultList.MoveRange(startIndex, endIndex, count);
		}

		private void OnLeftReset(IReadOnlyList<KeyValuePair<TKey, TLeft>> newItems)
		{
			var total = _rightJoiners.Sum(joiner => joiner.Count);
			_resultList.ReplaceRange(0, _resultList.Count - total, new TResult[0]);

			foreach (var joiner in _leftJoiners)
				joiner.Joiner.Dispose();

			_leftJoiners.Clear();

			UpdateIndices(0);

			for (int i = 0; i < newItems.Count; ++i)
				OnLeftAdded(i, newItems[i]);
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

		private JoinerData CreateJoiner()
		{
			var joiner = new ActiveListJoiner<TLeft, TRight, TResult, TParameter>(_joinBehaviour, _parameter, _resultSelector, _leftResultSelectorPropertiesToWatch, _rightResultSelectorPropertiesToWatch, _resultSelectorParameterPropertiesToWatch);

			var data = new JoinerData(joiner);

			joiner.AddRequested += (index, value) =>
			{
				++data.Count;

				UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));

				_resultList.Add(data.Offset + index, value);
			};
			joiner.RemoveRequested += index =>
			{
				--data.Count;

				UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));

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

					UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));
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

					UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));
				}

				_resultList.ReplaceRange(data.Offset, oldCount, newValues);
			};

			return data;
		}

		private JoinerData AddLeftJoiner(int joinerIndex, TKey key)
		{
			var joiner = CreateJoiner();

			_leftJoiners.Add(joinerIndex, joiner);

			for (int i = joinerIndex; i < _leftJoiners.Count; ++i)
				_leftJoiners[i].LeftSourceIndex = i;

			if (!_joinerLookup.TryGetValue(key, out var joiners))
			{
				joiners = new List<JoinerData>();
				_joinerLookup.Add(key, joiners);
			}

			joiners.Add(joiner);

			UpdateIndices(joinerIndex);

			return joiner;
		}

		private JoinerData AddRightJoiner(int joinerIndex, TKey key)
		{
			var joiner = CreateJoiner();

			_rightJoiners.Add(joinerIndex, joiner);

			if (!_joinerLookup.TryGetValue(key, out var joiners))
			{
				joiners = new List<JoinerData>();
				_joinerLookup.Add(key, joiners);
			}

			joiners.Add(joiner);

			return joiner;
		}

		private void UpdateIndices(int startIndex)
		{
			int offset;
			if (startIndex == 0)
				offset = 0;
			else
			{
				--startIndex;
				offset = startIndex < _leftJoiners.Count ? _leftJoiners[startIndex].Offset : _rightJoiners[startIndex].Offset;
			}

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
				_rightJoiners[i].Offset = offset;
				offset += _rightJoiners[i].Count;
			}
		}

		public override IEnumerator<TResult> GetEnumerator()
			=> _resultList.GetEnumerator();
	}
}
