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
        public DateTimeNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public DateTime Value
        {
            get
            {
                var dateText = this.Text
                    .Replace("datetime'", string.Empty)
                    .Replace("'", string.Empty);

                return DateTime.Parse(dateText, null, DateTimeStyles.RoundtripKind);
            }
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            return Expression.Constant(this.Value);
        }

        public override object RetrieveStaticValue()
        {
            return this.Value;
        }
    }
}