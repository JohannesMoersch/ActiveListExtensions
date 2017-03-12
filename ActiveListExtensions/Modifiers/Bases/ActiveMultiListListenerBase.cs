using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Modifiers.Bases
{
	internal abstract class ActiveMultiListListenerBase<T, U> : ActiveListListenerBase<T, U>
	{
		private List<CollectionWrapper<U>> _sourceLists = new List<CollectionWrapper<U>>();

		protected IReadOnlyList<IReadOnlyList<U>> SourceLists => _sourceLists;

		private string[] _propertiesToWatch;

		public ActiveMultiListListenerBase(IActiveList<T> source, IEnumerable<string> propertiesToWatch = null) 
			: base(source, propertiesToWatch)
		{
			_propertiesToWatch = propertiesToWatch?.ToArray();
		}

		protected override void OnDisposed()
		{
			foreach (var wrapper in _sourceLists)
				wrapper.Dispose();
			_sourceLists.Clear();
			base.OnDisposed();
		}

		protected void AddSourceCollection(int collectionIndex, IReadOnlyList<U> collection, bool usePropertyWatcher = true)
		{
			if (collectionIndex < 0 || collectionIndex > _sourceLists.Count)
				throw new ArgumentOutOfRangeException(nameof(collectionIndex));

			var wrapper = new CollectionWrapper<U>(collection, usePropertyWatcher ? _propertiesToWatch : null);
			wrapper.ItemModified += (s, i, v) => ItemModified(wrapper.CollectionIndex, i, v);
			wrapper.ItemAdded += (s, i, v) => OnAdded(wrapper.CollectionIndex, i, v);
			wrapper.ItemRemoved += (s, i, v) => OnRemoved(wrapper.CollectionIndex, i, v);
			wrapper.ItemReplaced += (s, i, o, n) => OnReplaced(wrapper.CollectionIndex, i, o, n);
			wrapper.ItemMoved += (s, o, n, v) => OnMoved(wrapper.CollectionIndex, o, n, v);
			wrapper.ItemsReset += s => OnReset(wrapper.CollectionIndex, s);

			_sourceLists.Insert(collectionIndex, wrapper);
			for (int i = collectionIndex; i < _sourceLists.Count; ++i)
				_sourceLists[i].CollectionIndex = i;
			OnCollectionInserted(collectionIndex);
			_sourceLists[collectionIndex].Reset();
		}

		protected void RemoveSourceCollection(int collectionIndex)
		{
			if (collectionIndex < 0 || collectionIndex >= _sourceLists.Count)
				throw new ArgumentOutOfRangeException(nameof(collectionIndex));

			OnReset(collectionIndex, new U[0]);
			var wrapper = _sourceLists[collectionIndex];
			_sourceLists.RemoveAt(collectionIndex);
			for (int i = collectionIndex; i < _sourceLists.Count; ++i)
				_sourceLists[i].CollectionIndex = i;
			OnCollectionRemoved(collectionIndex);
			wrapper.Dispose();
		}

		protected abstract void OnAdded(int collectionIndex, int index, U value);

		protected abstract void OnRemoved(int collectionIndex, int index, U value);

		protected abstract void OnReplaced(int collectionIndex, int index, U oldValue, U newValue);

		protected abstract void OnMoved(int collectionIndex, int oldIndex, int newIndex, U value);

		protected abstract void OnReset(int collectionIndex, IReadOnlyList<U> newItems);

		protected virtual void ItemModified(int collectionIndex, int index, U value) { }

		protected virtual void OnCollectionInserted(int collectionIndex) { }

		protected virtual void OnCollectionRemoved(int collectionIndex) { }
	}
}
