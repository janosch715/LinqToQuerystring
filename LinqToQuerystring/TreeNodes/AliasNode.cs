namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class AliasNode : TreeNode
    {
        public AliasNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var child = this.ChildNodes.FirstOrDefault();
            if (child != null)
            {
                return child.BuildLinqExpression(query, inputType, expression, item);
            }

            return item;
        }
    }
}
