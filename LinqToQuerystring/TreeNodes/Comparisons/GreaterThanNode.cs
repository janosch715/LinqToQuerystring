namespace LinqToQuerystring.TreeNodes.Comparisons
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class GreaterThanNode : TwoChildNode
    {
        public GreaterThanNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var leftExpression = this.LeftNode.BuildLinqExpression(buildLinqExpressionParameters);
            var rightExpression = this.RightNode.BuildLinqExpression(buildLinqExpressionParameters);

            NormalizeTypes(ref leftExpression, ref rightExpression);

            return ApplyEnsuringNullablesHaveValues(Expression.GreaterThan, leftExpression, rightExpression);
        }

        public override object RetrieveStaticValue()
        {
            throw new NotSupportedException("The node has no static value.");
        }
    }
}