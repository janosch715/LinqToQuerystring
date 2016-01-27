namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class IdentifierNode : BaseIdentifierNode
    {
        public IdentifierNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        protected override Expression GetPropertyExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            return Expression.Property(buildLinqExpressionParameters.Item, this.Text);
        }
    }
}