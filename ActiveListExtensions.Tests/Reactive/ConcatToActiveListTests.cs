using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Reactive
{
	public class ConcatToActiveListTests
	{
		[Fact]
		public void OnNextAddsToList()
		{
			var observable = new Subject<int>();

			var list = observable.ConcatToActiveList();

			Assert.True(list.SequenceEqual(new int[0]));

			observable.OnNext(0);
			observable.OnNext(1);
			observable.OnNext(2);

			Assert.True(list.SequenceEqual(new[] { 0, 1, 2 }));
		}
	}
}
