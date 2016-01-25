namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class DynamicIdentifierNode : SingleChildNode
    {
        public DynamicIdentifierNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var key = this.Text.Trim(new[] { '[', ']' });
            var property = Expression.Call(buildLinqExpressionParameters.Item, "get_Item", null, Expression.Constant(key));

            var child = this.ChildNodes.FirstOrDefault();
            if (child != null)
            {
                var newBuildLinqExpressionParameters =
                    new BuildLinqExpressionParameters(
                        buildLinqExpressionParameters.Query,
                        buildLinqExpressionParameters.InputType,
                        buildLinqExpressionParameters.Expression,
                        property);

                return child.BuildLinqExpression(newBuildLinqExpressionParameters);
            }

            return property;
        }
    }
}