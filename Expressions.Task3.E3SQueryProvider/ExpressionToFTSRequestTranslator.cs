using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace Expressions.Task3.E3SQueryProvider
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        private readonly StringBuilder _resultStringBuilder;

        public ExpressionToFtsRequestTranslator()
        {
            _resultStringBuilder = new StringBuilder();
        }

        public string Translate(Expression exp)
        {
            Visit(exp);

            return _resultStringBuilder.ToString();
        }

        #region protected methods

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                Expression predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            if (node.Method.DeclaringType == typeof(string))
            {
                Visit(MakeStringBinaryExpression(node));

                return node;
            }

            return base.VisitMethodCall(node);
        }

        private static BinaryExpression MakeStringBinaryExpression(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Equals":
                    return Expression.Equal(node.Object, node.Arguments[0]);
                case "StartsWith":
                    return Expression.Equal(node.Object, ToStartsWith(node.Arguments[0]));
                case "EndsWith":
                    return Expression.Equal(node.Object, ToEndsWith(node.Arguments[0]));
                case "Contains":
                    return Expression.Equal(node.Object, ToContains(node.Arguments[0]));
                default:
                    return null;
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    if (node.Left.NodeType == ExpressionType.MemberAccess &&
                        node.Right.NodeType == ExpressionType.Constant)
                    {
                        Visit(node.Left);
                        _resultStringBuilder.Append("(");
                        Visit(node.Right);
                        _resultStringBuilder.Append(")");
                    }
                    else if (node.Right.NodeType == ExpressionType.MemberAccess &&
                             node.Left.NodeType == ExpressionType.Constant)
                    {
                        Visit(node.Right);
                        _resultStringBuilder.Append("(");
                        Visit(node.Left);
                        _resultStringBuilder.Append(")");
                    }
                    else if (node.Left.NodeType != ExpressionType.MemberAccess)
                        throw new NotSupportedException($"Left operand should be property or field: {node.NodeType}");
                    else if (node.Right.NodeType != ExpressionType.Constant)
                        throw new NotSupportedException($"Right operand should be constant: {node.NodeType}");
                    else
                        throw new NotSupportedException($"Unsupported operands: {node.Left.NodeType} = {node.Right.NodeType}");
                    break;
                case ExpressionType.AndAlso:
                {
                    _resultStringBuilder.AppendLine("\"statements\": [");
                    _resultStringBuilder.Append("  { \"query\":\"");
                    Visit(node.Left);
                    _resultStringBuilder.AppendLine("\"},");
                    _resultStringBuilder.Append("  { \"query\":\"");
                    Visit(node.Right);
                    _resultStringBuilder.AppendLine("\"}");
                    _resultStringBuilder.Append("]");
                    break;
                }
                default:
                    throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _resultStringBuilder.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _resultStringBuilder.Append(node.Value);

            return node;
        }

        #endregion

        private static ConstantExpression ToStartsWith(Expression expression) =>
            Expression.Constant($"{GetArgumentValue(expression)}*");

        private static ConstantExpression ToEndsWith(Expression expression) =>
            Expression.Constant($"*{GetArgumentValue(expression)}");

        private static ConstantExpression ToContains(Expression expression) =>
            Expression.Constant($"*{GetArgumentValue(expression)}*");

        private static string GetArgumentValue(Expression expression) =>
            (string)((ConstantExpression)expression).Value;
    }
}
