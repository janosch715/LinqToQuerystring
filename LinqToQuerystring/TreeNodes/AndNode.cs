namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class AndNode : TwoChildNode
    {
        public AndNode(Type inputType, IToken payload, TreeNodeFactory treeNodeFactory)
            : base(inputType, payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            return Expression.AndAlso(
                this.LeftNode.BuildLinqExpression(query, inputType, expression, item),
                this.RightNode.BuildLinqExpression(query, inputType, expression, item));
        }
    }
}