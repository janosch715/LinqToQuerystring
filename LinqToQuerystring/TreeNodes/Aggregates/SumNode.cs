namespace LinqToQuerystring.TreeNodes.Aggregates
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class SumNode : TreeNode
    {
        public SumNode(Type inputType, IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var property = this.ChildNodes.ElementAt(0).BuildLinqExpression(query, inputType, expression, item);
            return Expression.Call(typeof(Enumerable), "Sum", null, property);
        }
    }
}
