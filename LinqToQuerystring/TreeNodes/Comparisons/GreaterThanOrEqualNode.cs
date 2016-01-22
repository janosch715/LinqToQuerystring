namespace LinqToQuerystring.TreeNodes.Comparisons
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class GreaterThanOrEqualNode : TwoChildNode
    {
        public GreaterThanOrEqualNode(Type inputType, IToken payload, TreeNodeFactory treeNodeFactory)
            : base(inputType, payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var leftExpression = this.LeftNode.BuildLinqExpression(query, inputType, expression, item);
            var rightExpression = this.RightNode.BuildLinqExpression(query, inputType, expression, item);

            NormalizeTypes(ref leftExpression, ref rightExpression);

            return ApplyEnsuringNullablesHaveValues(Expression.GreaterThanOrEqual, leftExpression, rightExpression);
        }
    }
}