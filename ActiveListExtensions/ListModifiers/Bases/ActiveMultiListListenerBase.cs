﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.ListModifiers.Bases
{
	internal abstract class ActiveMultiListListenerBase<TSource, TOtherSources, TParameter, TResult> : ActiveListListenerBase<TSource, TParameter, TResult>
	{
		private QuickList<CollectionWrapper<TOtherSources>> _sourceLists = new QuickList<CollectionWrapper<TOtherSources>>();

		protected IReadOnlyList<IReadOnlyList<TOtherSources>> SourceLists => _sourceLists;

		private string[] _otherSourcePropertiesToWatch;

		public ActiveMultiListListenerBase(IActiveList<TSource> source, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch = null, IEnumerable<string> otherSourcePropertiesToWatch = null, IEnumerable<string> parameterPropertiesToWatch = null) 
			: base(source, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_otherSourcePropertiesToWatch = otherSourcePropertiesToWatch?.ToArray();
		}

		protected override void OnDisposed()
		{
			foreach (var wrapper in _sourceLists)
				wrapper.Dispose();
			_sourceLists.Clear();
			base.OnDisposed();
		}

		protected void AddSourceCollection(int collectionIndex, IReadOnlyList<TOtherSources> collection, bool usePropertyWatcher = true)
		{
			if (collectionIndex < 0 || collectionIndex > _sourceLists.Count)
				throw new ArgumentOutOfRangeException(nameof(collectionIndex));

			var wrapper = new CollectionWrapper<TOtherSources>(collection, usePropertyWatcher ? _otherSourcePropertiesToWatch : null);
			wrapper.ItemModified += (s, i, v) => ItemModified(wrapper.CollectionIndex, i, v);
			wrapper.ItemAdded += (s, i, v) => OnAdded(wrapper.CollectionIndex, i, v);
			wrapper.ItemRemoved += (s, i, v) => OnRemoved(wrapper.CollectionIndex, i, v);
			wrapper.ItemReplaced += (s, i, o, n) => OnReplaced(wrapper.CollectionIndex, i, o, n);
			wrapper.ItemMoved += (s, o, n, v) => OnMoved(wrapper.CollectionIndex, o, n, v);
			wrapper.ItemsReset += s => OnReset(wrapper.CollectionIndex, s);

			_sourceLists.Add(collectionIndex, wrapper);
			for (int i = collectionIndex; i < _sourceLists.Count; ++i)
				_sourceLists[i].CollectionIndex = i;
			OnCollectionInserted(collectionIndex);
			_sourceLists[collectionIndex].Reset();
		}

		protected void RemoveSourceCollection(int collectionIndex)
		{
			if (collectionIndex < 0 || collectionIndex >= _sourceLists.Count)
				throw new ArgumentOutOfRangeException(nameof(collectionIndex));

			var wrapper = _sourceLists[collectionIndex];
			_sourceLists.Remove(collectionIndex);
			for (int i = collectionIndex; i < _sourceLists.Count; ++i)
				_sourceLists[i].CollectionIndex = i;
			wrapper.Dispose();
			OnCollectionRemoved(collectionIndex);
			OnReset(collectionIndex, new TOtherSources[0]);
		}

		protected void MoveSourceCollection(int oldCollectionIndex, int newCollectionIndex)
		{
			if (oldCollectionIndex < 0 || oldCollectionIndex >= _sourceLists.Count)
				throw new ArgumentOutOfRangeException(nameof(oldCollectionIndex));

			if (newCollectionIndex < 0 || newCollectionIndex >= _sourceLists.Count)
				throw new ArgumentOutOfRangeException(nameof(newCollectionIndex));

			_sourceLists.Move(oldCollectionIndex, newCollectionIndex);

			var min = oldCollectionIndex < newCollectionIndex ? oldCollectionIndex : newCollectionIndex;
			var max = oldCollectionIndex > newCollectionIndex ? oldCollectionIndex : newCollectionIndex;

			for (int i = min; i <= max; ++i)
				_sourceLists[i].CollectionIndex = i;
		}

		protected abstract void OnAdded(int collectionIndex, int index, TOtherSources value);

		protected abstract void OnRemoved(int collectionIndex, int index, TOtherSources value);

		protected abstract void OnReplaced(int collectionIndex, int index, TOtherSources oldValue, TOtherSources newValue);

		protected abstract void OnMoved(int collectionIndex, int oldIndex, int newIndex, TOtherSources value);

		protected abstract void OnReset(int collectionIndex, IReadOnlyList<TOtherSources> newItems);

		protected virtual void ItemModified(int collectionIndex, int index, TOtherSources value) { }

		protected virtual void OnCollectionInserted(int collectionIndex) { }

		protected virtual void OnCollectionRemoved(int collectionIndex) { }
	}
}
