using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Modifiers.Bases
{
	internal abstract class ActiveMultiListBase<TSource, TOtherSources, TResult> : ActiveMultiListListenerBase<TSource, TOtherSources, TResult>
	{
		public override int Count => ResultList.Count;

		public override TResult this[int index] => ResultList[index];

		protected ObservableList<TResult> ResultList { get; }

		public ActiveMultiListBase(IActiveList<TSource> source, IEnumerable<string> sourcePropertiesToWatch = null, IEnumerable<string> otherSourcePropertiesToWatch = null) 
			: base(source, sourcePropertiesToWatch, otherSourcePropertiesToWatch)
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
