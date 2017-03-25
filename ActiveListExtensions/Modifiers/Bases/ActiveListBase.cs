using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Modifiers.Bases
{
	internal abstract class ActiveListBase<TSource, TResult> : ActiveListBase<TSource, TResult, TResult>
	{
		public ActiveListBase(IActiveList<TSource> source, IEnumerable<string> propertiesToWatch = null)
			: base(source, i => i, propertiesToWatch)
		{
		}
	}

	internal abstract class ActiveListBase<TSource, TResultItem, TResult> : ActiveListListenerBase<TSource, TResult>
	{
		public override int Count => ResultList.Count;

		public override TResult this[int index] => (ResultList as IActiveList<TResult>)[index];

		protected ObservableList<TResultItem, TResult> ResultList { get; }

		public ActiveListBase(IActiveList<TSource> source, Func<TResultItem, TResult> resultSelector, IEnumerable<string> propertiesToWatch = null)
			:  base(source, propertiesToWatch)
		{
			ResultList = new ObservableList<TResultItem, TResult>(resultSelector);
			ResultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
			ResultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);
		}

		protected override void OnDisposed()
		{
			ResultList.Dispose();
			base.OnDisposed();
		}

		public override IEnumerator<TResult> GetEnumerator() => (ResultList as IActiveList<TResult>).GetEnumerator();
	}
}
