using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	internal class ActiveRepeat<TValue> : ActiveBase<TValue>
	{
		public override int Count => _resultList.Count;

		public override TValue this[int index] => _resultList[index];

		private ObservableList<TValue, TValue> _resultList;

		private ValueWatcher<TValue> _valueWatcher;

		private ValueWatcher<int> _countWatcher;

		public ActiveRepeat(IActiveValue<TValue> value, IActiveValue<int> count)
		{
			_resultList = new ObservableList<TValue, TValue>(i => i);
			_resultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
			_resultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);

			_valueWatcher = new ValueWatcher<TValue>(value, null);
			_valueWatcher.ValueOrValuePropertyChanged += ValueChanged;

			_countWatcher = new ValueWatcher<int>(count, null);
			_countWatcher.ValueOrValuePropertyChanged += CountChanged;

			CountChanged();
		}

		private void ValueChanged()
		{
			for (int i = 0; i < _resultList.Count; ++i)
				_resultList.Replace(i, _valueWatcher.Value);
		}

		private void CountChanged()
		{
			if (_countWatcher.Value > _resultList.Count)
			{
				for (int i = _resultList.Count; i < _countWatcher.Value; ++i)
					_resultList.Add(i, _valueWatcher.Value);
			}
			else if (_countWatcher.Value < _resultList.Count)
			{
				for (int i = _resultList.Count - 1; i >= _countWatcher.Value; --i)
					_resultList.Remove(i);
			}
		}

		protected override void OnDisposed()
		{
			_valueWatcher.Dispose();
			_countWatcher.Dispose();
		}

		public override IEnumerator<TValue> GetEnumerator() => _resultList.GetEnumerator();
	}
}
