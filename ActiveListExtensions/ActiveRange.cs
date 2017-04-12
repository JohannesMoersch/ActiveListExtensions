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
		}

		private void CountChanged()
		{
		}

		protected override void OnDisposed()
		{
			_startWatcher.Dispose();
			_countWatcher.Dispose();
		}
		public override IEnumerator<int> GetEnumerator() => _resultList.GetEnumerator();
	}
}
