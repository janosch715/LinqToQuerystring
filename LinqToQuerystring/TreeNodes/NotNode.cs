namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class NotNode : SingleChildNode
    {
        public NotNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var childExpression = this.ChildNode.BuildLinqExpression(buildLinqExpressionParameters);
            if (!typeof(bool).IsAssignableFrom(childExpression.Type))
            {
                childExpression = Expression.Convert(childExpression, typeof(bool));
            }

            return Expression.Not(childExpression);
        }
    }
}