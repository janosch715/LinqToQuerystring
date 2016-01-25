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

        public override IQueryable ModifyQuery(IQueryable query, Type inputType)
        {
            var queryresult = query;
            var orderbyChildren = this.Children.Cast<ExplicitOrderByBase>();

            if (!queryresult.Provider.GetType().Name.Contains("DbQueryProvider") && !queryresult.Provider.GetType().Name.Contains("MongoQueryProvider"))
            {
                orderbyChildren = orderbyChildren.Reverse();
            }

            var explicitOrderByNodes = orderbyChildren as IList<ExplicitOrderByBase> ?? orderbyChildren.ToList();
            explicitOrderByNodes.First().IsFirstChild = true;

            foreach (var child in explicitOrderByNodes)
            {
                var newBuildLinqExpressionParameters =
                    new BuildLinqExpressionParameters(
                        queryresult,
                        inputType,
                        queryresult.Expression,
                        null);

                queryresult = queryresult.Provider.CreateQuery(child.BuildLinqExpression(newBuildLinqExpressionParameters));
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