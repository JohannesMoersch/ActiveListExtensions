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
	internal class ActiveList<TSource> : ActiveBase<TSource, TSource>
	{
		public override int Count => _collection.Count;

		public override TSource this[int index] =>  _collection[index];

		private CollectionWrapper<TSource> _collection;

		public ActiveList(IReadOnlyList<TSource> source)
		{
			_collection = new CollectionWrapper<TSource>(source);
			_collection.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
		}

		protected override void OnDisposed() => _collection.Dispose();

		public override IEnumerator<TSource> GetEnumerator() => _collection.GetEnumerator();
	}
}
