using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveGetOrDefault<TKey, TSource> : ActiveListValueBase<IActiveGrouping<TKey, TSource>, TKey, IEnumerable<TSource>>
	{
		private IActiveLookup<TKey, TSource> _source;

		public ActiveGetOrDefault(IActiveLookup<TKey, TSource> source, IActiveValue<TKey> parameter) 
			: base(source, parameter)
		{
			_source = source;

			Initialize();

			OnParameterChanged();
		}

		protected override void OnParameterChanged() => Value = _source[ParameterValue];

		protected override void OnAdded(int index, IActiveGrouping<TKey, TSource> value) { }

		protected override void OnMoved(int oldIndex, int newIndex, IActiveGrouping<TKey, TSource> value) { }

		protected override void OnRemoved(int index, IActiveGrouping<TKey, TSource> value) { }

		protected override void OnReplaced(int index, IActiveGrouping<TKey, TSource> oldValue, IActiveGrouping<TKey, TSource> newValue) { }

		protected override void OnReset(IReadOnlyList<IActiveGrouping<TKey, TSource>> newItems) { }
	}
}
