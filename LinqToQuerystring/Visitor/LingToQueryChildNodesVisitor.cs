// -----------------------------------------------------------------------
//  <copyright file="LingToQueryChildNodesVisitor.cs" company="j-consulting GmbH">
//      Copyright (c) j-consulting GmbH. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace LinqToQuerystring.Visitor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using TreeNodes;
    using TreeNodes.Aggregates;
    using TreeNodes.Base;
    using TreeNodes.Comparisons;
    using TreeNodes.Functions;

    public class LingToQueryChildNodesVisitor : BaseLinqToQueryStringVisitor<Expression, BuildLinqExpressionParameters>
    {
        public override Expression VisitIdentifier(BaseIdentifierNode node, BuildLinqExpressionParameters context)
        {
            Expression property;

            if (node is IdentifierNode)
            {
                property = Expression.Property(context.Item, node.Text);
            }
            else if (node is DynamicIdentifierNode)
            {
                var key = node.Text.Trim('[', ']');
                property = Expression.Call(context.Item, "get_Item", null, Expression.Constant(key));
            }
            else
            {
                throw new InvalidOperationException("Expected identifier or dynamic identifier node.");
            }

            var child = node.ChildNodes.FirstOrDefault();
            if (child == null)
            {
                return property;
            }

            if (context.Configuration.UseNullForInvalidIdentifiers)
            {
                var variable = Expression.Variable(property.Type);
                var assingment = Expression.Assign(variable, property);

                var newContext = context.ChangeItem(variable);
                var childExpression = this.Visit(child, newContext);

                var condition =
                    Expression.Condition(
                        Expression.Equal(variable, Expression.Constant(null, variable.Type)),
                        Expression.Constant(null, childExpression.Type),
                        childExpression);

                return Expression.Block(childExpression.Type, new List<ParameterExpression>() { variable }, assingment, condition);
            }
            else
            {
                var newContext = context.ChangeItem(property);
                var childExpression = this.Visit(child, newContext);
                return childExpression;
            }
        }

        public override Expression VisitSingleChildOperation(SingleChildNode node, BuildLinqExpressionParameters context)
        {
            var childExpression = this.Visit(node.ChildNode, context);

            if (node is NotNode)
            {
                if (typeof(bool).IsAssignableFrom(childExpression.Type) == false)
                {
                    childExpression = Expression.Convert(childExpression, typeof(bool));
                }

                return Expression.Not(childExpression);
            }

            if (typeof(string).IsAssignableFrom(childExpression.Type) == false)
            {
                childExpression = Expression.Convert(childExpression, typeof(string));
            }

            if (node is ToUpperNode)
            {
                return Expression.Call(childExpression, "ToUpper", null, null);
            }

            if (node is ToLowerNode)
            {
                return Expression.Call(childExpression, "ToLower", null, null);
            }
            
            throw new NotSupportedException($"The requested function {node.GetType().Name} is not supported.");
        }

        public override Expression VisitDateTimeFunction(SingleChildNode node, BuildLinqExpressionParameters context)
        {
            var childexpression = this.Visit(node.ChildNode, context);

            if ((typeof(DateTime).IsAssignableFrom(childexpression.Type)
                || typeof(DateTimeOffset).IsAssignableFrom(childexpression.Type)) == false)
            {
                childexpression = Expression.Convert(childexpression, typeof(DateTime));
            }

            string propertyName;

            if (node is DayNode || node is DaysNode)
            {
                propertyName = "Day";
            }
            else if (node is MonthNode)
            {
                propertyName = "Month";
            }
            else if (node is YearNode || node is YearsNode)
            {
                propertyName = "Year";
            }
            else if (node is HourNode || node is HoursNode)
            {
                propertyName = "Hour";
            }
            else if (node is MinuteNode || node is MinutesNode)
            {
                propertyName = "Minute";
            }
            else if (node is SecondNode || node is SecondsNode)
            {
                propertyName = "Second";
            }
            else
            {
                throw new NotSupportedException($"The requested function {node.GetType().Name} is not supported.");
            }

            return Expression.Property(childexpression, propertyName);
        }

        public override Expression VisitTwoChildrenOperation(TwoChildNode node, BuildLinqExpressionParameters context)
        {
            var equalsNode = node as EqualsNode;
            if (equalsNode != null)
            {
                return this.VisitEqualsNode(equalsNode, context);
            }

            var leftExpression = this.Visit(node.LeftNode, context);
            var rightExpression = this.Visit(node.RightNode, context);

            if (node is AndNode)
            {
                return Expression.AndAlso(leftExpression, rightExpression);
            }

            if (node is OrNode)
            {
                return Expression.OrElse(leftExpression, rightExpression);
            }

            if ((node is GreaterThanNode)
                || (node is GreaterThanOrEqualNode)
                || (node is LessThanNode)
                || (node is LessThanOrEqualNode))
            {
                Func<Expression, Expression, Expression> expression;

                if (node is GreaterThanNode)
                {
                    expression = Expression.GreaterThan;
                }
                else if (node is GreaterThanOrEqualNode)
                {
                    expression = Expression.GreaterThanOrEqual;
                }
                else if (node is LessThanNode)
                {
                    expression = Expression.LessThan;
                }
                else
                {
                    expression = Expression.LessThanOrEqual;
                }

                NormalizeTypes(ref leftExpression, ref rightExpression);
                return ApplyEnsuringNullablesHaveValues(expression, leftExpression, rightExpression);
            }

            if (node is NotEqualsNode)
            {
                NormalizeTypes(ref leftExpression, ref rightExpression);
                return ApplyWithNullAsValidAlternative(Expression.NotEqual, leftExpression, rightExpression);
            }
            
            if ((node is StartsWithNode)
                || (node is SubstringOfNode)
                || (node is EndsWithNode))
            {
                if (!typeof(string).IsAssignableFrom(leftExpression.Type))
                {
                    leftExpression = Expression.Convert(leftExpression, typeof(string));
                }

                if (!typeof(string).IsAssignableFrom(rightExpression.Type))
                {
                    rightExpression = Expression.Convert(rightExpression, typeof(string));
                }

                Expression functionExpression;
                if (node is StartsWithNode)
                {
                    functionExpression = Expression.Call(leftExpression, "StartsWith", null, rightExpression);
                }
                else if (node is SubstringOfNode)
                {
                    var ignoreCaseParameter = Expression.Constant(StringComparison.InvariantCultureIgnoreCase);
                    var indexOf = Expression.Call(rightExpression, "IndexOf", null, leftExpression, ignoreCaseParameter);
                    functionExpression = Expression.GreaterThanOrEqual(indexOf, Expression.Constant(0));
                }
                else
                {
                    functionExpression = Expression.Call(leftExpression, "EndsWith", null, rightExpression);
                }
                
                return Expression.Condition(
                    Expression.OrElse(
                        Expression.Equal(leftExpression, Expression.Constant(null)), 
                        Expression.Equal(rightExpression, Expression.Constant(null))),
                    Expression.Constant(false),
                    functionExpression);
            }

            throw new NotSupportedException($"The requested function {node.GetType().Name} is not supported.");
        }

        public virtual Expression VisitEqualsNode(TwoChildNode node, BuildLinqExpressionParameters context)
        {
            var leftExpression = this.Visit(node.LeftNode, context);
            var rightExpression = this.Visit(node.RightNode, context);

            // Nasty workaround to avoid comparison of Aggregate functions to true or false which breaks Entity framework
            if (leftExpression.Type == typeof(bool)
                && rightExpression.Type == typeof(bool)
                && rightExpression is ConstantExpression)
            {
                if ((bool)(rightExpression as ConstantExpression).Value)
                {
                    return leftExpression;
                }

                return Expression.Not(leftExpression);
            }

            if (rightExpression.Type == typeof(bool)
                && leftExpression.Type == typeof(bool)
                && leftExpression is ConstantExpression)
            {
                if ((bool)(leftExpression as ConstantExpression).Value)
                {
                    return rightExpression;
                }

                return Expression.Not(rightExpression);
            }

            NormalizeTypes(ref leftExpression, ref rightExpression);

            return ApplyEnsuringNullablesHaveValues(Expression.Equal, leftExpression, rightExpression);
        }

        public override Expression VisitConstantNode(TreeNode node, BuildLinqExpressionParameters context)
        {
            return Expression.Constant(node.RetrieveStaticValue());
        }

        public override Expression VisitAggregateNode(TreeNode node, BuildLinqExpressionParameters context)
        {
            if (node is AnyNode || node is AllNode)
            {
                return this.VisitAllAnyNode(node, context);
            }

            if (node is AverageNode || node is SumNode)
            {
                return this.VisitAverageSumNode(node, context);
            }

            if (node is MaxNode || node is MinNode)
            {
                return this.VisitMaxMinNode(node, context);
            }

            if (node is CountNode)
            {
                return this.VisitCountNode(node, context);
            }

            throw new NotSupportedException($"The requested function {node.GetType().Name} is not supported.");
        }

        public virtual Expression VisitCountNode(TreeNode node, BuildLinqExpressionParameters context)
        {
            var property = this.Visit(node.ChildNodes.ElementAt(0), context);

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

            return Expression.Call(typeof(Enumerable), "Count", new[] { underlyingType }, property);
        }

        public virtual Expression VisitMaxMinNode(TreeNode node, BuildLinqExpressionParameters context)
        {
            var property = this.Visit(node.ChildNodes.ElementAt(0), context);

            var underlyingType = property.Type;
            if (typeof(IEnumerable).IsAssignableFrom(property.Type) && property.Type.GetGenericArguments().Any())
            {
                underlyingType = property.Type.GetGenericArguments()[0];
            }

            string functionName;

            if (node is MaxNode)
            {
                functionName = "Max";
            }
            else if (node is MinNode)
            {
                functionName = "Min";
            }
            else
            {
                throw new NotSupportedException($"The requested function {node.GetType().Name} is not supported.");
            }

            return Expression.Call(typeof(Enumerable), functionName, new[] { underlyingType }, property);
        }

        public virtual Expression VisitAverageSumNode(TreeNode node, BuildLinqExpressionParameters context)
        {
            var property = this.Visit(node.ChildNodes.ElementAt(0), context);

            string functionName;

            if (node is AverageNode)
            {
                functionName = "Average";
            }
            else if (node is SumNode)
            {
                functionName = "Sum";
            }
            else
            {
                throw new NotSupportedException($"The requested function {node.GetType().Name} is not supported.");
            }

            return Expression.Call(typeof(Enumerable), functionName, null, property);
        }

        public virtual Expression VisitAllAnyNode(TreeNode node, BuildLinqExpressionParameters context)
        {
            var property = this.Visit(node.ChildNodes.ElementAt(0), context);  
            var alias = node.ChildNodes.ElementAt(1).Text;
            var filter = node.ChildNodes.ElementAt(2);

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

            var newContext = context.ChangeItem(parameter);

            var lambda = Expression.Lambda(this.Visit(filter, newContext), parameter);

            string functionName;

            if (node is AnyNode)
            {
                functionName = "Any";
            }
            else if (node is AllNode)
            {
                functionName = "All";
            }
            else
            {
                throw new NotSupportedException($"The requested function {node.GetType().Name} is not supported.");
            }

            return Expression.Call(typeof(Enumerable), functionName, new[] { underlyingType }, property, lambda);
        }

        public override Expression VisitAliasNode(AliasNode node, BuildLinqExpressionParameters context)
        {
            var child = node.ChildNode;
            if (child != null)
            {
                return child.BuildLinqExpression(context);
            }

            return context.Item;
        }

        public override Expression VisitIgnoredNode(IgnoredNode node, BuildLinqExpressionParameters context)
        {
            return context.Expression;
        }

        public override Expression VisitExplicitOrderByNode(BaseExplicitOrderByNode node, BuildLinqExpressionParameters context)
        {
            var parameter = context.Item ?? Expression.Parameter(context.InputType, "o");
            var childExpression = context.Expression;

            var temp = parameter;
            foreach (var child in node.ChildNodes)
            {
                childExpression =
                    child.BuildLinqExpression(
                        new BuildLinqExpressionParameters(
                            context.Configuration,
                            context.Query,
                            context.InputType,
                            childExpression,
                            temp));

                temp = childExpression;
            }

            Debug.Assert(childExpression != null, "childExpression should never be null");

            string methodName;

            if (node is AscNode || node is DescNode)
            {
                methodName = "OrderBy";
                if ((context.Query.Provider.GetType().Name.Contains("DbQueryProvider")
                    || context.Query.Provider.GetType().Name.Contains("MongoQueryProvider"))
                    && context.IsFirstOrderBy == false)
                {
                    methodName = "ThenBy";
                }

                if (node is DescNode)
                {
                    methodName += "Descending";
                }
            }
            else
            {
                throw new NotSupportedException($"The requested function {node.GetType().Name} is not supported.");
            }

            var lambda = Expression.Lambda(childExpression, parameter as ParameterExpression);

            return Expression.Call(
                typeof(Queryable), 
                methodName, 
                new[] { context.Query.ElementType, childExpression.Type }, 
                context.Query.Expression, 
                lambda);
        }

        protected static void NormalizeTypes(ref Expression leftSide, ref Expression rightSide)
        {
            var rightSideIsConstant = rightSide is ConstantExpression;
            var leftSideIsConstant = leftSide is ConstantExpression;

            if (rightSideIsConstant && leftSideIsConstant)
            {
                return;
            }

            if (rightSideIsConstant)
            {
                // If we are comparing to an object try to cast it to the same type as the constant
                if (leftSide.Type == typeof(object))
                {
                    leftSide = MapAndCast(leftSide, rightSide);
                }
                else
                {
                    rightSide = MapAndCast(rightSide, leftSide);
                }
            }

            if (leftSideIsConstant)
            {
                // If we are comparing to an object try to cast it to the same type as the constant
                if (rightSide.Type == typeof(object))
                {
                    rightSide = MapAndCast(rightSide, leftSide);
                }
                else
                {
                    leftSide = MapAndCast(leftSide, rightSide);
                }
            }
        }

        private static Expression MapAndCast(Expression from, Expression to)
        {
            var mapped = Configuration.TypeConversionMap(from.Type, to.Type);
            if (mapped != from.Type)
            {
                from = CastIfNeeded(from, mapped);
            }

            return CastIfNeeded(from, to.Type);
        }

        protected static Expression CastIfNeeded(Expression expression, Type type)
        {
            var converted = expression;
            if (!type.IsAssignableFrom(expression.Type))
            {
                var convertToType = Configuration.TypeConversionMap(expression.Type, type);
                converted = Expression.Convert(expression, convertToType);
            }

            return converted;
        }

        protected static Expression ApplyEnsuringNullablesHaveValues(Func<Expression, Expression, Expression> produces, Expression leftExpression, Expression rightExpression)
        {
            var leftExpressionIsNullable = (Nullable.GetUnderlyingType(leftExpression.Type) != null);
            var rightExpressionIsNullable = (Nullable.GetUnderlyingType(rightExpression.Type) != null);

            if (leftExpressionIsNullable && !rightExpressionIsNullable)
            {
                return Expression.AndAlso(
                    Expression.NotEqual(leftExpression, Expression.Constant(null)),
                    produces(Expression.Property(leftExpression, "Value"), rightExpression));
            }

            if (rightExpressionIsNullable && !leftExpressionIsNullable)
            {
                return Expression.AndAlso(
                    Expression.NotEqual(rightExpression, Expression.Constant(null)),
                    produces(leftExpression, Expression.Property(rightExpression, "Value")));
            }

            return produces(leftExpression, rightExpression);
        }

        protected static Expression ApplyWithNullAsValidAlternative(Func<Expression, Expression, Expression> produces, Expression leftExpression, Expression rightExpression)
        {
            var leftExpressionIsNullable = (Nullable.GetUnderlyingType(leftExpression.Type) != null);
            var rightExpressionIsNullable = (Nullable.GetUnderlyingType(rightExpression.Type) != null);

            if (leftExpressionIsNullable && !rightExpressionIsNullable)
            {
                return Expression.OrElse(
                    Expression.Equal(leftExpression, Expression.Constant(null)),
                    produces(Expression.Property(leftExpression, "Value"), rightExpression));
            }

            if (rightExpressionIsNullable && !leftExpressionIsNullable)
            {
                return Expression.OrElse(
                    Expression.Equal(rightExpression, Expression.Constant(null)),
                    produces(leftExpression, Expression.Property(rightExpression, "Value")));
            }

            return produces(leftExpression, rightExpression);
        }
    }
}