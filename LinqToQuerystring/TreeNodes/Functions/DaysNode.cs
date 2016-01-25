﻿namespace LinqToQuerystring.TreeNodes.Functions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.Exceptions;
    using LinqToQuerystring.TreeNodes.Base;

    public class DaysNode : SingleChildNode
    {
        public DaysNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var childexpression = this.ChildNode.BuildLinqExpression(buildLinqExpressionParameters);

            if (!typeof(DateTime).IsAssignableFrom(childexpression.Type))
            {
                throw new FunctionNotSupportedException(childexpression.Type, "days");
            }

            return Expression.Property(childexpression, "Day");
        }
    }
}