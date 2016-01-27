namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class AliasNode : SingleChildNode
    {
        public AliasNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var child = this.ChildNode;
            if (child != null)
            {
                return child.BuildLinqExpression(buildLinqExpressionParameters);
            }

            return buildLinqExpressionParameters.Item;
        }
    }
}
