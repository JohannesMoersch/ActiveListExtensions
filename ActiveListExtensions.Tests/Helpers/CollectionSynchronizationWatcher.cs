using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Tests.Helpers
{
	public class CollectionSynchronizationWatcher<T>
	{
		private IReadOnlyList<T> _source;

		private IList<T> _internalList = new List<T>();

		public CollectionSynchronizationWatcher(IReadOnlyList<T> source)
		{
			_source = source;

			if (_source is INotifyCollectionChanged)
				(_source as INotifyCollectionChanged).CollectionChanged += SourceCollectionChanged;

			foreach (var item in _source)
				_internalList.Add(item);
		}

		private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				if (e.NewItems.Count > 1)
					throw new Exception("Change notifications can only include a single item.");
				if (!Equals(_source[e.NewStartingIndex], e.NewItems[0]))
					throw new Exception("Change notification item out of sync with source collection.");
			}
			if (e.OldItems != null)
			{
				if (e.OldItems.Count > 1)
					throw new Exception("Change notifications can only include a single item.");
				if (!Equals(_internalList[e.OldStartingIndex], e.OldItems[0]))
					throw new Exception("Change notification item out of sync with source collection.");
			}
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					_internalList.Insert(e.NewStartingIndex, (T)e.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Remove:
					_internalList.RemoveAt(e.OldStartingIndex);
					break;
				case NotifyCollectionChangedAction.Move:
					_internalList.RemoveAt(e.OldStartingIndex);
					_internalList.Insert(e.NewStartingIndex, (T)e.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Replace:
					_internalList[e.NewStartingIndex] = (T)e.NewItems[0];
					break;
				case NotifyCollectionChangedAction.Reset:
					_internalList.Clear();
					foreach (var item in _source)
						_internalList.Add(item);
					break;
			}
			if (_source.Count != _internalList.Count)
				throw new Exception("Source collection count out of sync.");
			if (_source.Zip(_internalList, (o1, o2) => !Equals(o1, o2)).Any(b => b))
				throw new Exception("Source collection enumerator out of sync.");
			for (int i = 0; i < _internalList.Count; ++i)
			{
				if (!Equals(_source[i], _internalList[i]))
					throw new Exception("Source collection values out of sync.");
			}
		}
	}
}
