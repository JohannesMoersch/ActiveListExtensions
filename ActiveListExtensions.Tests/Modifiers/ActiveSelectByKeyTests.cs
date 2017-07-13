using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveSelectByKeyTests
	{
		[Fact]
		public void ChangingParameterThrowsResetNotification()
		{
			var list = ActiveList.Create(Enumerable.Range(5, 10));

			var lookup = list.ToActiveLookup(i => i / 10);

			var parameter = ActiveValue.Create(0);

			var sut = lookup.ActiveSelectByKey(parameter);

			bool changeOccured = false;
			sut.CollectionChanged += (s, e) => changeOccured = true;

			Assert.False(changeOccured);

			parameter.Value = 1;

			Assert.True(changeOccured);
		}

		[Fact]
		public void ChangingParameterStaysUpToDate()
		{
			var list = ActiveList.Create(Enumerable.Range(5, 10));

			var lookup = list.ToActiveLookup(i => i / 10);

			var parameter = ActiveValue.Create(0);

			var sut = lookup.ActiveSelectByKey(parameter);

			Assert.True(list.Take(5).SequenceEqual(sut));

			parameter.Value = 1;

			Assert.True(list.Skip(5).Take(5).SequenceEqual(sut));
		}
	}
}
