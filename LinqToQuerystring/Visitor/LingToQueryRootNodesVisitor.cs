// -----------------------------------------------------------------------
//  <copyright file="LingToQueryRootNodesVisitor.cs" company="j-consulting GmbH">
//      Copyright (c) j-consulting GmbH. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace LinqToQuerystring.Visitor
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using TreeNodes;

    public class LingToQueryRootNodesVisitor : LingToQueryChildNodesVisitor
    {
        public override Expression VisitFilterNode(FilterNode node, BuildLinqExpressionParameters context)
        {
            var parameter = context.Item ?? Expression.Parameter(context.InputType, "o");

            var newContext = context.ChangeItem(parameter);
            var lambda =
                Expression.Lambda(this.Visit(node.ChildNode, newContext), parameter as ParameterExpression);

            return Expression.Call(typeof(Queryable), "Where", new[] { context.Query.ElementType }, context.Query.Expression, lambda);
        }

        public override Expression VisitSelectNode(SelectNode node, BuildLinqExpressionParameters context)
        {
            var fixedexpr =
                 Expression.Call(
                     typeof(Queryable),
                     "Cast",
                     new[] { context.InputType },
                     context.Query.Expression);

            var query = context.Query.Provider.CreateQuery(fixedexpr);

            var parameter = context.Item ?? Expression.Parameter(context.InputType, "o");
            Expression childExpression = fixedexpr;

            MethodInfo addMethod = typeof(Dictionary<string, object>).GetMethod("Add");

            var elements = node.ChildNodes.Select(
                o => Expression.ElementInit(
                    addMethod,
                    Expression.Constant(o.Text),
                    Expression.Convert(
                        this.Visit(
                            o,
                            new BuildLinqExpressionParameters(
                                context.Configuration,
                                query,
                                context.InputType,
                                childExpression,
                                parameter)),
                        typeof(object))));

            var newDictionary = Expression.New(typeof(Dictionary<string, object>));
            var init = Expression.ListInit(newDictionary, elements);

            var lambda = Expression.Lambda(init, parameter as ParameterExpression);
            return Expression.Call(typeof(Queryable), "Select", new[] { query.ElementType, typeof(Dictionary<string, object>) }, query.Expression, lambda);
        }

        public override Expression VisitSkipNode(SkipNode node, BuildLinqExpressionParameters context)
        {
            return Expression.Call(
                typeof(Queryable),
                "Skip",
                new[] { context.Query.ElementType },
                context.Query.Expression,
                this.Visit(node.ChildNode, context));
        }

        public override Expression VisitTopNode(TopNode node, BuildLinqExpressionParameters context)
        {
            return Expression.Call(
                typeof(Queryable),
                "Take",
                new[] { context.Query.ElementType },
                context.Query.Expression,
                this.Visit(node.ChildNode, context));
        }
    }
}