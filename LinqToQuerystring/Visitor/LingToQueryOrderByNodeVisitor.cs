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
    using TreeNodes;
    using TreeNodes.Base;

    public class LingToQueryOrderByNodeVisitor : BaseLinqToQueryStringVisitor<IQueryable, BuildLinqExpressionParameters>
    {
        private readonly BaseLinqToQueryStringVisitor<Expression, BuildLinqExpressionParameters> expressionBuilderVisitor;

        public LingToQueryOrderByNodeVisitor(
            BaseLinqToQueryStringVisitor<Expression, BuildLinqExpressionParameters> expressionBuilderVisitor)
        {
            this.expressionBuilderVisitor = expressionBuilderVisitor;
        }

        public override IQueryable VisitOrderByNode(TreeNode node, BuildLinqExpressionParameters context)
        {
            var queryresult = context.Query;
            var orderbyChildren = node.Children.Cast<BaseExplicitOrderByNode>();

            if (!queryresult.Provider.GetType().Name.Contains("DbQueryProvider") && !queryresult.Provider.GetType().Name.Contains("MongoQueryProvider"))
            {
                orderbyChildren = orderbyChildren.Reverse();
            }

            var explicitOrderByNodes = orderbyChildren as IList<BaseExplicitOrderByNode> ?? orderbyChildren.ToList();

            var isFirst = true;

            foreach (var child in explicitOrderByNodes)
            {
                var newContext =
                    new BuildLinqExpressionParameters(
                        context.Configuration,
                        queryresult,
                        context.InputType,
                        queryresult.Expression,
                        null,
                        isFirst);

                queryresult = queryresult.Provider.CreateQuery(this.expressionBuilderVisitor.Visit(child, newContext));
                isFirst = false;
            }

            return queryresult;
        }
    }
}