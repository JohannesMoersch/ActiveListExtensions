using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;
using Xunit;

namespace ActiveListExtensions.Tests.Utilities
{
	public class PropertyWatcherListTests
	{
		private PropertyWatcherList<PropertyWatcherTestClass> sut;

		[Fact]
		public void AddItemsThenThrowChangeNotifications()
		{
			int eventsReceived = 0;
			sut = new PropertyWatcherList<PropertyWatcherTestClass>(new[] { "Test", "Things" });
			sut.ValueChanged += (s, e) => ++eventsReceived;

			var item1 = new PropertyWatcherTestClass();
			var item2 = new PropertyWatcherTestClass();
			var item3 = new PropertyWatcherTestClass();
			sut.Add(0, item1);
			sut.Add(1, item2);
			sut.Add(2, item3);

			item1.ThrowChangeNotification("Test");
			item2.ThrowChangeNotification("Things");
			item3.ThrowChangeNotification("Test");

			Assert.Equal(eventsReceived, 3);
		}

		[Fact]
		public void InsertItemsThenThrowChangeNotifications()
		{
			int eventsReceived = 0;
			sut = new PropertyWatcherList<PropertyWatcherTestClass>(new[] { "Test", "Things" });
			sut.ValueChanged += (s, e) => ++eventsReceived;

			var item1 = new PropertyWatcherTestClass();
			var item2 = new PropertyWatcherTestClass();
			var item3 = new PropertyWatcherTestClass();
			sut.Add(0, item1);
			sut.Add(0, item2);
			sut.Add(1, item3);

			item1.ThrowChangeNotification("Test");
			item2.ThrowChangeNotification("Things");
			item3.ThrowChangeNotification("Test");

			Assert.Equal(eventsReceived, 3);
		}

		[Fact]
		public void AddAndThenRemoveItemsThenThrowChangeNotifications()
		{
			int eventsReceived = 0;
			sut = new PropertyWatcherList<PropertyWatcherTestClass>(new[] { "Test", "Things" });
			sut.ValueChanged += (s, e) => ++eventsReceived;

			var item1 = new PropertyWatcherTestClass();
			var item2 = new PropertyWatcherTestClass();
			var item3 = new PropertyWatcherTestClass();
			sut.Add(0, item1);
			sut.Add(1, item2);
			sut.Add(2, item3);
			sut.Remove(0);
			sut.Remove(0);
			sut.Remove(0);

			item1.ThrowChangeNotification("Test");
			item2.ThrowChangeNotification("Things");
			item3.ThrowChangeNotification("Test");

			Assert.Equal(eventsReceived, 0);
		}

		[Fact]
		public void AddItemsThenThrowIgnoredChangeNotifications()
		{
			int eventsReceived = 0;
			sut = new PropertyWatcherList<PropertyWatcherTestClass>(new[] { "Test", "Things" });
			sut.ValueChanged += (s, e) => ++eventsReceived;

			var item1 = new PropertyWatcherTestClass();
			var item2 = new PropertyWatcherTestClass();
			var item3 = new PropertyWatcherTestClass();
			sut.Add(0, item1);
			sut.Add(1, item2);
			sut.Add(2, item3);

			item1.ThrowChangeNotification("Abc");
			item2.ThrowChangeNotification("Abc");
			item3.ThrowChangeNotification("Abc");

			Assert.Equal(eventsReceived, 0);
		}

		[Fact]
		public void AddAndThenResetWithNewItemsThenThrowChangeNotifications()
		{
			int eventsReceived = 0;
			sut = new PropertyWatcherList<PropertyWatcherTestClass>(new[] { "Test", "Things" });
			sut.ValueChanged += (s, e) => ++eventsReceived;

			var item1 = new PropertyWatcherTestClass();
			var item2 = new PropertyWatcherTestClass();
			var item3 = new PropertyWatcherTestClass();
			sut.Add(0, item1);
			sut.Add(0, item2);
			sut.Reset(new[] { item3 });

			item1.ThrowChangeNotification("Test");
			item2.ThrowChangeNotification("Things");
			item3.ThrowChangeNotification("Test");

			Assert.Equal(eventsReceived, 1);
		}
	}

	public class PropertyWatcherTestClass : INotifyPropertyChanged
	{
		public void ThrowChangeNotification(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
