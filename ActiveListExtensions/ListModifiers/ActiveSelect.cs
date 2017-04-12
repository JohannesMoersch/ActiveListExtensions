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
	internal class ActiveSelect<TSource, TParameter, TResult> : ActiveListBase<TSource, TResult, TParameter, TResult>
	{
		private Func<TSource, TResult> _selector;

		public ActiveSelect(IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch)
			: this(source, selector, null, propertiesToWatch, null)
		{
		}

		public ActiveSelect(IActiveList<TSource> source, Func<TSource, TParameter, TResult> selector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, i => selector.Invoke(i, parameter.Value), parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveSelect(IActiveList<TSource> source, Func<TSource, TResult> selector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, i => i, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			_selector = selector ?? throw new ArgumentNullException(nameof(selector));

			Initialize();
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
