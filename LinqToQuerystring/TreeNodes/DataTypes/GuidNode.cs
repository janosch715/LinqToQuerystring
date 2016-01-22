namespace LinqToQuerystring.TreeNodes.DataTypes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class GuidNode : TreeNode
    {
        public GuidNode(Type inputType, IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var guidText = this.Text.Replace("guid'", string.Empty).Replace("'", string.Empty);
            return Expression.Constant(new Guid(guidText));
        }
    }
}