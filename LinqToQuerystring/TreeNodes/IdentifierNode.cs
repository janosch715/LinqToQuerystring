namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class IdentifierNode : TreeNode
    {
        public IdentifierNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var property = Expression.Property(item, this.Text);

            var child = this.ChildNodes.FirstOrDefault();
            if (child != null)
            {
                return child.BuildLinqExpression(query, inputType, expression, property);
            }

            return property;
        }
    }
}