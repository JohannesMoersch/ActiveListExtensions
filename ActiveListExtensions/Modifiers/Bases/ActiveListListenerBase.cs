using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Modifiers.Bases
{
	internal abstract class ActiveListListenerBase<T, U> : ActiveBase<T, U>
	{
		private CollectionWrapper<T> _sourceList;

		protected IReadOnlyList<T> SourceList => _sourceList;

		public ActiveListListenerBase(IActiveList<T> source, IEnumerable<string> propertiesToWatch = null)
		{
			_sourceList = new CollectionWrapper<T>(source, propertiesToWatch?.ToArray());
			_sourceList.ItemModified += (s, i, v) => ItemModified(i, v);
			_sourceList.ItemAdded += (s, i, v) => OnAdded(i, v);
			_sourceList.ItemRemoved += (s, i, v) => OnRemoved(i, v);
			_sourceList.ItemReplaced += (s, i, o, n) => OnReplaced(i, o, n);
			_sourceList.ItemMoved += (s, o, n, v) => OnMoved(o, n, v);
			_sourceList.ItemsReset += s => OnReset(s);
		}

		protected void Initialize() => _sourceList.Reset();

		protected override void OnDisposed()
		{
			_sourceList.Dispose();
		}

		protected abstract void OnAdded(int index, T value);

		protected abstract void OnRemoved(int index, T value);

		protected abstract void OnReplaced(int index, T oldValue, T newValue);

		protected abstract void OnMoved(int oldIndex, int newIndex, T value);

		protected abstract void OnReset(IReadOnlyList<T> newItems);

		protected virtual void ItemModified(int index, T value) { }
	}
}
