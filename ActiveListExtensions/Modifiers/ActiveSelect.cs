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
	internal class ActiveSelect<T, U> : ActiveListBase<T, U>
	{
		private Func<T, U> _selector;

		public ActiveSelect(IActiveList<T> source, Func<T, U> selector, IEnumerable<string> propertiesToWatch)
			: base(source, propertiesToWatch)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (selector == null)
				throw new ArgumentNullException(nameof(selector));
			_selector = selector;
			Initialize();
		}

		protected override void OnDisposed()
		{
			_selector = null;
		}

		protected override void OnAdded(int index, T value) => ResultList.Add(index, _selector.Invoke(value));

		protected override void OnRemoved(int index, T value) => ResultList.Remove(index);

		protected override void OnReplaced(int index, T oldValue, T newValue) => ResultList.Replace(index, _selector.Invoke(newValue));

		protected override void OnMoved(int oldIndex, int newIndex, T value) => ResultList.Move(oldIndex, newIndex);

		protected override void OnReset(IReadOnlyList<T> newItems) => ResultList.Reset(newItems.Select(v => _selector.Invoke(v)));

		protected override void ItemModified(int index, T value)
		{
			var newValue = _selector.Invoke(value);
			if (!Equals(ResultList[index], newValue))
				ResultList.Replace(index, newValue);
		}
	}
}
