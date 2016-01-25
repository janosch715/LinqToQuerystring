namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class AscNode : ExplicitOrderByBase
    {
        public AscNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var parameter = buildLinqExpressionParameters.Item ?? Expression.Parameter(buildLinqExpressionParameters.InputType, "o");
            var childExpression = buildLinqExpressionParameters.Expression;

            var temp = parameter;
            foreach (var child in this.ChildNodes)
            {
                childExpression = child.BuildLinqExpression(new BuildLinqExpressionParameters(buildLinqExpressionParameters.Query, buildLinqExpressionParameters.InputType, childExpression, temp));
                temp = childExpression;
            }

            Debug.Assert(childExpression != null, "childExpression should never be null");

            var methodName = "OrderBy";
            if ((buildLinqExpressionParameters.Query.Provider.GetType().Name.Contains("DbQueryProvider") || buildLinqExpressionParameters.Query.Provider.GetType().Name.Contains("MongoQueryProvider")) && !this.IsFirstChild)
            {
                methodName = "ThenBy";
            }

            var lambda = Expression.Lambda(childExpression, parameter as ParameterExpression);
            return Expression.Call(typeof(Queryable), methodName, new[] { buildLinqExpressionParameters.Query.ElementType, childExpression.Type }, buildLinqExpressionParameters.Query.Expression, lambda);
        }

        public override object RetrieveStaticValue()
        {
            throw new NotSupportedException("The node has no static value.");
        }
    }
}