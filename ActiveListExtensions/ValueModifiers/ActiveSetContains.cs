using ActiveListExtensions.Utilities;
using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveSetContains<TSet, TSource> : ActiveListValueBase<TSource, TSet, bool>
	{
		private IActiveSet<TSet> _set;

		public ActiveSetContains(IActiveList<TSource> source, IActiveSet<TSet> set, IActiveValue<TSet> value)
			: base(source, value)
		{
			_set = set;

			Value = _set.Contains(ParameterValue);
		}

		protected override void OnAdded(int index, TSource value) => Value = _set.Contains(ParameterValue);

		protected override void OnMoved(int oldIndex, int newIndex, TSource value) => Value = _set.Contains(ParameterValue);

		protected override void OnRemoved(int index, TSource value) => Value = _set.Contains(ParameterValue);

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue) => Value = _set.Contains(ParameterValue);

		protected override void OnReset(IReadOnlyList<TSource> newItems) => Value = _set.Contains(ParameterValue);
	}
}
