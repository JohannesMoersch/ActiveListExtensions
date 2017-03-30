using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveMaxOrDefault<TSource, TValue> : ActiveListCompareBase<TSource, TValue>
	{
		private readonly Comparer<TValue> _comparer;

		public ActiveMaxOrDefault(IActiveList<TSource> source, Func<TSource, TValue> selector, IEnumerable<string> propertiesToWatch = null)
			: base(source, selector, propertiesToWatch)
		{
			_comparer = Comparer<TValue>.Default;

			Initialize();
		}

		protected override int Compare(TValue leftValue, TValue rightValue) => _comparer.Compare(leftValue, rightValue);
	}
}
