﻿namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class SelectNode : TreeNode
    {
        public SelectNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var fixedexpr = 
                Expression.Call(
                    typeof(Queryable), 
                    "Cast", 
                    new[] { buildLinqExpressionParameters.InputType }, 
                    buildLinqExpressionParameters.Query.Expression);

            var query = buildLinqExpressionParameters.Query.Provider.CreateQuery(fixedexpr);

            var parameter = buildLinqExpressionParameters.Item ?? Expression.Parameter(buildLinqExpressionParameters.InputType, "o");
            Expression childExpression = fixedexpr;

            MethodInfo addMethod = typeof(Dictionary<string, object>).GetMethod("Add");

            var elements = this.ChildNodes.Select(
                o => Expression.ElementInit(
                    addMethod, 
                    Expression.Constant(o.Text), 
                    Expression.Convert(
                        o.BuildLinqExpression(
                            new BuildLinqExpressionParameters(
                                buildLinqExpressionParameters.Configuration,
                                query, 
                                buildLinqExpressionParameters.InputType, 
                                childExpression,
                                parameter)), 
                        typeof(object))));

            var newDictionary = Expression.New(typeof(Dictionary<string, object>));
            var init = Expression.ListInit(newDictionary, elements);

            var lambda = Expression.Lambda(init, parameter as ParameterExpression);
            return Expression.Call(typeof(Queryable), "Select", new[] { query.ElementType, typeof(Dictionary<string, object>) }, query.Expression, lambda);
        }

        public override int CompareTo(TreeNode other)
        {
            if (other is SelectNode)
            {
                return 0;
            }

            // Select clause should always be last apart from inlinecount
            if (other is InlineCountNode)
            {
                return -1;
            }

            return 1;
        }
    }
}