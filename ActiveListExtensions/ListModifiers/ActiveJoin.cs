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

		private class JoinerSets
		{
			public IList<JoinerData> Joiners { get; } = new List<JoinerData>();

			public JoinerData NullJoiner { get; set; }
		}

		public override int Count => _resultList.Count;

		public override TResult this[int index] => _resultList[index];

		private readonly CollectionWrapper<KeyValuePair<TKey, TLeft>> _leftItems;

		private readonly IActiveLookup<TKey, TRight> _rightItems;

		private readonly CollectionWrapper<IActiveGrouping<TKey, TRight>> _rightGroups;

		private readonly QuickList<JoinerData> _leftJoiners;

		private readonly QuickList<JoinerData> _rightJoiners;

		private readonly IDictionary<TKey, JoinerSets> _joinerLookup;

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
			_joinerLookup = new Dictionary<TKey, JoinerSets>();

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
			OnRightReset(_rightGroups);
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
			if (Equals(oldValue.Key, newValue.Key))
				_leftJoiners[index].Joiner.SetLeft(newValue.Value);
			else
			{
				OnLeftRemoved(index, oldValue);
				OnLeftAdded(index, newValue);
			}
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
			var total = _rightJoiners.Sum(joiner => joiner?.Count ?? 0);
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
			var joiner = AddRightJoiner(index, value.Key);

			joiner?.Joiner.SetRight(value);
		}

		private void OnRightRemoved(int index, IActiveGrouping<TKey, TRight> value)
		{
			var joiner = _rightJoiners[index];
			if (joiner != null)
			{
				_resultList.ReplaceRange(joiner.Offset, joiner.Count, new TResult[0]);

				joiner.Joiner.Dispose();

				_rightJoiners[index] = null;
			}
		}

		private void OnRightReplaced(int index, IActiveGrouping<TKey, TRight> oldValue, IActiveGrouping<TKey, TRight> newValue)
		{
		}

		private void OnRightMoved(int oldIndex, int newIndex, IActiveGrouping<TKey, TRight> value)
		{
		}

		private void OnRightReset(IReadOnlyList<IActiveGrouping<TKey, TRight>> newItems)
		{
			for (int i = _rightJoiners.Count - 1; i >= 0; --i)
			{
				if (_rightJoiners[i] == null)
					continue;

				_rightJoiners[i].Joiner.Dispose();

				_resultList.ReplaceRange(_rightJoiners[i].Offset, _rightJoiners[i].Count, new TResult[0]); 
			}

			_rightJoiners.Clear();

			foreach (var set in _joinerLookup.Values)
				set.NullJoiner = null;

			for (int i = 0; i < newItems.Count; ++i)
				OnRightAdded(i, newItems[i]);
		}

		private void Check()
		{
			if (_leftJoiners.Concat(_rightJoiners).Where(j => j != null).Max(j => j.Offset + j.Count) != _resultList.Count)
				Console.WriteLine();
		}

		private JoinerData CreateJoiner()
		{
			var joiner = new ActiveListJoiner<TLeft, TRight, TResult, TParameter>(_joinBehaviour, _parameter, _resultSelector, _leftResultSelectorPropertiesToWatch, _rightResultSelectorPropertiesToWatch, _resultSelectorParameterPropertiesToWatch);

			var data = new JoinerData(joiner);

			joiner.AddRequested += (index, value) =>
			{
				Check();

				++data.Count;

				UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));

				_resultList.Add(data.Offset + index, value);

				Check();
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
				Check();

				var newValues = values.ToArray();

				var oldCount = data.Count;
				if (data.Count != newValues.Length)
				{
					data.Count = newValues.Length;

					UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));
				}

				_resultList.ReplaceRange(data.Offset, oldCount, newValues);

				Check();
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
				joiners = new JoinerSets();
				_joinerLookup.Add(key, joiners);
			}

			if (joiners.NullJoiner != null)
			{
				var oldOffset = joiners.NullJoiner.Offset;
				var oldCount = joiners.NullJoiner.Count;

				UpdateIndices(joiners.NullJoiner.GetTargetIndex(_leftJoiners.Count));

				joiners.NullJoiner.Joiner.Dispose();

				joiners.NullJoiner = null;

				_resultList.ReplaceRange(oldOffset, oldCount, new TResult[0]);
			}

			joiners.Joiners.Add(joiner);

			UpdateIndices(joinerIndex);

			return joiner;
		}

		private JoinerData AddRightJoiner(int joinerIndex, TKey key)
		{
			if (!_joinerLookup.TryGetValue(key, out var joiners))
			{
				joiners = new JoinerSets();
				_joinerLookup.Add(key, joiners);
			}
			if (joiners.NullJoiner == null && joiners.Joiners.Count == 0)
			{
				joiners.NullJoiner = CreateJoiner();

				joiners.NullJoiner.RightSourceIndex = joinerIndex;
			}

			_rightJoiners.Add(joinerIndex, joiners.NullJoiner);

			return joiners.NullJoiner;
		}

		private void UpdateIndices(int startIndex)
		{
			int offset;
			if (startIndex == 0)
				offset = 0;
			else
			{
				while (!(--startIndex < _leftJoiners.Count ? _leftJoiners[startIndex].Offset : _rightJoiners[startIndex - _leftJoiners.Count]?.Offset).HasValue)
					continue;
				offset = startIndex < _leftJoiners.Count ? _leftJoiners[startIndex].Offset : _rightJoiners[startIndex - _leftJoiners.Count].Offset;
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
				if (_rightJoiners[i] == null)
					continue;

				_rightJoiners[i].Offset = offset;
				offset += _rightJoiners[i].Count;
			}
		}

		public override IEnumerator<TResult> GetEnumerator()
			=> _resultList.GetEnumerator();
	}
}
