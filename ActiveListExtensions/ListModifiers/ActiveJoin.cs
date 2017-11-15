using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveJoin<TLeft, TRight, TResult, TKey, TParameter> : ActiveBase<TResult>
	{
		public override int Count => _resultList.Count;

		public override TResult this[int index] => _resultList[index];

		private readonly CollectionWrapper<KeyValuePair<TKey, TLeft>> _leftItems;

		private readonly IActiveLookup<TKey, TRight> _rightItems;

		private readonly CollectionWrapper<IActiveGrouping<TKey, TRight>> _rightGroups;

		private readonly QuickList<ActiveListJoinerData<TLeft, TRight, TResult, TKey>> _leftJoiners;

		private readonly QuickList<ActiveListJoinerData<TLeft, TRight, TResult, TKey>> _rightJoiners;

		private readonly IDictionary<TKey, ActiveListJoinerSet<TLeft, TRight, TResult, TKey>> _joinerLookup;

		private readonly ActiveListJoinBehaviour _joinBehaviour;

		private readonly IActiveValue<TParameter> _parameter;

		private readonly Func<JoinOption<TLeft>, JoinOption<TRight>, TParameter, TResult> _resultSelector;

		private readonly IEnumerable<string> _leftResultSelectorPropertiesToWatch;
		private readonly IEnumerable<string> _rightResultSelectorPropertiesToWatch;

		private readonly ValueWatcher<TParameter> _parameterWatcher;

		private TParameter ParameterValue => _parameterWatcher != null ? _parameterWatcher.Value : default(TParameter);

		private readonly ObservableList<TResult> _resultList;

		private bool _fullResetInProgress;

		public ActiveJoin(ActiveListJoinBehaviour joinBehaviour, IActiveList<TLeft> source, IReadOnlyList<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
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

		public ActiveJoin(ActiveListJoinBehaviour joinBehaviour, IActiveList<TLeft> source, IReadOnlyList<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> leftKeySelectorParameterPropertiesToWatch, IEnumerable<string> rightKeySelectorParameterPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
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

		private ActiveJoin(ActiveListJoinBehaviour joinBehaviour, IActiveList<KeyValuePair<TKey, TLeft>> left, IActiveLookup<TKey, TRight> right, IActiveValue<TParameter> parameter, Func<JoinOption<TLeft>, JoinOption<TRight>, TParameter, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
		{
			_joinBehaviour = joinBehaviour;
			_parameter = parameter;
			_resultSelector = resultSelector;

			_leftResultSelectorPropertiesToWatch = leftResultSelectorPropertiesToWatch;
			_rightResultSelectorPropertiesToWatch = rightResultSelectorPropertiesToWatch;

			if (parameter != null)
			{
				_parameterWatcher = new ValueWatcher<TParameter>(parameter, resultSelectorParameterPropertiesToWatch);
				_parameterWatcher.ValueOrValuePropertyChanged += () => OnParameterChanged();
			}

			_leftJoiners = new QuickList<ActiveListJoinerData<TLeft, TRight, TResult, TKey>>();
			_rightJoiners = new QuickList<ActiveListJoinerData<TLeft, TRight, TResult, TKey>>();
			_joinerLookup = new Dictionary<TKey, ActiveListJoinerSet<TLeft, TRight, TResult, TKey>>();

			_leftItems = new CollectionWrapper<KeyValuePair<TKey, TLeft>>(left);
			_leftItems.ItemModified += (s, i, v) => OnLeftReplaced(i, v, v);
			_leftItems.ItemAdded += (s, i, v) => OnLeftAdded(i, v);
			_leftItems.ItemRemoved += (s, i, v) => OnLeftRemoved(i, v);
			_leftItems.ItemReplaced += (s, i, o, n) => OnLeftReplaced(i, o, n);
			_leftItems.ItemMoved += (s, o, n, v) => OnLeftMoved(o, n, v);
			_leftItems.ItemsReset += s => FullReset();

			_rightItems = right;

			_rightGroups = new CollectionWrapper<IActiveGrouping<TKey, TRight>>(right);
			_rightGroups.ItemModified += (s, i, v) => OnRightReplaced(i, v, v);
			_rightGroups.ItemAdded += (s, i, v) => OnRightAdded(i, v);
			_rightGroups.ItemRemoved += (s, i, v) => OnRightRemoved(i, v);
			_rightGroups.ItemReplaced += (s, i, o, n) => OnRightReplaced(i, o, n);
			_rightGroups.ItemMoved += (s, o, n, v) => OnRightMoved(o, n, v);
			_rightGroups.ItemsReset += s => FullReset();

			_resultList = new ObservableList<TResult>();
			_resultList.PropertyChanged += (s, e) =>
			{
				if (!_fullResetInProgress)
					NotifyOfPropertyChange(e);
			};
			_resultList.CollectionChanged += (s, e) =>
			{
				if (!_fullResetInProgress)
					NotifyOfCollectionChange(e);
			};

			FullReset();
		}

		private void FullReset()
		{
			var oldCount = _resultList.Count;

			_fullResetInProgress = true;
			try
			{
				_resultList.Clear();

				foreach (var set in _joinerLookup.Values)
					set.Dispose();

				_joinerLookup.Clear();
				_leftJoiners.Clear();
				_rightJoiners.Clear();

				OnLeftReset(_leftItems);
				OnRightReset(_rightItems);
			}
			finally
			{
				_fullResetInProgress = false;
				if (oldCount != _resultList.Count)
					NotifyOfPropertyChange(new PropertyChangedEventArgs(nameof(Count)));
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		private void OnParameterChanged()
			=> FullReset();

		private ActiveListJoinerSet<TLeft, TRight, TResult, TKey> CreateJoinerSet(TKey key)
		{
			var set = new ActiveListJoinerSet<TLeft, TRight, TResult, TKey>(_joinBehaviour, key, _rightItems, (l, r) => _resultSelector.Invoke(l, r, ParameterValue), _leftResultSelectorPropertiesToWatch, _rightResultSelectorPropertiesToWatch);

			set.JoinerAdded += OnJoinerAdded;
			set.JoinerRemoved += OnJoinerRemoved;
			set.SetEmptied += OnSetEmptied;

			return set;
		}

		private void OnLeftAdded(int index, KeyValuePair<TKey, TLeft> value)
		{
			if (!_joinerLookup.TryGetValue(value.Key, out var set))
			{
				set = CreateJoinerSet(value.Key);
				_joinerLookup.Add(value.Key, set);
			}

			set.AddLeft(index, value.Value);
		}

		private void OnLeftRemoved(int index, KeyValuePair<TKey, TLeft> value)
			=> _joinerLookup[value.Key].RemoveLeft(index);

		private void OnLeftReplaced(int index, KeyValuePair<TKey, TLeft> oldValue, KeyValuePair<TKey, TLeft> newValue)
		{
			if (Equals(oldValue.Key, newValue.Key))
				_joinerLookup[oldValue.Key].ReplaceLeft(index, newValue.Value);
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

			_leftJoiners.Move(oldIndex, newIndex);

			var min = oldIndex < newIndex ? oldIndex : newIndex;
			var max = oldIndex > newIndex ? oldIndex : newIndex;

			for (int i = min; i <= max; ++i)
				_leftJoiners[i].SourceIndex = i;

			UpdateIndices(min);

			_resultList.MoveRange(startIndex, endIndex, count);
		}

		private void OnLeftReset(IReadOnlyList<KeyValuePair<TKey, TLeft>> newItems)
		{
			var resets = _joinerLookup.Values.Select(set => set.ResetLeft()).ToArray();

			try
			{
				for (int i = 0; i < newItems.Count; ++i)
				{
					var item = newItems[i];

					if (!_joinerLookup.TryGetValue(item.Key, out var set))
					{
						set = CreateJoinerSet(item.Key);
						_joinerLookup.Add(item.Key, set);
					}

					set.AddLeft(i, item.Value);
				}
			}
			finally
			{
				foreach (var reset in resets)
					reset.Dispose();
			}
		}

		private void OnRightAdded(int index, IActiveGrouping<TKey, TRight> value)
		{
			if (!_joinerLookup.TryGetValue(value.Key, out var set))
			{
				set = CreateJoinerSet(value.Key);
				_joinerLookup.Add(value.Key, set);
			}

			set.SetRight(index);
		}

		private void OnRightRemoved(int index, IActiveGrouping<TKey, TRight> value)
		{
			_joinerLookup[value.Key].ClearRight();
		}

		private void OnRightReplaced(int index, IActiveGrouping<TKey, TRight> oldValue, IActiveGrouping<TKey, TRight> newValue)
		{
			OnRightRemoved(index, oldValue);
			OnRightAdded(index, newValue);
		}

		private void OnRightMoved(int oldIndex, int newIndex, IActiveGrouping<TKey, TRight> value)
			=> HandleRightMoved(oldIndex, newIndex);

		private void HandleRightMoved(int oldIndex, int newIndex)
		{
			var startIndex = _rightJoiners[oldIndex].Offset;
			int endIndex;
			if (oldIndex < newIndex)
				endIndex = (_rightJoiners[newIndex].Offset + _rightJoiners[newIndex].Count) - _rightJoiners[oldIndex].Count;
			else
				endIndex = _rightJoiners[newIndex].Offset;
			var count = _rightJoiners[oldIndex].Count;

			_rightJoiners.Move(oldIndex, newIndex);

			var min = oldIndex < newIndex ? oldIndex : newIndex;
			var max = oldIndex > newIndex ? oldIndex : newIndex;

			for (int i = min; i <= max; ++i)
				_rightJoiners[i].SourceIndex = i;

			UpdateIndices(min);

			_resultList.MoveRange(startIndex, endIndex, count);
		}

		private void OnRightReset(IReadOnlyList<IActiveGrouping<TKey, TRight>> newItems)
		{
			var groups = newItems
				.Select((item, index) => Tuple.Create(item.Key, new Func<int>(() => index), true))
				.Concat(_joinerLookup.Where(kvp => kvp.Value.HasRight).Select(kvp => Tuple.Create(kvp.Key, new Func<int>(() => kvp.Value.RightSourceIndex.Value), false)))
				.GroupBy(t => t.Item1).ToArray();

			foreach (var group in groups)
			{
				if (!_joinerLookup.TryGetValue(group.Key, out var set))
				{
					set = CreateJoinerSet(group.Key);
					_joinerLookup.Add(group.Key, set);
				}

				var newItem = group.FirstOrDefault(item => item.Item3);
				var oldItem = group.FirstOrDefault(item => !item.Item3);

				if (newItem != null)
				{
					if (oldItem != null)
						HandleRightMoved(oldItem.Item2.Invoke(), newItem.Item2.Invoke());
					else
						set.SetRight(newItem.Item2.Invoke());
				}
				else if (oldItem != null)
					set.ClearRight();
			}
		}

		private void OnJoinerAdded(ActiveListJoinerData<TLeft, TRight, TResult, TKey> data)
		{
			if (data.IsLeftJoiner)
			{
				_leftJoiners.Add(data.SourceIndex, data);

				for (int i = data.SourceIndex + 1; i < _leftJoiners.Count; ++i)
					_leftJoiners[i].SourceIndex = i;
			}
			else
			{
				_rightJoiners.Add(data.SourceIndex, data);

				for (int i = data.SourceIndex + 1; i < _rightJoiners.Count; ++i)
					_rightJoiners[i].SourceIndex = i;
			}

			UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));

			data.Joiner.AddRequested += (index, value) =>
			{
				++data.Count;

				UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));

				_resultList.Add(data.Offset + index, value);
			};
			data.Joiner.RemoveRequested += index =>
			{
				--data.Count;

				UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));

				_resultList.Remove(data.Offset + index);
			};
			data.Joiner.ReplaceRequested += (index, newValue) =>
			{
				_resultList.Replace(data.Offset + index, newValue);
			};
			data.Joiner.ReplaceRangeRequested += (index, oldCount, values) =>
			{
				var diff = values.Count - oldCount;

				if (diff != 0)
				{
					data.Count += diff;

					UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));
				}

				_resultList.ReplaceRange(data.Offset + index, oldCount, values);
			};
			data.Joiner.MoveRequested += (oldIndex, newIndex) =>
			{
				_resultList.Move(data.Offset + oldIndex, data.Offset + newIndex);
			};
			data.Joiner.MoveRangeRequested += (oldIndex, newIndex, count) =>
			{
				_resultList.MoveRange(data.Offset + oldIndex, data.Offset + newIndex, count);
			};
			data.Joiner.ResetRequested += values =>
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
		}

		private void OnJoinerRemoved(ActiveListJoinerData<TLeft, TRight, TResult, TKey> data)
		{
			if (data.IsLeftJoiner)
			{
				_leftJoiners.Remove(data.SourceIndex);

				for (int i = data.SourceIndex; i < _leftJoiners.Count; ++i)
					_leftJoiners[i].SourceIndex = i;
			}
			else
			{
				_rightJoiners.Remove(data.SourceIndex);

				for (int i = data.SourceIndex; i < _rightJoiners.Count; ++i)
					_rightJoiners[i].SourceIndex = i;
			}

			UpdateIndices(data.GetTargetIndex(_leftJoiners.Count));
		}

		private void OnSetEmptied(ActiveListJoinerSet<TLeft, TRight, TResult, TKey> set)
		{
			_joinerLookup.Remove(set.Key);

			set.JoinerAdded -= OnJoinerAdded;
			set.JoinerRemoved -= OnJoinerRemoved;
			set.SetEmptied -= OnSetEmptied;

			set.Dispose();
		}

		private void UpdateIndices(int startIndex)
		{
			var offset = startIndex == 0 ? 0 : (--startIndex < _leftJoiners.Count ? _leftJoiners[startIndex].Offset : _rightJoiners[startIndex - _leftJoiners.Count].Offset);

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
