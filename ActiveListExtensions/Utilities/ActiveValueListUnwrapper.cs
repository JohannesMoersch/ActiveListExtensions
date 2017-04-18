using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveValueListUnwrapper<TSource, TParameter, TResult> : IActiveList<TResult>
	{
		internal class ValueStore : IDisposable
		{
			private IActiveValue<TResult> _value;
			public TResult Value => _value.Value;

			public int Index { get; set; }

			public ValueStore(IActiveValue<TResult> value)
			{
				_value = value;
				_value.ValueChanged += ValueChangedHandler;
			}

			private void ValueChangedHandler(IActiveValue<TResult> activeValue, IActiveValueChangedEventArgs<TResult> eventArgs) => ValueChanged?.Invoke(eventArgs.OldValue, eventArgs.NewValue, Index);

			public void Dispose() => _value.ValueChanged -= ValueChangedHandler;

			public event Action<TResult, TResult, int> ValueChanged;
		}

		public TResult this[int index] => (_valueStore as IActiveList<TResult>)[index];

		public int Count => _valueStore.Count;

		private ObservableList<ValueStore, TResult> _valueStore;

		private CollectionWrapper<TSource> _sourceList;

		private Func<TSource, IActiveValue<TResult>> _selector;

		private ValueWatcher<TParameter> _parameterWatcher;

		private bool _isDisposed;

		public ActiveValueListUnwrapper(IReadOnlyList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, IActiveValue<TResult>> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
		{
			_selector = selector;

			_valueStore = new ObservableList<ValueStore, TResult>(store => store.Value);
			_valueStore.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, e);
			_valueStore.CollectionChanged += (s, e) => CollectionChanged?.Invoke(this, e);

			_sourceList = new CollectionWrapper<TSource>(source, sourcePropertiesToWatch?.ToArray());
			_sourceList.ItemModified += (s, i, v) => ItemModified(i, v);
			_sourceList.ItemAdded += (s, i, v) => OnAdded(i, v);
			_sourceList.ItemRemoved += (s, i, v) => OnRemoved(i, v);
			_sourceList.ItemReplaced += (s, i, o, n) => OnReplaced(i, o, n);
			_sourceList.ItemMoved += (s, o, n, v) => OnMoved(o, n, v);
			_sourceList.ItemsReset += s => OnReset(s);

			if (parameter != null)
			{
				_parameterWatcher = new ValueWatcher<TParameter>(parameter, parameterPropertiesToWatch);
				_parameterWatcher.ValueOrValuePropertyChanged += () => OnParameterChanged();
			}

			OnReset(_sourceList);
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;
			_isDisposed = true;

			_valueStore.Dispose();
			_sourceList.Dispose();
		}

		private void OnParameterChanged() => OnReset(_sourceList);

		private ValueStore GetStoreFromSource(int index, TSource source)
		{
			var value = _selector.Invoke(source);
			var store = new ValueStore(value) { Index = index };
			store.ValueChanged += ValueChanged;
			return store;
		}

		private void ValueChanged(TResult oldValue, TResult newValue, int index) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index));

		private void OnAdded(int index, TSource value)
		{
			_valueStore.Add(index, GetStoreFromSource(index, value));

			for (int i = index; i < _valueStore.Count; ++i)
				_valueStore[i].Index = i;
		}

		private void OnRemoved(int index, TSource value)
		{
			_valueStore[index].Dispose();
			_valueStore.Remove(index);

			for (int i = index; i < _valueStore.Count; ++i)
				_valueStore[i].Index = i;
		}

		private void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			_valueStore[index].Dispose();
			_valueStore.Replace(index, GetStoreFromSource(index, newValue));

			_valueStore[index].Index = index;
		}

		private void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			_valueStore.Move(oldIndex, newIndex);

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
				_valueStore[i].Index = i;
		}

		private void OnReset(IReadOnlyList<TSource> newItems)
		{
			foreach (var item in _valueStore)
				item.Dispose();
			_valueStore.Reset(newItems.Select((value, index) => GetStoreFromSource(index, value)));
		}

		protected virtual void ItemModified(int index, TSource value) => OnReplaced(index, value, value);

		public IEnumerator<TResult> GetEnumerator() => (_valueStore as IActiveList<TResult>).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public event PropertyChangedEventHandler PropertyChanged;

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
