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
	internal class ActiveLookup<TSource, TKey> : ActiveListListenerBase<TSource, IActiveGrouping<TKey, TSource>>, IActiveLookup<TKey, TSource>
	{
		private class ItemData
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
		}

		private class GroupData
		{
			public int TargetIndex { get; set; }

			public Group<TKey, ItemData> Items { get; }

			public GroupData(TKey key) => Items = new Group<TKey, ItemData>(key);
		}

		private readonly IList<ItemData> _sourceData;

		private readonly ObservableList<GroupData> _resultData;

		private readonly IDictionary<TKey, GroupData> _resultSet;

		private readonly Func<TSource, TKey> _keySelector;

		public override int Count => _resultData.Count;

		public override IActiveGrouping<TKey, TSource> this[int index] => _resultData[index].Items;

		public IEnumerable<TSource> this[TKey key] => throw new NotImplementedException();

		public ActiveLookup(IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			: base(source, propertiesToWatch)
		{
			_sourceData = new List<ItemData>();
			_resultData = new ObservableList<GroupData>();
			_resultSet = new Dictionary<TKey, GroupData>();

			_keySelector = keySelector;

			_resultData.CollectionChanged += (s, e) => NotifyOfCollectionChange(RewrapEventArgs(e));
			_resultData.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);
		}

		private NotifyCollectionChangedEventArgs RewrapEventArgs(NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ((GroupData)args.NewItems[0]).Items, args.NewStartingIndex);
				case NotifyCollectionChangedAction.Remove:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ((GroupData)args.OldItems[0]).Items, args.OldStartingIndex);
				case NotifyCollectionChangedAction.Replace:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, ((GroupData)args.NewItems[0]).Items, ((GroupData)args.OldItems[0]).Items, args.NewStartingIndex);
				case NotifyCollectionChangedAction.Move:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, ((GroupData)args.NewItems[0]).Items, args.NewStartingIndex, args.OldStartingIndex);
				case NotifyCollectionChangedAction.Reset:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			}
			return null;
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
			OnRemoved(index, oldValue);
			OnAdded(index, newValue);
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

			if (_resultSet.TryGetValue(item.Key, out GroupData group))
			{
				var oldTargetIndex = item.TargetIndex;
				item.TargetIndex = FindTargetIndex(group.Items, item.SourceIndex);
				group.Items.Move(oldTargetIndex, item.TargetIndex);
			}
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			throw new NotImplementedException();
		}

		protected override void ItemModified(int index, TSource value)
		{
			throw new NotImplementedException();
		}

		private void AddToGroup(ItemData item)
		{
			if (!_resultSet.TryGetValue(item.Key, out GroupData group))
			{
				group = new GroupData(item.Key);

				_resultSet.Add(item.Key, group);

				group.TargetIndex = _resultData.Count;
				_resultData.Add(group.TargetIndex, group);
			}

			item.TargetIndex = FindTargetIndex(group.Items, item.SourceIndex);
			group.Items.Add(item.TargetIndex, item);
		}

		private void RemoveFromGroup(ItemData item)
		{
			if (_resultSet.TryGetValue(item.Key, out GroupData group))
			{
				group.Items.Remove(item.TargetIndex);
				item.TargetIndex = -1;

				if (group.Items.Count == 0)
				{
					_resultData.Remove(group.TargetIndex);
					group.TargetIndex = -1;

					_resultSet.Remove(item.Key);
				}
			}
		}

		private int FindTargetIndex(IReadOnlyList<ItemData> list, int sourceIndex)
		{
			var bottom = 0;
			var top = list.Count - 1;
			while (bottom <= top)
			{
				var mid = bottom + (top - bottom) / 2;
				var midItem = list[mid];
				var comparison = sourceIndex.CompareTo(midItem.SourceIndex);
				if (comparison > 0)
					bottom = mid + 1;
				else
					top = mid - 1;
			}
			return bottom;
		}

		public override IEnumerator<IActiveGrouping<TKey, TSource>> GetEnumerator() => _resultData.Select(kvp => kvp.Itemsgit a).GetEnumerator();
	}
}