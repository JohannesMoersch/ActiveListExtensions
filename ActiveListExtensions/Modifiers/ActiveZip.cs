using ActiveListExtensions.Modifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Modifiers
{
    internal class ActiveZip<TSource, TOtherSource, TResult> : ActiveMultiListBase<TSource, TOtherSource, TResult>
    {
        public ActiveZip(IActiveList<TSource> source, IEnumerable<TOtherSource> otherSource, Func<TSource, TOtherSource, TResult> resultSelector, IEnumerable<string> propertiesToWatch = null)
            : base(source, propertiesToWatch)
        {
            if (otherSource == null)
                throw new ArgumentNullException(nameof(otherSource));
            Initialize();
            AddSourceCollection(0, (otherSource as IReadOnlyList<TOtherSource>) ?? otherSource.ToArray());
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

        }
    }
}
