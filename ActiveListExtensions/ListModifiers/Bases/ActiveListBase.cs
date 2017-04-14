using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.ListModifiers.Bases
{
	internal abstract class ActiveListBase<TSource, TResultItem, TParameter, TResult> : ActiveListListenerBase<TSource, TParameter, TResult>
	{
		public override int Count => ResultList.Count;

		public override TResult this[int index] => (ResultList as IActiveList<TResult>)[index];

		protected ObservableList<TResultItem, TResult> ResultList { get; }

		public ActiveListBase(IActiveList<TSource> source, Func<TResultItem, TResult> resultSelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcerPropertiesToWatch = null, IEnumerable<string> parameterPropertiesToWatch = null)
			: base(source, parameter, sourcerPropertiesToWatch, parameterPropertiesToWatch)
		{
			ResultList = CreateResultList(resultSelector);
			ResultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
			ResultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);
		}

		protected override void OnDisposed()
		{
			ResultList.Dispose();
			base.OnDisposed();
		}

		protected virtual ObservableList<TResultItem, TResult> CreateResultList(Func<TResultItem, TResult> resultSelector) => new ObservableList<TResultItem, TResult>(resultSelector);

		public override IEnumerator<TResult> GetEnumerator() => (ResultList as IActiveList<TResult>).GetEnumerator();
	}
}
