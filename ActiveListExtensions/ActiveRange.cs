using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	internal class ActiveRange : ActiveBase<int>
	{
		public override int Count => _resultList.Count;

		public override int this[int index] => _resultList[index];

		private ObservableList<int, int> _resultList;

		private int _start;
		private ValueWatcher<int> _startWatcher;

		private ValueWatcher<int> _countWatcher;

		public ActiveRange(IActiveValue<int> start, IActiveValue<int> count)
		{
			_resultList = new ObservableList<int, int>(i => i);
			_resultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
			_resultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);

			_startWatcher = new ValueWatcher<int>(start, null);
			_startWatcher.ValueOrValuePropertyChanged += StartChanged;

			_countWatcher = new ValueWatcher<int>(count, null);
			_countWatcher.ValueOrValuePropertyChanged += CountChanged;

			CountChanged();
		}

		private void StartChanged()
		{
			if (_startWatcher.Value > _start)
			{
				var max = _startWatcher.Value - _start;

				if (max > _resultList.Count)
					max = _resultList.Count;

				for (int i = max - 1; i >= 0; --i)
					_resultList.Remove(i);

				_start = _startWatcher.Value;

				for (int i = _resultList.Count; i < _countWatcher.Value; ++i)
					_resultList.Add(i, _start + i);
			}
			else if (_startWatcher.Value < _start)
			{
				var min = _startWatcher.Value - _start;

				if (min < 0)
					min = 0;

				for (int i = _resultList.Count - 1; i >= min; --i)
					_resultList.Remove(i);

				_start = _startWatcher.Value;

				var max = _countWatcher.Value - _resultList.Count;

				for (int i = 0; i < max; ++i)
					_resultList.Add(i, _start + i);
			}
		}

		private void CountChanged()
		{
			var count = _countWatcher.Value;

			if (count < 0)
				count = 0;

			if (count > _resultList.Count)
			{
				for (int i = _resultList.Count; i < count; ++i)
					_resultList.Add(i, _startWatcher.Value + i);
			}
			else if (count < _resultList.Count)
			{
				for (int i = _resultList.Count - 1; i >= count; --i)
					_resultList.Remove(i);
			}
		}

		protected override void OnDisposed()
		{
			_startWatcher.Dispose();
			_countWatcher.Dispose();
		}
		public override IEnumerator<int> GetEnumerator() => _resultList.GetEnumerator();
	}
}
