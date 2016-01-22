namespace LinqToQuerystring.TreeNodes.DataTypes
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class DecimalNode : TreeNode
    {
        public DecimalNode(Type inputType, IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var value = Convert.ToDecimal(this.Text.Replace("m", string.Empty), CultureInfo.InvariantCulture);
            return Expression.Constant(value);
        }
    }
}