namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class DescNode : BaseExplicitOrderByNode
    {
        public DescNode(IToken payload, TreeNodeFactory treeNodeFactory)
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
                var newBuildLinqExpressionParameters =
                    new BuildLinqExpressionParameters(
                        buildLinqExpressionParameters.Configuration,
                        buildLinqExpressionParameters.Query,
                        buildLinqExpressionParameters.InputType,
                        childExpression,
                        temp);

                childExpression = child.BuildLinqExpression(newBuildLinqExpressionParameters);
                temp = childExpression;
            }

            Debug.Assert(childExpression != null, "childExpression should never be null");

            var methodName = "OrderByDescending";
            if ((buildLinqExpressionParameters.Query.Provider.GetType().Name.Contains("DbQueryProvider") 
                || buildLinqExpressionParameters.Query.Provider.GetType().Name.Contains("MongoQueryProvider")) 
                && buildLinqExpressionParameters.IsFirstOrderBy == false)
            {
                methodName = "ThenByDescending";
            }

            var lambda = Expression.Lambda(childExpression, new[] { parameter as ParameterExpression });
            return Expression.Call(typeof(Queryable), methodName, new[] { buildLinqExpressionParameters.Query.ElementType, childExpression.Type }, buildLinqExpressionParameters.Query.Expression, lambda);
        }
    }
}