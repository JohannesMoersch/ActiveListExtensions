using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveListAsActiveValue<TSource> : ActiveListValueBase<TSource, object, IReadOnlyList<TSource>>
	{
		public ActiveListAsActiveValue(IActiveList<TSource> source) 
			: base(source, null, true)
		{
			Initialize();
		}

		protected override void OnAdded(int index, TSource value) => Value = SourceList;

		protected override void OnRemoved(int index, TSource value) => Value = SourceList;

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue) => Value = SourceList;

		protected override void OnMoved(int oldIndex, int newIndex, TSource value) => Value = SourceList;

		protected override void OnReset(IReadOnlyList<TSource> newItems) => Value = SourceList;
	}
}
