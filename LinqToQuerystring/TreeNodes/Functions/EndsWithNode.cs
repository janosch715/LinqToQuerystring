namespace LinqToQuerystring.TreeNodes.Functions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class EndsWithNode : TwoChildNode
    {
        public EndsWithNode(Type inputType, IToken payload, TreeNodeFactory treeNodeFactory)
            : base(inputType, payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var leftExpression = this.LeftNode.BuildLinqExpression(query, inputType, expression, item);
            var rightExpression = this.RightNode.BuildLinqExpression(query, inputType, expression, item);

            if (!typeof(string).IsAssignableFrom(leftExpression.Type))
            {
                leftExpression = Expression.Convert(leftExpression, typeof(string));
            }

            if (!typeof(string).IsAssignableFrom(rightExpression.Type))
            {
                rightExpression = Expression.Convert(rightExpression, typeof(string));
            }

            return Expression.Call(leftExpression, "EndsWith", null, new[] { rightExpression });
        }
    }
}