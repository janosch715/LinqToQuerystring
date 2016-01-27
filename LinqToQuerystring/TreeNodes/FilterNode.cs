namespace LinqToQuerystring.TreeNodes
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

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var parameter = buildLinqExpressionParameters.Item ?? Expression.Parameter(buildLinqExpressionParameters.InputType, "o");

            var newBuildLinqExpressionParameters = buildLinqExpressionParameters.ChangeItem(parameter);
            var lambda = 
                Expression.Lambda(this.ChildNode.BuildLinqExpression(newBuildLinqExpressionParameters), parameter as ParameterExpression);

            return Expression.Call(typeof(Queryable), "Where", new[] { buildLinqExpressionParameters.Query.ElementType }, buildLinqExpressionParameters.Query.Expression, lambda);
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