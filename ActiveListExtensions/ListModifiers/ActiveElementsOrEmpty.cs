using ActiveListExtensions.ListModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveElementsOrEmpty<TSource> : ActiveMultiListBase<TSource, int, object, TSource>
	{
		public ActiveElementsOrEmpty(IActiveList<TSource> source, IReadOnlyList<int> indexes)
			: base(source, null)
		{
		}

		protected override void OnAdded(int index, TSource value)
		{
			throw new NotImplementedException();
		}

		protected override void OnRemoved(int index, TSource value)
		{
			throw new NotImplementedException();
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			throw new NotImplementedException();
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			throw new NotImplementedException();
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			throw new NotImplementedException();
		}

		protected override void OnAdded(int collectionIndex, int index, int value)
		{
			throw new NotImplementedException();
		}

		protected override void OnRemoved(int collectionIndex, int index, int value)
		{
			throw new NotImplementedException();
		}

		protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, int value)
		{
			throw new NotImplementedException();
		}

		protected override void OnReplaced(int collectionIndex, int index, int oldValue, int newValue)
		{
			throw new NotImplementedException();
		}

		protected override void OnReset(int collectionIndex, IReadOnlyList<int> newItems)
		{
			throw new NotImplementedException();
		}
	}
}
