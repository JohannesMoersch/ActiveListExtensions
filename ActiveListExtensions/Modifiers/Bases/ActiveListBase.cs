using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Modifiers.Bases
{
	internal abstract class ActiveListBase<T, U> : ActiveListListenerBase<T, U>
	{
		public override int Count => ResultList.Count;

		public override U this[int index] => ResultList[index];

		protected ObservableList<U> ResultList { get; }

		public ActiveListBase(IActiveList<T> source, IEnumerable<string> propertiesToWatch = null)
			:  base(source, propertiesToWatch)
		{
			ResultList = new ObservableList<U>();
			ResultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
			ResultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);
		}

		protected override void OnDisposed()
		{
			ResultList.Dispose();
			base.OnDisposed();
		}

		public override IEnumerator<U> GetEnumerator() => ResultList.GetEnumerator();
	}
}
