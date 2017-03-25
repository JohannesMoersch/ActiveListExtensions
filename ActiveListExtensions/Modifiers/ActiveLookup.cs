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

		public override int Count => _resultData.Count;

		public override IActiveGrouping<TKey, TSource> this[int index] => throw new NotImplementedException();

		public IEnumerable<TSource> this[TKey key] => throw new NotImplementedException();

		public ActiveLookup(IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			: base(source, propertiesToWatch)
		{
			_sourceData = new List<ItemData>();
			_resultData = new ObservableList<GroupData>();
			_resultSet = new Dictionary<TKey, GroupData>();

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
			throw new NotImplementedException();
		}

		protected override void OnRemoved(int index, TSource value)
		{
			throw new NotImplementedException();
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			throw new NotImplementedException();
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			throw new NotImplementedException();
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			throw new NotImplementedException();
		}

		public override IEnumerator<IActiveGrouping<TKey, TSource>> GetEnumerator() => _resultData.Select(kvp => kvp.Items as IActiveGrouping<TKey, TSource>).GetEnumerator();
	}
}