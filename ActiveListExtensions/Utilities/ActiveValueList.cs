using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveValueList<T> : ObservableList<IActiveValue<T>, ActiveValueList<T>.ValueStore, T>
	{
		internal class ValueStore : IDisposable
		{
			private IActiveValue<T> _value;
			public T Value => _value.Value;

			public int Index { get; set; }

			public ValueStore(IActiveValue<T> value)
			{
				_value = value;
				_value.ValueChanged += ValueChangedHandler;
			}

			private void ValueChangedHandler(IActiveValue<T> activeValue, IActiveValueChangedEventArgs<T> eventArgs) => ValueChanged?.Invoke(eventArgs.OldValue, eventArgs.NewValue, Index);

			public void Dispose() => _value.ValueChanged -= ValueChangedHandler;

			public event Action<T, T, int> ValueChanged;
		}

		public ActiveValueList() 
			: base(i => i.Value)
		{
		}

		protected override ValueStore GetStoreFromSource(IActiveValue<T> source)
		{
			var store = new ValueStore(source);
			store.ValueChanged += ValueChanged;
			return store;
		}

		private void ValueChanged(T oldValue, T newValue, int index) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index));

		protected override void DisposeOfStore(ValueStore store) => store.Dispose();

		public override void Add(int index, IActiveValue<T> value)
		{
			base.Add(index, value);
			for (int i = index; i < List.Count; ++i)
				List[i].Index = i;
		}

		public override void Remove(int index)
		{
			base.Remove(index);
			for (int i = index; i < List.Count; ++i)
				List[i].Index = i;
		}

		public override void Move(int oldIndex, int newIndex)
		{
			base.Move(oldIndex, newIndex);
			int start, end;
			if (oldIndex < newIndex)
			{
				start = oldIndex;
				end = newIndex;
			}
			else
			{
				start = newIndex;
				end = oldIndex;
			}
			for (int i = start; i <= end; ++i)
				List[i].Index = i;
		}

		public override void Replace(int index, IActiveValue<T> newValue)
		{
			base.Replace(index, newValue);
			List[index].Index = index;
		}

		public override void ReplaceRange(int startIndex, int oldCount, IReadOnlyList<IActiveValue<T>> newValues)
		{
			base.ReplaceRange(startIndex, oldCount, newValues);
			var end = startIndex + newValues.Count;
			for (int i = startIndex; i < end; ++i)
				List[i].Index = i;
		}

		public override void MoveRange(int oldIndex, int newIndex, int count)
		{
			base.MoveRange(oldIndex, newIndex, count);

			var min = oldIndex < newIndex ? oldIndex : newIndex;
			var max = (oldIndex > newIndex ? oldIndex : newIndex) + count;

			for (int i = min; i < max; ++i)
				List[i].Index = i;
		}

		public override void Reset(IEnumerable<IActiveValue<T>> values)
		{
			base.Reset(values);
			for (int i = 0; i < List.Count; ++i)
				List[i].Index = i;
		}
	}
}
