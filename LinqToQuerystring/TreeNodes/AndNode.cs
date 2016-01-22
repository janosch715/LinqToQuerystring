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

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            return Expression.AndAlso(
                this.LeftNode.BuildLinqExpression(query, inputType, expression, item),
                this.RightNode.BuildLinqExpression(query, inputType, expression, item));
        }

        public override object RetrieveStaticValue()
        {
            throw new NotSupportedException("The node has no static value.");
        }
    }
}