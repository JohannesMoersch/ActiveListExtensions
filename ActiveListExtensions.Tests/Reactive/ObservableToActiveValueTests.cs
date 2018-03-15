using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Reactive
{
	public class ObservableToActiveValueTests
	{
		[Fact]
		public void ValueChangesWhenObservableChanges()
		{
			var subject = new Subject<int>();

			var value = subject.ToActiveValue();

			Assert.Equal(0, value.Value);

			subject.OnNext(10);

			Assert.Equal(10, value.Value);
		}

		[Fact]
		public void ValueThrowsChangeNotificationWhenObservableChanges()
		{
			var subject = new Subject<int>();

			var value = subject.ToActiveValue();

			bool notificationReceived = false;
			value.PropertyChanged += (s, e) => notificationReceived = true;

			subject.OnNext(10);

			Assert.True(notificationReceived);
		}

		[Fact]
		public void DefaultValueOnEmptyObservable()
		{
			var subject = new Subject<int>();

			var value = subject.ToActiveValue(10);

			Assert.Equal(10, value.Value);
		}
	}
}
