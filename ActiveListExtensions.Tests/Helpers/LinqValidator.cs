using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Tests.Helpers
{
	public class LinqValidator<T1, T2, U>
	{
		private IReadOnlyList<T1> _originalCollection1;

		private IReadOnlyList<T2> _originalCollection2;

		private IReadOnlyList<U> _processedCollection;

		private Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IEnumerable<U>> _linqEquivalent;

		private bool _useSetComparison;

		private KeyEqualityComparer<U> _comparer;

		public LinqValidator(IReadOnlyList<T1> originalCollection, IReadOnlyList<U> processedCollection, Func<IReadOnlyList<T1>, IEnumerable<U>> linqEquivalent, bool useSetComparison, Func<U, object> keySelector)
			: this(originalCollection, null, processedCollection, (l1, l2) => linqEquivalent.Invoke(l1), useSetComparison, keySelector)
		{
		}

		public LinqValidator(IReadOnlyList<T1> originalCollection1, IReadOnlyList<T2> originalCollection2, IReadOnlyList<U> processedCollection, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IEnumerable<U>> linqEquivalent, bool useSetComparison, Func<U, object> keySelector)
		{
			_originalCollection1 = originalCollection1;
			_originalCollection2 = originalCollection2;
			_processedCollection = processedCollection;
			_linqEquivalent = linqEquivalent;
			_useSetComparison = useSetComparison;
			_comparer = new KeyEqualityComparer<U>(keySelector);
		}

		public void Validate()
		{
			var fromLinq = _linqEquivalent.Invoke(_originalCollection1, _originalCollection2).ToArray();
			if (fromLinq.Length != _processedCollection.Count)
				throw new Exception("Linq equivalent differs from processed collection.");
			if (_useSetComparison)
			{
				if (fromLinq.Intersect(_processedCollection, _comparer).Count() != _processedCollection.Count)
					throw new Exception("Linq equivalent differs from processed collection.");
			}
			else
			{
				if (fromLinq.Zip(_processedCollection, (o1, o2) => !_comparer.Equals(o1, o2)).Any(b => b))
					throw new Exception("Linq equivalent differs from processed collection.");
			}
		}
	}
}
