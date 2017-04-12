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

		private IActiveValue<int> _start;

		private IActiveValue<int> _count;

		public ActiveRange(IActiveValue<int> start, IActiveValue<int> count)
		{
			_resultList = new ObservableList<int, int>(i => i);
			_resultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
			_resultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);

			_start = start;

			_count = count;
		}

		public override IEnumerator<int> GetEnumerator() => _resultList.GetEnumerator();
	}
}
