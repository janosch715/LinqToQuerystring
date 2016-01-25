namespace LinqToQuerystring.TreeNodes.Functions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class SubstringOfNode : TwoChildNode
    {
        public SubstringOfNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var leftExpression = this.LeftNode.BuildLinqExpression(buildLinqExpressionParameters);
            var rightExpression = this.RightNode.BuildLinqExpression(buildLinqExpressionParameters);

            if (!typeof(string).IsAssignableFrom(leftExpression.Type))
            {
                leftExpression = Expression.Convert(leftExpression, typeof(string));
            }

            if (!typeof(string).IsAssignableFrom(rightExpression.Type))
            {
                rightExpression = Expression.Convert(rightExpression, typeof(string));
            }

            var indexOf = Expression.Call(rightExpression, "IndexOf", null, leftExpression, Expression.Constant(StringComparison.InvariantCultureIgnoreCase));

            return Expression.GreaterThanOrEqual(indexOf, Expression.Constant(0));
        }
    }
}