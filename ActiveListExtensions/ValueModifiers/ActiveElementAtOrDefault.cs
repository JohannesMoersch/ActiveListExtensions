using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveElementAtOrDefault<TSource> : ActiveListValueBase<TSource, TSource>
	{
		private readonly IActiveValue<int> _index;

		public ActiveElementAtOrDefault(IActiveList<TSource> source, IActiveValue<int> index) 
			: base(source)
		{
			_index = index;

			PropertyChangedEventManager.AddHandler(_index, IndexChanged, nameof(IActiveValue<TSource>.Value));

			Initialize();
		}

		protected override void OnDisposed()
		{
			base.OnDisposed();

			PropertyChangedEventManager.RemoveHandler(_index, IndexChanged, nameof(IActiveValue<TSource>.Value));
		}

		private void IndexChanged(object key, PropertyChangedEventArgs args) => UpdateValue();

		private void UpdateValue() => Value = _index.Value >= 0 && _index.Value < SourceList.Count ? SourceList[_index.Value] : default(TSource);

		protected override void OnAdded(int index, TSource value)
		{
			if (index <= _index.Value)
				UpdateValue();
		}

		protected override void OnRemoved(int index, TSource value)
		{
			if (index <= _index.Value)
				UpdateValue();
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (oldIndex < _index.Value)
			{
				if (newIndex >= _index.Value)
					UpdateValue();
			}
			else if (oldIndex == _index.Value)
				UpdateValue();
			else if (newIndex <= _index.Value)
				UpdateValue();
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (index == _index.Value)
				UpdateValue();
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems) => UpdateValue();
	}
}
