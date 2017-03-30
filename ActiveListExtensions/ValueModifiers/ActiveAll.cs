using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveAll<TSource> : ActiveListPredicateBase<TSource, bool>
	{
		public ActiveAll(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch = null)
			: base(source, predicate, propertiesToWatch)
		{
		}

		protected override bool GetValue(int count) => count == SourceList.Count;
	}
}
