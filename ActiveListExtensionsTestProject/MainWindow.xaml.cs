using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ActiveListExtensions;
using ActiveListExtensions.Utilities;
using System.Data;

namespace ActiveListExtensionsTestProject
{
	public partial class MainWindow : Window
	{
		public class TestData : INotifyPropertyChanged
		{
			private int _one;
			public int One
			{
				get { return _one; }
				set
				{
					if (_one == value)
						return;
					_one = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(One)));
				}
			}

			private string _two;
			public string Two
			{
				get { return _two; }
				set
				{
					if (_two == value)
						return;
					_two = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Two)));
				}
			}

			private ObservableCollection<char> _three = new ObservableCollection<char>();
			public ObservableCollection<char> Three
			{
				get { return _three; }
				set
				{
					if (_three == value)
						return;
					_three = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Three)));
				}
			}

			public TestData(int one, string two, string three)
			{
				One = one;
				Two = two;
				foreach (var c in three)
					Three.Add(c);
			}

			public event PropertyChangedEventHandler PropertyChanged;
		}

		public IActiveList<string> Data { get; }

		public ObservableCollection<TestData> Source { get; }

		public IActiveList<string> Combined { get; }

		public IActiveList<char> SelectMany { get; }

		public IActiveValue<int> SelectManyCount { get; }

		public class BlahRow : DataRow
		{
			public BlahRow(DataRowBuilder builder) : base(builder)
			{
			}
		}

		public class BlahTable : TypedTableBase<BlahRow>
		{
		}

		public MainWindow()
		{
			BlahTable aa = new BlahTable();
			var blah = aa.ToActiveList();

			var a = new[] { 1, 2, 3, 4, 4, 4 };
			var b = new[] { 1, 3, 4 };

			var c = a.Except(b).ToArray();

			Source = new ObservableCollection<TestData>();
			Source.Add(new TestData(1, "One", "Abc"));
			Source.Add(new TestData(2, "Two", "Def"));
			Source.Add(new TestData(3, "Three", "Ghi"));

			var activeList = (Source as IList<TestData>).ToActiveList();

			Data = activeList.ActiveWhere(x => x.One % 2 == 0).ActiveSelect(x => $"{x.One} - {x.Two}");

			Data.ToActiveValue(s => (s as IReadOnlyList<string>).Count);

			Combined = activeList.ActiveSelect(x => $"{x.One} - {x.Two}").ActiveConcat(Data).ActiveOrderBy(s => s);

			SelectMany = activeList.ActiveSelectMany(x => x.Three).ActiveReverse().ActiveSkip(2).ActiveTake(8);

			SelectManyCount = SelectMany.ToActiveValue(l => l.Count);

			InitializeComponent();
		}

		private IReadOnlyList<string> CreateLambda()
		{
			return (Source as IList<TestData>).ToActiveList().ActiveWhere(x => x.One % 2 == 0).ActiveSelect(x => $"{x.One} - {x.Two}");
		}

		private Random _random = new Random(1234567890);

		private void AddButton(object sender, RoutedEventArgs e)
		{
			var index = _random.Next(Source.Count);
			Source.Insert(index, GenerateRandom());
		}

		private void RemoveButton(object sender, RoutedEventArgs e)
		{
			if (Source.Count == 0)
				return;
			var index = _random.Next(Source.Count);
			Source.RemoveAt(index);
		}

		private void MoveButton(object sender, RoutedEventArgs e)
		{
			if (Source.Count == 0)
				return;
			var oldIndex = _random.Next(Source.Count);
			var newIndex = _random.Next(Source.Count);
			Source.Move(oldIndex, newIndex);
		}

		private void ReplaceButton(object sender, RoutedEventArgs e)
		{
			if (Source.Count == 0)
				return;
			var index = _random.Next(Source.Count);
			Source[index] = GenerateRandom();
		}

		private void ResetButton(object sender, RoutedEventArgs e)
		{
			Source.Clear();
		}

		private void ChangeNumber(object sender, RoutedEventArgs e)
		{
			if (Source.Count == 0)
				return;
			var index = _random.Next(Source.Count);
			Source[index].One = GenerateRandom().One;
		}

		private void ChangeString(object sender, RoutedEventArgs e)
		{
			if (Source.Count == 0)
				return;
			var index = _random.Next(Source.Count);
			Source[index].Two = GenerateRandom().Two;
		}

		private void ReplaceCharacters(object sender, RoutedEventArgs e)
		{
			if (Source.Count == 0)
				return;
			var index = _random.Next(Source.Count);
			var collection = new ObservableCollection<char>();
			foreach (var c in GenerateRandom().Two)
				collection.Add(c);
			Source[index].Three = collection;
		}

		private void ChangeCharacters(object sender, RoutedEventArgs e)
		{
			if (Source.Count == 0)
				return;
			var index = _random.Next(Source.Count);
			if (Source[index].Three.Count > 0)
			{
				switch (_random.Next(0, 3))
				{
					case 0:
						Source[index].Three.Insert(_random.Next(0, Source[index].Three.Count + 1), (char)_random.Next('a', 'z'));
						break;
					case 1:
						Source[index].Three.RemoveAt(_random.Next(0, Source[index].Three.Count));
						break;
					case 2:
						Source[index].Three[_random.Next(0, Source[index].Three.Count)] = (char)_random.Next('a', 'z');
						break;
				}
			}
			else
				Source[index].Three.Add((char)_random.Next('a', 'z'));
		}

		private TestData GenerateRandom()
		{
			return new TestData(_random.Next(100), new String(Enumerable.Range(0, _random.Next(3, 6)).Select(i => (char)_random.Next('a', 'z')).ToArray()), new String(Enumerable.Range(0, _random.Next(3, 6)).Select(i => (char)_random.Next('a', 'z')).ToArray()));
		}
	}
}
