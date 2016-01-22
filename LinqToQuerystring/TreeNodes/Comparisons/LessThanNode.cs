namespace LinqToQuerystring.TreeNodes.Comparisons
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class LessThanNode : TwoChildNode
    {
        public LessThanNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var leftExpression = this.LeftNode.BuildLinqExpression(query, inputType, expression, item);
            var rightExpression = this.RightNode.BuildLinqExpression(query, inputType, expression, item);

            NormalizeTypes(ref leftExpression, ref rightExpression);

            return ApplyEnsuringNullablesHaveValues(Expression.LessThan, leftExpression, rightExpression);
        }

        public override object RetrieveStaticValue()
        {
            throw new NotSupportedException("The node has no static value.");
        }
    }
}