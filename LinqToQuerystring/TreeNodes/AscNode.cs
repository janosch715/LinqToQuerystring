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

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var parameter = item ?? Expression.Parameter(inputType, "o");
            Expression childExpression = expression;

            var temp = parameter;
            foreach (var child in this.ChildNodes)
            {
                childExpression = child.BuildLinqExpression(query, inputType, childExpression, temp);
                temp = childExpression;
            }

            Debug.Assert(childExpression != null, "childExpression should never be null");

            var methodName = "OrderBy";
            if ((query.Provider.GetType().Name.Contains("DbQueryProvider") || query.Provider.GetType().Name.Contains("MongoQueryProvider")) && !this.IsFirstChild)
            {
                methodName = "ThenBy";
            }

            var lambda = Expression.Lambda(childExpression, new[] { parameter as ParameterExpression });
            return Expression.Call(typeof(Queryable), methodName, new[] { query.ElementType, childExpression.Type }, query.Expression, lambda);
        }

        public override object RetrieveStaticValue()
        {
            throw new NotSupportedException("The node has no static value.");
        }
    }
}