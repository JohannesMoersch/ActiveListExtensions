using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal static class ExpressionExtensions
	{
		private class PropertyVisitor : ExpressionVisitor
		{
			private static MethodInfo[] _matchMethods = typeof(JoinOptionExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(method => method.Name == nameof(JoinOptionExtensions.Match)).ToArray();

			private static MethodInfo _getOrElseMethod = typeof(JoinOptionExtensions).GetMethod(nameof(JoinOptionExtensions.GetOrElse), BindingFlags.Public | BindingFlags.Static);

			private IList<string>[] _properties;

			private Dictionary<ParameterExpression, int> _parameterMap;

			private bool _stripJoinOptions;

			private ParameterExpression GetParentParameter(Expression expression)
			{
				if (expression == null)
					return null;
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

			private ParameterExpression GetParameterViaGetOrElse(MethodCallExpression expression)
			{
				if (expression.Method.IsGenericMethod && expression.Method.GetGenericMethodDefinition() == _getOrElseMethod)
					return GetParentParameter(expression.Arguments[0]);
				return null;
			}

			private ParameterExpression GetParameterViaValue(MemberExpression node)
			{
				if (node.Member.DeclaringType.IsGenericType && node.Member.DeclaringType.GetGenericTypeDefinition() == typeof(JoinOption<>) && node.Member.Name == nameof(JoinOption<object>.Value))
					return GetParentParameter(node.Expression);
				return null;
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (!node.Method.IsGenericMethod)
					return base.VisitMethodCall(node);

				var definition = node.Method.GetGenericMethodDefinition();

				if (definition != _matchMethods[0] && definition != _matchMethods[1])
					return base.VisitMethodCall(node);

				var parameter = GetParentParameter(node.Arguments[0]);

				if (parameter == null || !_parameterMap.TryGetValue(parameter, out int index))
					return base.VisitMethodCall(node);

				if (node.Arguments[1] is LambdaExpression lambda)
				{
					foreach (var property in new PropertyVisitor().GetReferencedProperties(lambda, false)[0])
						_properties[index].Add(property);
				}

				return base.VisitMethodCall(node);
			}

			protected override Expression VisitMember(MemberExpression node)
			{
				var parameter = GetParentParameter(node.Expression);
				if (parameter == null && node.Expression.NodeType == ExpressionType.Call && node.Expression is MethodCallExpression callExpression)
					parameter = GetParameterViaGetOrElse(callExpression);
				if (parameter == null && node.Expression.NodeType == ExpressionType.MemberAccess && node.Expression is MemberExpression expression)
					parameter = GetParameterViaValue(expression);
				if (parameter != null && _parameterMap.TryGetValue(parameter, out int index))
					_properties[index].Add(node.Member.Name);
				return base.VisitMember(node);
			}

			public IReadOnlyList<IEnumerable<string>> GetReferencedProperties(LambdaExpression expression, bool stripJoinOptions)
			{
				if (expression == null)
					throw new ArgumentNullException();

				_properties = new List<string>[expression.Parameters.Count];

				_parameterMap = new Dictionary<ParameterExpression, int>();

				_stripJoinOptions = stripJoinOptions;

				for (int i = 0; i < expression.Parameters.Count; ++i)
				{
					_properties[i] = new List<string>();
					_parameterMap.Add(expression.Parameters[i], i);
				}

				Visit(expression);
				return _properties;
			}
		}

		public static IEnumerable<string> GetReferencedProperties<T, U>(this Expression<Func<T, U>> expression, bool stripJoinOptions = false) => new PropertyVisitor().GetReferencedProperties(expression, stripJoinOptions)[0];

		public static Tuple<IEnumerable<string>, IEnumerable<string>> GetReferencedProperties<T1, T2, U>(this Expression<Func<T1, T2, U>> expression, bool stripJoinOptions = false)
		{
			var properties = new PropertyVisitor().GetReferencedProperties(expression, stripJoinOptions);
			return Tuple.Create(properties[0], properties[1]);
		}

		public static Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> GetReferencedProperties<T1, T2, T3, U>(this Expression<Func<T1, T2, T3, U>> expression, bool stripJoinOptions = false)
		{
			var properties = new PropertyVisitor().GetReferencedProperties(expression, stripJoinOptions);
			return Tuple.Create(properties[0], properties[1], properties[2]);
		}
	}
}
