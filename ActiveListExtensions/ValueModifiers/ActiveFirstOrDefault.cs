using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveFirstOrDefault<TSource> : ActiveListValueBase<TSource, TSource>
	{
		public ActiveFirstOrDefault(IActiveList<TSource> source) 
			: base(source)
		{
		}

		protected override void OnAdded(int index, TSource value)
		{
			if (index == 0)
				Value = value;
		}

		protected override void OnRemoved(int index, TSource value)
		{
			if (index == 0)
				Value = SourceList.FirstOrDefault();
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (newIndex == 0)
				Value = value;
			else if (oldIndex == 0)
				Value = SourceList.FirstOrDefault();
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (index == 0)
				Value = newValue;
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems) => Value = SourceList.FirstOrDefault();
	}
}
