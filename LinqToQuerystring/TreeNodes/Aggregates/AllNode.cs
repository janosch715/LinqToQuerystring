﻿namespace LinqToQuerystring.TreeNodes.Aggregates
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class AllNode : TreeNode
    {
        public AllNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            var property = this.ChildNodes.ElementAt(0).BuildLinqExpression(query, inputType, expression, item);
            var alias = this.ChildNodes.ElementAt(1).Text;
            var filter = this.ChildNodes.ElementAt(2);

            var underlyingType = property.Type;
            if (typeof(IEnumerable).IsAssignableFrom(property.Type) && property.Type.GetGenericArguments().Any())
            {
                underlyingType = property.Type.GetGenericArguments()[0];
            }
            else
            {
                //We will sometimes need to cater for special cases here, such as Enumerating BsonValues
                underlyingType = Configuration.EnumerableTypeMap(underlyingType);
                var enumerable = typeof(IEnumerable<>).MakeGenericType(underlyingType);
                property = Expression.Convert(property, enumerable);
            }

            var parameter = Expression.Parameter(underlyingType, alias);

            var lambda = Expression.Lambda(
                filter.BuildLinqExpression(query, inputType, expression, parameter), new[] { parameter });

            return Expression.Call(typeof(Enumerable), "All", new[] { underlyingType }, property, lambda);
        }

        public override object RetrieveStaticValue()
        {
            throw new NotSupportedException("The node has no static value.");
        }
    }
}
