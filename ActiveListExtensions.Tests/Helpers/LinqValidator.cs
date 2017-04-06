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

		public LinqValidator(IReadOnlyList<T1> originalCollection, IReadOnlyList<U> processedCollection, Func<IReadOnlyList<T1>, IEnumerable<U>> linqEquivalent, bool useSetComparison, Func<U, object> keySelector, Func<U, U, bool> additionalComparer = null)
			: this(originalCollection, null, processedCollection, (l1, l2) => linqEquivalent.Invoke(l1), useSetComparison, keySelector, additionalComparer)
		{
		}

		public LinqValidator(IReadOnlyList<T1> originalCollection1, IReadOnlyList<T2> originalCollection2, IReadOnlyList<U> processedCollection, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IEnumerable<U>> linqEquivalent, bool useSetComparison, Func<U, object> keySelector, Func<U, U, bool> additionalComparer = null)
		{
			_originalCollection1 = originalCollection1;
			_originalCollection2 = originalCollection2;
			_processedCollection = processedCollection;
			_linqEquivalent = linqEquivalent;
			_useSetComparison = useSetComparison;
			_comparer = new KeyEqualityComparer<U>(keySelector, additionalComparer);
		}

		public void Validate()
		{
			var fromLinq = _linqEquivalent.Invoke(_originalCollection1, _originalCollection2).ToArray();
			if (_useSetComparison)
			{
				if (!SetCompare(fromLinq, _processedCollection, _comparer))
					throw new Exception("Linq equivalent differs from processed collection.");
			}
			else if (!ListCompare(fromLinq, _processedCollection, _comparer))
				throw new Exception("Linq equivalent differs from processed collection.");
		}

		public static bool SetCompare(IReadOnlyList<U> listOne, IReadOnlyList<U> listTwo, IEqualityComparer<U> comparer)
		{
			if (listOne.Count != listTwo.Count)
				return false;
			if (listOne.Intersect(listTwo, comparer).Count() != listTwo.Count)
				return false;
			return true;
		}

		public static bool ListCompare(IReadOnlyList<U> listOne, IReadOnlyList<U> listTwo, IEqualityComparer<U> comparer)
		{
			if (listOne.Count != listTwo.Count)
				return false;
			if (listOne.Zip(listTwo, (o1, o2) => !comparer.Equals(o1, o2)).Any(b => b))
				return false;
			return true;
		}
	}
}
