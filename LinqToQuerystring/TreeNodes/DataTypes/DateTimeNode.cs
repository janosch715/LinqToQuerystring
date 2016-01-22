namespace LinqToQuerystring.TreeNodes.DataTypes
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class DateTimeNode : TreeNode
    {
        public DateTimeNode(Type inputType, IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var dateText = this.Text
                .Replace("datetime'", string.Empty)
                .Replace("'", string.Empty)
                .Replace(".", ":");

            return Expression.Constant(DateTime.Parse(dateText, null, DateTimeStyles.RoundtripKind));
        }
    }
}