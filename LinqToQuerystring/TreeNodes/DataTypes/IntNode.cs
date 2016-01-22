namespace LinqToQuerystring.TreeNodes.DataTypes
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class IntNode : TreeNode
    {
        public IntNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public int Value
            => Convert.ToInt32(this.Text, CultureInfo.InvariantCulture);

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            return Expression.Constant(this.Value);
        }

        public override object RetrieveStaticValue()
        {
            return this.Value;
        }
    }
}