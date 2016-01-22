﻿namespace LinqToQuerystring.TreeNodes.Functions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.Exceptions;
    using LinqToQuerystring.TreeNodes.Base;

    public class HourNode : SingleChildNode
    {
        public HourNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var childexpression = this.ChildNode.BuildLinqExpression(query, inputType, expression, item);

            if (!typeof(DateTime).IsAssignableFrom(childexpression.Type) && !typeof(DateTimeOffset).IsAssignableFrom(childexpression.Type))
            {
                throw new FunctionNotSupportedException(childexpression.Type, "hour");
            }

            return Expression.Property(childexpression, "Hour");
        }
    }
}