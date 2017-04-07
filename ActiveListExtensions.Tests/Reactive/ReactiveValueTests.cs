using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Reactive
{
	public class ReactiveValueTests
	{
		[Fact]
		public void ObservableTriggersOnSubscribe()
		{
			var value = new ActiveValue<int>();

			value.Value = 10;

			int observedValue = 0;

			var sut = value.ObserveValue();

			sut.Subscribe(i => observedValue = i);

			Assert.Equal(value.Value, observedValue);
		}

		[Fact]
		public void ObservableTriggersOnChange()
		{
			var value = new ActiveValue<int>();

			var observedValue = value.Value;

			var sut = value.ObserveValue();

			sut.Subscribe(i => observedValue = i);

			value.Value = 20;

			Assert.Equal(value.Value, observedValue);
		}
	}
}
