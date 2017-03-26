using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal static class ExpressionExtensions
	{
		private class PropertyVisitor : ExpressionVisitor
		{
			private IList<string>[] _properties;

			private Dictionary<ParameterExpression, int> _parameterMap;

			private ParameterExpression GetParentParameter(Expression expression)
			{
				while (expression.NodeType != ExpressionType.Parameter)
				{
					if ((expression.NodeType == ExpressionType.TypeAs || expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked) && expression is UnaryExpression unary)
						expression = unary.Operand;
					else
						break;
				}
				if (expression.NodeType == ExpressionType.Parameter && expression is ParameterExpression parameter)
					return parameter;
				return null;
			}

			protected override Expression VisitMember(MemberExpression node)
			{
				var parameter = GetParentParameter(node.Expression);
				if (parameter != null && _parameterMap.TryGetValue(parameter, out int index))
					_properties[index].Add(node.Member.Name);
				return base.VisitMember(node);
			}

			public IReadOnlyList<IEnumerable<string>> GetReferencedProperties(LambdaExpression expression)
			{
				if (expression == null)
					throw new ArgumentNullException();

				_properties = new List<string>[expression.Parameters.Count];

				_parameterMap = new Dictionary<ParameterExpression, int>();

				for (int i = 0; i < expression.Parameters.Count; ++i)
				{
					_properties[i] = new List<string>();
					_parameterMap.Add(expression.Parameters[i], i);
				}

				Visit(expression);
				return _properties;
			}
		}

		private static IEnumerable<string> ReferencedProperties<T, U>(this Expression<Func<T, U>> expression) => new PropertyVisitor().GetReferencedProperties(expression)[0];

		private static Tuple<IEnumerable<string>, IEnumerable<string>> ReferencedProperties<T1, T2, U>(this Expression<Func<T1, T2, U>> expression)
		{
			var properties = new PropertyVisitor().GetReferencedProperties(expression);
			return Tuple.Create(properties[0], properties[1]);
		}


		public static IEnumerable<string> GetReferencedProperties<T, U>(this Expression<Func<T, U>> expression) => typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T)) ? expression.ReferencedProperties() : Enumerable.Empty<string>();

		public static Tuple<IEnumerable<string>, IEnumerable<string>> GetReferencedProperties<T1, T2, U>(this Expression<Func<T1, T2, U>> expression)
		{
			if (!typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T1)) && !typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T2)))
				return Tuple.Create(Enumerable.Empty<string>(), Enumerable.Empty<string>());
			var properties = expression.ReferencedProperties();
			var sourceProperties = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T1)) ? properties.Item1 : Enumerable.Empty<string>();
			var otherSourceProperties = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T2)) ? properties.Item2 : Enumerable.Empty<string>();
			return Tuple.Create(sourceProperties, otherSourceProperties);
		}
	}
}
