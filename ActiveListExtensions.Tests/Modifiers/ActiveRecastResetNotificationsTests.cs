using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveRecastResetNotificationsTests
	{
		[Fact]
		public void EqualInitially()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void InsertAtStart()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 0, 1, 2, 3 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void InsertInMiddle()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 4, 5 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 1, 2, 3, 4, 5 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void InsertAtEnd()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 1, 2, 3, 4 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void AddMultiple()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 4, 6, 7, 8 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void AddWithDuplicates()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 1, 2 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 0, 0, 1, 1, 1, 2, 2 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveFromStart()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 2, 3 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveFromMiddle()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 1, 3 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveFromEnd()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 1, 2 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveMultiple()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 1, 2, 4, 6, 7, 8 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveWithDuplicates()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 0, 0, 1, 1, 1, 2, 2 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 1, 1, 2 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void Scramble()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			value.Value = new[] { 1, 4, 8, 5, 2, 9, 0, 3, 7, 6 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RandomOperations()
		{
			RandomGenerator.ResetRandomGenerator();

			var value = new ActiveValue<IEnumerable<int>>(new int[0]);

			var sut = value.ToActiveList().ActiveRecastResetNotifications();

			for (int i = 0; i < 500; ++i)
			{
				var count = RandomGenerator.GenerateRandomInteger(0, 50);

				value.Value = RandomGenerator.GenerateRandomIntegerList(count);

				Assert.True(value.Value.SequenceEqual(sut));
			}
		}
	}
}
