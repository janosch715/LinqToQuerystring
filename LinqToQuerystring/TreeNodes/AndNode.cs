namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class AndNode : TwoChildNode
    {
        public AndNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            return Expression.AndAlso(
                this.LeftNode.BuildLinqExpression(buildLinqExpressionParameters),
                this.RightNode.BuildLinqExpression(buildLinqExpressionParameters));
        }
    }
}