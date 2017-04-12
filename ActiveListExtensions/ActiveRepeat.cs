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

		private IActiveValue<TValue> _value;

		private IActiveValue<int> _count;

		public ActiveRepeat(IActiveValue<TValue> value, IActiveValue<int> count)
		{
			_resultList = new ObservableList<TValue, TValue>(i => i);
			_resultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
			_resultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);

			_value = value;

			_count = count;
		}

		public override IEnumerator<TValue> GetEnumerator() => _resultList.GetEnumerator();
	}
}
