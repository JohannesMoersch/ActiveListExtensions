using ActiveListExtensions.Modifiers.Bases;
using ActiveListExtensions.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveLookup<TSource, TKey> : ActiveListBase<TSource, ActiveLookup<TSource, TKey>.GroupData, IActiveGrouping<TKey, TSource>>, IActiveLookup<TKey, TSource>
	{
		internal class Group : ObservableList<ItemData, TSource>, IActiveGrouping<TKey, TSource>
		{
			public TKey Key { get; }

			public Group(TKey key) : base(i => i.Value) => Key = key;
		}

		internal class ItemData
		{
			public int SourceIndex { get; set; }

			public int TargetIndex { get; set; }

			public TKey Key { get; }

			public TSource Value { get; }

			public ItemData(TKey key, TSource value)
			{
				Key = key;
				Value = value;
			}

			public override string ToString() => $"Source:{SourceIndex}|Target:{TargetIndex}|Value:{Value}";
		}

		internal class GroupData
		{
			public int TargetIndex { get; set; }

			public Group Items { get; }

			public GroupData(TKey key) => Items = new Group(key);
		}

		private readonly IList<ItemData> _sourceData;

		private readonly IDictionary<TKey, GroupData> _resultSet;

		private readonly Func<TSource, TKey> _keySelector;

		public IEnumerable<TSource> this[TKey key] => _resultSet[key].Items;

		public ActiveLookup(IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			: base(source, i => i.Items, propertiesToWatch)
		{
			_sourceData = new List<ItemData>();
			_resultSet = new Dictionary<TKey, GroupData>();

			_keySelector = keySelector;

			Initialize();
		}

		public bool Contains(TKey key) => _resultSet.ContainsKey(key);

		protected override void OnAdded(int index, TSource value)
		{
			var item = new ItemData(_keySelector.Invoke(value), value);
			item.SourceIndex = index;
			_sourceData.Insert(index, item);

			for (int i = index + 1; i < _sourceData.Count; ++i)
				_sourceData[i].SourceIndex = i;

			AddToGroup(item);
		}

		protected override void OnRemoved(int index, TSource value)
		{
			var item = _sourceData[index];
			item.SourceIndex = -1;
			_sourceData.RemoveAt(index);

			for (int i = index; i < _sourceData.Count; ++i)
				_sourceData[i].SourceIndex = i;

			RemoveFromGroup(item);
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			var oldItem = _sourceData[index];
			var newKey = _keySelector.Invoke(newValue);
			if (Equals(oldItem.Key, newKey))
			{
				var newItem = new ItemData(newKey, newValue);
				newItem.TargetIndex = oldItem.TargetIndex;
				_resultSet[oldItem.Key].Items.Replace(oldItem.TargetIndex, newItem);

				_sourceData[index] = newItem;
			}
			else
			{
				OnRemoved(index, oldValue);
				OnAdded(index, newValue);
			}
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			var item = _sourceData[oldIndex];
			_sourceData.RemoveAt(oldIndex);
			_sourceData.Insert(newIndex, item);

			var min = oldIndex < newIndex ? oldIndex : newIndex;
			var max = oldIndex < newIndex ? newIndex : oldIndex;

			for (int i = min; i <= max; ++i)
				_sourceData[i].SourceIndex = i;

			item.SourceIndex = oldIndex;

			if (_resultSet.TryGetValue(item.Key, out GroupData group))
			{
				var oldTargetIndex = item.TargetIndex;
				item.TargetIndex = FindTargetIndex(group.Items, newIndex);
				if (item.TargetIndex > oldTargetIndex)
					--item.TargetIndex;

				item.SourceIndex = newIndex;

				group.Items.Move(oldTargetIndex, item.TargetIndex);

				min = oldTargetIndex < item.TargetIndex ? oldTargetIndex : item.TargetIndex;
				max = oldTargetIndex < item.TargetIndex ? item.TargetIndex : oldTargetIndex;

				for (int i = min; i <= max; ++i)
					group.Items[i].TargetIndex = i;
			}
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			_sourceData.Clear();
			_resultSet.Clear();

			foreach (var value in newItems)
			{
				var item = new ItemData(_keySelector.Invoke(value), value);
				item.SourceIndex = _sourceData.Count;
				_sourceData.Add(item);
				AddToGroup(item, false);
			}

			ResultList.Reset(_resultSet.Values.Select((g, i) =>
			{
				g.TargetIndex = i;
				return g;
			}));
		}

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);

		private void AddToGroup(ItemData item, bool addToResultList = true)
		{
			if (!_resultSet.TryGetValue(item.Key, out GroupData group))
			{
				group = new GroupData(item.Key);

				_resultSet.Add(item.Key, group);

				if (addToResultList)
				{
					group.TargetIndex = Count;
					ResultList.Add(group.TargetIndex, group);

					for (int i = group.TargetIndex + 1; i < ResultList.Count; ++i)
						ResultList[i].TargetIndex = i;
				}
			}

			item.TargetIndex = FindTargetIndex(group.Items, item.SourceIndex);
			group.Items.Add(item.TargetIndex, item);

			for (int i = item.TargetIndex + 1; i < group.Items.Count; ++i)
				group.Items[i].TargetIndex = i;
		}

		private void RemoveFromGroup(ItemData item)
		{
			if (_resultSet.TryGetValue(item.Key, out GroupData group))
			{
				group.Items.Remove(item.TargetIndex);

				for (int i = item.TargetIndex; i < group.Items.Count; ++i)
					group.Items[i].TargetIndex = i;

				item.TargetIndex = -1;

				if (group.Items.Count == 0)
				{
					ResultList.Remove(group.TargetIndex);

					for (int i = group.TargetIndex; i < ResultList.Count; ++i)
						ResultList[i].TargetIndex = i;

					group.TargetIndex = -1;

					_resultSet.Remove(item.Key);
				}
			}
		}

		private int FindTargetIndex(Group group, int sourceIndex)
		{
			var bottom = 0;
			var top = group.Count - 1;
			while (bottom <= top)
			{
				var mid = bottom + (top - bottom) / 2;
				var midItem = group[mid];
				var comparison = sourceIndex.CompareTo(midItem.SourceIndex);
				if (comparison > 0)
					bottom = mid + 1;
				else
					top = mid - 1;
			}
			return bottom;
		}
	}
}