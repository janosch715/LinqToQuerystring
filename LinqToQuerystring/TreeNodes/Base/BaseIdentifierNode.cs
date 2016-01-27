namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public abstract class BaseIdentifierNode : SingleChildNode
    {
        protected BaseIdentifierNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        protected abstract Expression GetPropertyExpression(BuildLinqExpressionParameters buildLinqExpressionParameters);

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var property = this.GetPropertyExpression(buildLinqExpressionParameters);

            var child = this.ChildNodes.FirstOrDefault();
            if (child == null)
            {
                return property;
            }
            
            if (buildLinqExpressionParameters.Configuration.UseNullForInvalidIdentifiers)
            {
                var variable = Expression.Variable(property.Type);
                var assingment = Expression.Assign(variable, property);

                var newBuildLinqExpressionParameters = buildLinqExpressionParameters.ChangeItem(variable);
                var childExpression = child.BuildLinqExpression(newBuildLinqExpressionParameters);

                var condition = 
                    Expression.Condition(
                        Expression.Equal(variable, Expression.Constant(null, variable.Type)),
                        Expression.Constant(null, childExpression.Type),
                        childExpression);

                return Expression.Block(childExpression.Type, new List<ParameterExpression>() { variable }, assingment, condition);
            }
            else
            {
                var newBuildLinqExpressionParameters = buildLinqExpressionParameters.ChangeItem(property);
                var childExpression = child.BuildLinqExpression(newBuildLinqExpressionParameters);
                return childExpression;
            }
        }
    }
}