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

			_otherSourceList = new CollectionWrapper<TSource>(otherSource);
			_otherSourceList.ItemModified += (s, i, v) => OtherItemModified(i, v);
			_otherSourceList.ItemAdded += (s, i, v) => OnOtherAdded(i, v);
			_otherSourceList.ItemRemoved += (s, i, v) => OnOtherRemoved(i, v);
			_otherSourceList.ItemReplaced += (s, i, o, n) => OnOtherReplaced(i, o, n);
			_otherSourceList.ItemMoved += (s, o, n, v) => OnOtherMoved(o, n, v);
			_otherSourceList.ItemsReset += s => OnOtherReset(s);

			Initialize();
		}

		private void Recalculate()
		{
			if (SourceList.Count != _otherSourceList.Count)
				Value = false;
			else
				Value = SourceList.SequenceEqual(_otherSourceList, _equalityComparer);
		}

		private bool Reevaluate(int index) => _equalityComparer.Equals(SourceList[index], _otherSourceList[index]);

		protected override void OnAdded(int index, TSource value) => Recalculate();

		protected override void OnRemoved(int index, TSource value) => Recalculate();

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems) => Recalculate();

		protected override void ItemModified(int index, TSource value)
		{
		}

		private void OnOtherAdded(int index, TSource value) => Recalculate();

		private void OnOtherRemoved(int index, TSource value) => Recalculate();

		private void OnOtherMoved(int oldIndex, int newIndex, TSource value)
		{
		}

		private void OnOtherReplaced(int index, TSource oldValue, TSource newValue)
		{
		}

		private void OnOtherReset(IReadOnlyList<TSource> newItems) => Recalculate();

		private void OtherItemModified(int index, TSource value)
		{
		}
	}
}
