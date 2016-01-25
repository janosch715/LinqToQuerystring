namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class IdentifierNode : SingleChildNode
    {
        public IdentifierNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var property = Expression.Property(buildLinqExpressionParameters.Item, this.Text);

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