namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class OrderByNode : QueryModifier
    {
        public OrderByNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            throw new NotSupportedException(
                "Orderby is just a placeholder and should be handled differently in Extensions.cs");
        }

        public override IQueryable ModifyQuery(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var queryresult = buildLinqExpressionParameters.Query;
            var orderbyChildren = this.Children.Cast<BaseExplicitOrderByNode>();

            if (!queryresult.Provider.GetType().Name.Contains("DbQueryProvider") && !queryresult.Provider.GetType().Name.Contains("MongoQueryProvider"))
            {
                orderbyChildren = orderbyChildren.Reverse();
            }

            var explicitOrderByNodes = orderbyChildren as IList<BaseExplicitOrderByNode> ?? orderbyChildren.ToList();

            var isFirst = true;

            foreach (var child in explicitOrderByNodes)
            {
                var newBuildLinqExpressionParameters =
                    new BuildLinqExpressionParameters(
                        buildLinqExpressionParameters.Configuration,
                        queryresult,
                        buildLinqExpressionParameters.InputType,
                        queryresult.Expression,
                        null,
                        isFirst);

                queryresult = queryresult.Provider.CreateQuery(child.BuildLinqExpression(newBuildLinqExpressionParameters));
                isFirst = false;
            }

            return queryresult;
        }

        public override int CompareTo(TreeNode other)
        {
            if (other is OrderByNode)
            {
                return 0;
            }

            if (other is FilterNode || other is ExpandNode)
            {
                return 1;
            }

            return -1;
        }
    }
}