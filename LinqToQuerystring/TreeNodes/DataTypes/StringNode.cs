namespace LinqToQuerystring.TreeNodes.DataTypes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class StringNode : TreeNode
    {
        public StringNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public string Value
        {
            get
            {
                var text = this.Text.Trim('\'');
                text = text.Replace(@"\\", @"\");
                text = text.Replace(@"\b", "\b");
                text = text.Replace(@"\t", "\t");
                text = text.Replace(@"\n", "\n");
                text = text.Replace(@"\f", "\f");
                text = text.Replace(@"\r", "\r");
                text = text.Replace(@"\'", "'");
                text = text.Replace(@"''", "'");

                return text;
            }
        }

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