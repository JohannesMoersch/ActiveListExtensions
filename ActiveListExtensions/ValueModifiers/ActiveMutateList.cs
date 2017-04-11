using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveMutateList<TSource, TResult> : ActiveListValueBase<TSource, object, TResult>
	{
		private readonly Func<IReadOnlyList<TSource>, TResult> _mutator;

		public ActiveMutateList(IActiveList<TSource> source, Func<IReadOnlyList<TSource>, TResult> mutator) 
			: base(source)
		{
			_mutator = mutator;
		}

		protected override void OnAdded(int index, TSource value) => Value = _mutator.Invoke(SourceList);

		protected override void OnRemoved(int index, TSource value) => Value = _mutator.Invoke(SourceList);

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue) => Value = _mutator.Invoke(SourceList);

		protected override void OnMoved(int oldIndex, int newIndex, TSource value) => Value = _mutator.Invoke(SourceList);

		protected override void OnReset(IReadOnlyList<TSource> newItems) => Value = _mutator.Invoke(SourceList);
	}
}
