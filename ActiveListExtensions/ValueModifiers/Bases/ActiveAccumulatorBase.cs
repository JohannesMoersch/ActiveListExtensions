using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers.Bases
{
	internal abstract class ActiveAccumulatorBase<TSource, TParameter, TValue, TResult> : ActiveListValueBase<TSource, TParameter, TResult>
	{
		private IList<TValue> _values;

		private Func<TSource, TValue> _selector;

		private Func<TValue, TValue, TValue> _adder;

		private Func<TValue, TValue, TValue> _subtractor;

		private TValue _sum;
		private TValue Sum
		{
			get => _sum;
			set
			{
				_sum = value;
				Value = GetValueFromSum(_sum);
			}
		}

		public ActiveAccumulatorBase(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TValue> selector, Func<TValue, TValue, TValue> adder, Func<TValue, TValue, TValue> subtractor, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_values = new List<TValue>();

			_selector = selector;

			_adder = adder;

			_subtractor = subtractor;
		}

		protected abstract TResult GetValueFromSum(TValue sum);

		protected override void OnAdded(int index, TSource value)
		{
			var addedValue = _selector.Invoke(value);

			_values.Insert(index, addedValue);

			Sum = _adder.Invoke(Sum, addedValue);
		}

		protected override void OnRemoved(int index, TSource value)
		{
			var removedValue = _values[index];

			_values.RemoveAt(index);

			Sum = _subtractor.Invoke(Sum, removedValue);
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			var movedValue = _values[oldIndex];
			_values.RemoveAt(oldIndex);
			_values.Insert(newIndex, movedValue);
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			var removedValue = _values[index];

			var addedValue = _selector.Invoke(newValue);

			_values[index] = addedValue;

			Sum = _adder.Invoke(_subtractor.Invoke(Sum, removedValue), addedValue);
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			_values.Clear();

			Sum = default(TValue);

			foreach (var value in newItems)
			{
				var addedValue = _selector.Invoke(value);

				_values.Add(addedValue);

				Sum = _adder.Invoke(Sum, addedValue);
			}
		}

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);
	}
}