using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveSingleOrDefault<TSource> : ActiveListValueBase<TSource, TSource>
	{
		private Func<TSource, bool> _predicate;

		public ActiveSingleOrDefault(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			: base(source, propertiesToWatch)
		{
			_predicate = predicate;

			Initialize();
		}

		protected override void OnAdded(int index, TSource value)
		{
		}

		protected override void OnRemoved(int index, TSource value)
		{
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
		}
	}
}
