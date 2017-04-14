using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ToActiveListTests
	{
		[Fact]
		public void ReplaceListInActiveValue()
		{
			RandomGenerator.ResetRandomGenerator();

			var value = new ActiveValue<IEnumerable<int>>();
			value.Value = RandomGenerator.GenerateRandomIntegerList(50);

			var sut = value.ToActiveList();

			Assert.True(value.Value.SequenceEqual(sut));

			value.Value = RandomGenerator.GenerateRandomIntegerList(100);

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void ReplaceListInActiveValueCausesReset()
		{
			RandomGenerator.ResetRandomGenerator();

			var value = new ActiveValue<IEnumerable<int>>();
			value.Value = RandomGenerator.GenerateRandomIntegerList(50);

			var sut = value.ToActiveList();

			bool resetTriggered = false;
			sut.CollectionChanged += (s, e) =>
			{
				if (e.Action == NotifyCollectionChangedAction.Reset)
					resetTriggered = true;
			};

			value.Value = RandomGenerator.GenerateRandomIntegerList(100);

			Assert.True(resetTriggered);
		}

		[Fact]
		public void ModifyListInActiveValue()
		{
			RandomGenerator.ResetRandomGenerator();

			var value = new ActiveValue<IEnumerable<int>>();
			value.Value = RandomGenerator.GenerateRandomIntegerList(100);

			var sut = value.ToActiveList();
			
			var list = new ObservableList<int>();
			value.Value = list;

			list.Add(0, 5);
			list.Add(1, 10);
			list.Add(1, 15);

			Assert.True(value.Value.SequenceEqual(sut));

			list.Remove(1);

			Assert.True(value.Value.SequenceEqual(sut));
		}
	}
}
