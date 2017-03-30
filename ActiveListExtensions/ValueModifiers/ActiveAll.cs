using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveAll<TSource> : ActiveListPredicateBase<TSource>
	{
		public ActiveAll(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch = null)
			: base(source, i => !predicate.Invoke(i), propertiesToWatch)
		{
			Initialize();
		}

		protected override bool GetValue(bool predicateMet) => SourceList.Count == 0 || !predicateMet;
	}
}
