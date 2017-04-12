using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.ListModifiers.Bases
{
	internal abstract class ActiveMultiListBase<TSource, TOtherSources, TParameter, TResult> : ActiveMultiListListenerBase<TSource, TOtherSources, TParameter, TResult>
	{
		public override int Count => ResultList.Count;

		public override TResult this[int index] => ResultList[index];

		protected ObservableList<TResult> ResultList { get; }

		public ActiveMultiListBase(IActiveList<TSource> source, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch = null, IEnumerable<string> otherSourcePropertiesToWatch = null, IEnumerable<string> parameterPropertiesToWatch = null) 
			: base(source, parameter, sourcePropertiesToWatch, otherSourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			ResultList = new ObservableList<TResult>();
			ResultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(e);
			ResultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);
		}

		protected override void OnDisposed()
		{
			ResultList.Dispose();
			base.OnDisposed();
		}

		public override IEnumerator<TResult> GetEnumerator() => ResultList.GetEnumerator();
	}
}
