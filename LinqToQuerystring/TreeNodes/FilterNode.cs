﻿namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class FilterNode : SingleChildNode
    {
        public FilterNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var parameter = item ?? Expression.Parameter(inputType, "o");
            var lambda = Expression.Lambda(
                this.ChildNode.BuildLinqExpression(query, inputType, expression, parameter), new[] { parameter as ParameterExpression });

            return Expression.Call(typeof(Queryable), "Where", new[] { query.ElementType }, query.Expression, lambda);
        }

        public override int CompareTo(TreeNode other)
        {
            if (other is FilterNode)
            {
                return 0;
            }

            return -1;
        }
    }
}