using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveLastOrDefault<TSource> : ActiveListValueBase<TSource, TSource>
	{
		public ActiveLastOrDefault(IActiveList<TSource> source) 
			: base(source)
		{
		}

		protected override void OnAdded(int index, TSource value)
		{
			if (index == SourceList.Count - 1)
				Value = value;
		}

		protected override void OnRemoved(int index, TSource value)
		{
			if (index == SourceList.Count)
				Value = SourceList.LastOrDefault();
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (oldIndex == SourceList.Count - 1)
				Value = SourceList.LastOrDefault();
			else if (newIndex == SourceList.Count - 1)
				Value = value;
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (index == SourceList.Count - 1)
				Value = newValue;
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems) => Value = newItems.LastOrDefault();
	}
}