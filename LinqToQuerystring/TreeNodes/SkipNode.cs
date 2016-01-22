namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class SkipNode : SingleChildNode
    {
        public SkipNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            return Expression.Call(typeof(Queryable), "Skip", new[] { query.ElementType }, query.Expression, this.ChildNode.BuildLinqExpression(query, inputType, expression, item));
        }

        public override int CompareTo(TreeNode other)
        {
            if (other is SkipNode)
            {
                return 0;
            }

            if (other is OrderByNode || other is FilterNode)
            {
                return 1;
            }

            return -1;
        }

        public override object RetrieveStaticValue()
        {
            return this.ChildNode.RetrieveStaticValue();
        }
    }
}