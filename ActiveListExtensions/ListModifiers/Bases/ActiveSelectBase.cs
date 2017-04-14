using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveSelectBase<TSource, TStore, TParameter, TResult> : ActiveListBase<TSource, TStore, TParameter, TResult>
	{
		private Func<TSource, TStore> _selector;

		public ActiveSelectBase(IActiveList<TSource> source, Func<TSource, TStore> selector, Func<TStore, TResult> resultSelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, resultSelector, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			_selector = selector ?? throw new ArgumentNullException(nameof(selector));
		}

		protected override void OnDisposed()
		{
			_selector = null;
		}

		protected override void OnAdded(int index, TSource value) => ResultList.Add(index, _selector.Invoke(value));

		protected override void OnRemoved(int index, TSource value) => ResultList.Remove(index);

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue) => ResultList.Replace(index, _selector.Invoke(newValue));

		protected override void OnMoved(int oldIndex, int newIndex, TSource value) => ResultList.Move(oldIndex, newIndex);

		protected override void OnReset(IReadOnlyList<TSource> newItems) => ResultList.Reset(newItems.Select(v => _selector.Invoke(v)));

		protected override void ItemModified(int index, TSource value)
		{
			var newValue = _selector.Invoke(value);
			if (!Equals(ResultList[index], newValue))
				ResultList.Replace(index, newValue);
		}
	}
}
