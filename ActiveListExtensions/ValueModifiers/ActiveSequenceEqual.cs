using ActiveListExtensions.Utilities;
using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveSequenceEqual<TSource, TParameter> : ActiveListValueBase<TSource, TParameter, bool>
	{
		private class FunctionEqualityComparer : IEqualityComparer<TSource>
		{
			private Func<TSource, TSource, bool> _comparer;

			public FunctionEqualityComparer(Func<TSource, TSource, bool> comparer) => _comparer = comparer;

			public bool Equals(TSource x, TSource y) => _comparer.Invoke(x, y);

			public int GetHashCode(TSource obj) => 0;
		}

		private IEqualityComparer<TSource> _equalityComparer;

		private CollectionWrapper<TSource> _otherSourceList;

		public ActiveSequenceEqual(IActiveList<TSource> source, IReadOnlyList<TSource> otherSource, Func<TSource, TSource, bool> comparer, IEnumerable<string> propertiesToWatch)
			: this(source, otherSource, null, comparer, propertiesToWatch, null)
		{
		}

		public ActiveSequenceEqual(IActiveList<TSource> source, IReadOnlyList<TSource> otherSource, IActiveValue<TParameter> parameter, Func<TSource, TSource, TParameter, bool> comparer, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, otherSource, parameter, (o1, o2) => comparer.Invoke(o1, o2, parameter.Value), sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveSequenceEqual(IActiveList<TSource> source, IReadOnlyList<TSource> otherSource, IActiveValue<TParameter> parameter, Func<TSource, TSource, bool> comparer, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_equalityComparer = new FunctionEqualityComparer(comparer);

			_otherSourceList = new CollectionWrapper<TSource>(otherSource, sourcePropertiesToWatch.ToArray());
			_otherSourceList.ItemModified += (s, i, v) => ItemModified(i, v);
			_otherSourceList.ItemAdded += (s, i, v) => OnAdded(i, v);
			_otherSourceList.ItemRemoved += (s, i, v) => OnRemoved(i, v);
			_otherSourceList.ItemReplaced += (s, i, o, n) => OnReplaced(i, o, n);
			_otherSourceList.ItemMoved += (s, o, n, v) => OnMoved(o, n, v);
			_otherSourceList.ItemsReset += s => OnReset(s);

			Initialize();
		}

		private void Recalculate()
		{
			if (SourceList.Count != _otherSourceList.Count)
				Value = false;
			else
				Value = SourceList.SequenceEqual(_otherSourceList, _equalityComparer);
		}

		protected override void OnParameterChanged() => Recalculate();

		private bool Evaluate(int index) => _equalityComparer.Equals(SourceList[index], _otherSourceList[index]);

		protected override void OnAdded(int index, TSource value) => Recalculate();

		protected override void OnRemoved(int index, TSource value) => Recalculate();

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (Value)
				Value = Evaluate(oldIndex) && Evaluate(newIndex);
			else
				Recalculate();
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (Value)
				Value = Evaluate(index);
			else
				Recalculate();
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems) => Recalculate();

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);
	}
}
