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

		public static IEnumerable<string> GetReferencedProperties<T, U>(this Expression<Func<T, U>> expression) => new PropertyVisitor().GetReferencedProperties(expression)[0];

		public static Tuple<IEnumerable<string>, IEnumerable<string>> GetReferencedProperties<T1, T2, U>(this Expression<Func<T1, T2, U>> expression)
		{
			var properties = new PropertyVisitor().GetReferencedProperties(expression);
			return Tuple.Create(properties[0], properties[1]);
		}

		public static Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> GetReferencedProperties<T1, T2, T3, U>(this Expression<Func<T1, T2, T3, U>> expression)
		{
			var properties = new PropertyVisitor().GetReferencedProperties(expression);
			return Tuple.Create(properties[0], properties[1], properties[2]);
		}
	}
}
