namespace LinqToQuerystring.TreeNodes.Functions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.Exceptions;
    using LinqToQuerystring.TreeNodes.Base;

    public class HourNode : SingleChildNode
    {
        public HourNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var childexpression = this.ChildNode.BuildLinqExpression(buildLinqExpressionParameters);

            if (!typeof(DateTime).IsAssignableFrom(childexpression.Type) && !typeof(DateTimeOffset).IsAssignableFrom(childexpression.Type))
            {
                childexpression = Expression.Convert(childexpression, typeof(DateTime));
            }

            return Expression.Property(childexpression, "Hour");
        }
    }
}