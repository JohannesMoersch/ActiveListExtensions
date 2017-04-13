using ActiveListExtensions.ListModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
    internal class ActiveZip<TSource, TOtherSource, TParameter, TResult> : ActiveMultiListBase<TSource, TOtherSource, TParameter, TResult>
    {
        private Func<TSource, TOtherSource, TResult> _resultSelector;

        private int SourceListCount => Math.Min(SourceList.Count, SourceLists[0].Count);

		public ActiveZip(IActiveList<TSource> source, IReadOnlyList<TOtherSource> otherSource, Func<TSource, TOtherSource, TResult> resultSelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> otherSourcePropertiesToWatch)
			: this(source, otherSource, resultSelector, null, sourcePropertiesToWatch, otherSourcePropertiesToWatch, null)
		{
		}

		public ActiveZip(IActiveList<TSource> source, IReadOnlyList<TOtherSource> otherSource, Func<TSource, TOtherSource, TParameter, TResult> resultSelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> otherSourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, otherSource, (i1, i2) => resultSelector.Invoke(i1, i2, parameter.Value), parameter, sourcePropertiesToWatch, otherSourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveZip(IActiveList<TSource> source, IReadOnlyList<TOtherSource> otherSource, Func<TSource, TOtherSource, TResult> resultSelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> otherSourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
            : base(source, parameter, sourcePropertiesToWatch, otherSourcePropertiesToWatch, parameterPropertiesToWatch)
        {
            _resultSelector = resultSelector ?? throw new ArgumentNullException(nameof(resultSelector));

            if (otherSource == null)
                throw new ArgumentNullException(nameof(otherSource));
            
            AddSourceCollection(0, otherSource);

			Initialize();
		}

        protected override void OnAdded(int collectionIndex, int index, TOtherSource value) => Refresh(index);

        protected override void OnRemoved(int collectionIndex, int index, TOtherSource value) => Refresh(index);

        protected override void OnReplaced(int collectionIndex, int index, TOtherSource oldValue, TOtherSource newValue) => Refresh(index, index);

        protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, TOtherSource value) => Refresh(Math.Min(oldIndex, newIndex), Math.Max(oldIndex, newIndex));

        protected override void OnReset(int collectionIndex, IReadOnlyList<TOtherSource> newItems) => Refresh(0);

        protected override void ItemModified(int collectionIndex, int index, TOtherSource value) => Refresh(index, index);

        protected override void OnAdded(int index, TSource value) => Refresh(index);

        protected override void OnRemoved(int index, TSource value) => Refresh(index);

        protected override void OnReplaced(int index, TSource oldValue, TSource newValue) => Refresh(index, index);

        protected override void OnMoved(int oldIndex, int newIndex, TSource value) => Refresh(Math.Min(oldIndex, newIndex), Math.Max(oldIndex, newIndex));

        protected override void OnReset(IReadOnlyList<TSource> newItems) => Refresh(0);

        protected override void ItemModified(int index, TSource value) => Refresh(index, index);

        private void Refresh(int startIndex, int? endIndex = null)
        {
            for (int i = startIndex; i <= (endIndex ?? ResultList.Count); ++i)
            {
                TResult result;
                if (!TryGetResultForIndex(i, out result))
                {
                    for (int j = ResultList.Count - 1; j >= i; --j)
                        ResultList.Remove(j);
					break;
                }

				if (i < ResultList.Count)
					ResultList.Replace(i, result);
				else
					ResultList.Add(i, result);
            }
        }

        private bool TryGetResultForIndex(int index, out TResult result)
        {
            if (index >= SourceListCount)
            {
                result = default(TResult);
                return false;
            }
            result = _resultSelector.Invoke(SourceList[index], SourceLists[0][index]);
            return true;
        }
    }
}
