using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;
using System.ComponentModel;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveList<T> : ActiveBase<T>
	{
		public override int Count => _collection.Count;

		public override T this[int index] =>  _collection[index];

		private IActiveValue<IReadOnlyList<T>> _source;

		private CollectionWrapper<T> _collection;

		public ActiveList(IActiveValue<IReadOnlyList<T>> source)
		{
			_source = source;

			_collection = new CollectionWrapper<T>(_source.Value);
			_collection.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);

			PropertyChangedEventManager.AddHandler(source, SourceChanged, nameof(IActiveValue<IReadOnlyList<T>>.Value));
		}

		private void SourceChanged(object key, PropertyChangedEventArgs args)
		{
			if (!IsDisposed)
				_collection.ReplaceCollection(_source.Value);
		}

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(_source, SourceChanged, nameof(IActiveValue<IReadOnlyList<T>>.Value));
			_collection.Dispose();
		}

		public override IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();
	}
}
