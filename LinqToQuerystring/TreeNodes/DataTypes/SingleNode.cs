﻿namespace LinqToQuerystring.TreeNodes.DataTypes
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class SingleNode : TreeNode
    {
        public SingleNode(Type inputType, IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var value = Convert.ToSingle(this.Text.Replace("f", string.Empty), CultureInfo.InvariantCulture);
            return Expression.Constant(value);
        }
    }
}