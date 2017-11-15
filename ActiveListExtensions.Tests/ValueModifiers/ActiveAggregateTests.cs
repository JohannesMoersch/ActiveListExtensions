using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveAggregateTests
	{
		[Fact]
		public void InitialValueIsCorrect()
		{
			var value = ActiveValue.Create(10);

			var sut = value.ActiveAggregate(1, (currentValue, newValue) => currentValue + newValue);

			Assert.Equal(11, sut.Value);
		}

		[Fact]
		public void UpdatesOnSourceChange()
		{
			var value = ActiveValue.Create(0);

			var sut = value.ActiveAggregate(0, (currentValue, newValue) => currentValue + newValue);

			Assert.Equal(0, sut.Value);

			value.Value = 10;

			Assert.Equal(10, sut.Value);

			value.Value = 5;

			Assert.Equal(15, sut.Value);
		}
	}
}
