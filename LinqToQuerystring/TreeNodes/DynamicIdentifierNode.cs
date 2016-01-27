namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class DynamicIdentifierNode : BaseIdentifierNode
    {
        public DynamicIdentifierNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        protected override Expression GetPropertyExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var key = this.Text.Trim('[', ']');
            return Expression.Call(buildLinqExpressionParameters.Item, "get_Item", null, Expression.Constant(key));
        }
    }
}