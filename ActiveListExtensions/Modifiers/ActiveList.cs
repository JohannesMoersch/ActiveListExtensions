using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveList<T> : ActiveBase<T, T>
	{
		public override int Count => _collection.Count;

		public override T this[int index] =>  _collection[index];

		private CollectionWrapper<T> _collection;

		public ActiveList(IReadOnlyList<T> source)
		{
			_collection = new CollectionWrapper<T>(source);
			_collection.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
		}

		protected override void OnDisposed() => _collection.Dispose();

		public override IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();
	}
}
