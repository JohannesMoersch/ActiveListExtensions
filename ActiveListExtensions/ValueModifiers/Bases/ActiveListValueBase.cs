using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers.Bases
{
	internal abstract class ActiveListValueBase<TSource, TValue> : ActiveValueBase<TValue>
	{
		private CollectionWrapper<TSource> _sourceList;

		protected IReadOnlyList<TSource> SourceList => _sourceList;

		public ActiveListValueBase(IActiveList<TSource> source, IEnumerable<string> propertiesToWatch = null)
		{
			_sourceList = new CollectionWrapper<TSource>(source, propertiesToWatch?.ToArray());
			_sourceList.ItemModified += (s, i, v) => ItemModified(i, v);
			_sourceList.ItemAdded += (s, i, v) => OnAdded(i, v);
			_sourceList.ItemRemoved += (s, i, v) => OnRemoved(i, v);
			_sourceList.ItemReplaced += (s, i, o, n) => OnReplaced(i, o, n);
			_sourceList.ItemMoved += (s, o, n, v) => OnMoved(o, n, v);
			_sourceList.ItemsReset += s => OnReset(s);
		}

		protected void Initialize() => _sourceList.Reset();

		public override void Dispose() => _sourceList.Dispose();

		protected abstract void OnAdded(int index, TSource value);

		protected abstract void OnRemoved(int index, TSource value);

		protected abstract void OnReplaced(int index, TSource oldValue, TSource newValue);

		protected abstract void OnMoved(int oldIndex, int newIndex, TSource value);

		protected abstract void OnReset(IReadOnlyList<TSource> newItems);

		protected virtual void ItemModified(int index, TSource value) { }
	}
}