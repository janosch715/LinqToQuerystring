// -----------------------------------------------------------------------
//  <copyright file="BaseLinqToQueryStringVisitor.cs" company="j-consulting GmbH">
//      Copyright (c) j-consulting GmbH. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace LinqToQuerystring.Visitor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using TreeNodes;
    using TreeNodes.Aggregates;
    using TreeNodes.Base;
    using TreeNodes.Comparisons;
    using TreeNodes.DataTypes;
    using TreeNodes.Functions;

    public abstract class BaseLinqToQueryStringVisitor<TResult, TContext>
    {
        private static readonly ISet<Type> SingleChildOperationNodes;

        private static readonly ISet<Type> TwoChildrenOperationNodes;

        private static readonly ISet<Type> DataNodes;

        private static readonly ISet<Type> AggregateOperationNodes;

        private static readonly ISet<Type> DateTimeFunctionNodes;

        static BaseLinqToQueryStringVisitor()
        {
            SingleChildOperationNodes = new HashSet<Type>()
            {
                typeof(NotNode),
                typeof(ToUpperNode),
                typeof(ToLowerNode),
            };

            TwoChildrenOperationNodes = new HashSet<Type>()
            {
                typeof(EqualsNode),
                typeof(GreaterThanNode),
                typeof(GreaterThanOrEqualNode),
                typeof(LessThanNode),
                typeof(LessThanOrEqualNode),
                typeof(NotEqualsNode),
                typeof(EndsWithNode),
                typeof(StartsWithNode),
                typeof(SubstringOfNode),
                typeof(AndNode),
                typeof(OrNode)
            };

            DataNodes = new HashSet<Type>()
            {
                typeof(BoolNode),
                typeof(ByteNode),
                typeof(DateTimeNode),
                typeof(DecimalNode),
                typeof(DoubleNode),
                typeof(GuidNode),
                typeof(IntNode),
                typeof(LongNode),
                typeof(SingleNode),
                typeof(StringNode),
                typeof(NullNode)
            };

            AggregateOperationNodes = new HashSet<Type>()
            {
                typeof(AllNode),
                typeof(AnyNode),
                typeof(AverageNode),
                typeof(CountNode),
                typeof(MaxNode),
                typeof(MinNode),
                typeof(SumNode)
            };

            DateTimeFunctionNodes = new HashSet<Type>()
            {
                typeof (DayNode),
                typeof (DaysNode),
                typeof (HourNode),
                typeof (HoursNode),
                typeof (MinuteNode),
                typeof (MinutesNode),
                typeof (MonthNode),
                typeof (SecondNode),
                typeof (SecondsNode),
                typeof (YearNode),
                typeof (YearsNode),
            };
        }

        public TResult Visit(TreeNode node, TContext context)
        {
            if (SingleChildOperationNodes.Contains(node.GetType()))
            {
                var singleChildNode = node as SingleChildNode;
                Debug.Assert(singleChildNode != null);

                return this.VisitSingleChildOperation(singleChildNode, context);
            }

            if (DateTimeFunctionNodes.Contains(node.GetType()))
            {
                var singleChildNode = node as SingleChildNode;
                Debug.Assert(singleChildNode != null);

                return this.VisitDateTimeFunction(singleChildNode, context);
            }

            if (TwoChildrenOperationNodes.Contains(node.GetType()))
            {
                var twoChildNode = node as TwoChildNode;
                Debug.Assert(twoChildNode != null);

                return this.VisitTwoChildrenOperation(twoChildNode, context);
            }

            if (DataNodes.Contains(node.GetType()))
            {
                return this.VisitConstantNode(node, context);
            }

            if (AggregateOperationNodes.Contains(node.GetType()))
            {
                return this.VisitAggregateNode(node, context);
            }

            var identifierNode = node as BaseIdentifierNode;
            if (identifierNode != null)
            {
                return this.VisitIdentifier(identifierNode, context);
            }

            var aliasNode = node as AliasNode;
            if (aliasNode != null)
            {
                return this.VisitAliasNode(aliasNode, context);
            }

            var ignoredNode = node as IgnoredNode;
            if (ignoredNode != null)
            {
                return this.VisitIgnoredNode(ignoredNode, context);
            }

            var explicitOrderByBase = node as BaseExplicitOrderByNode;
            if (explicitOrderByBase != null)
            {
                return this.VisitExplicitOrderByNode(explicitOrderByBase, context);
            }

            var expandNode = node as ExpandNode;
            if (expandNode != null)
            {
                return this.VisitExpandNode(expandNode, context);
            }

            var orderByNode = node as OrderByNode;
            if (orderByNode != null)
            {
                return this.VisitOrderByNode(orderByNode, context);
            }

            var filterNode = node as FilterNode;
            if (filterNode != null)
            {
                return this.VisitFilterNode(filterNode, context);
            }

            var inlineCountNode = node as InlineCountNode;
            if (inlineCountNode != null)
            {
                return this.VisitInlineCountNode(inlineCountNode, context);
            }

            var selectNode = node as SelectNode;
            if (selectNode != null)
            {
                return this.VisitSelectNode(selectNode, context);
            }

            var skipNode = node as SkipNode;
            if (skipNode != null)
            {
                return this.VisitSkipNode(skipNode, context);
            }

            var topNode = node as TopNode;
            if (topNode != null)
            {
                return this.VisitTopNode(topNode, context);
            }
            
            throw new InvalidOperationException($"The node type {node.GetType().Name} is not handled.");
        }

        public virtual TResult VisitIdentifier(BaseIdentifierNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        /// <summary>
        /// Handles Not, Day, Days, Hour, Hours, Minute, Minutes, Month, Second, Seconds, 
        /// ToLower, ToUpper, Year, Years
        /// </summary>
        /// <param name="node"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual TResult VisitSingleChildOperation(SingleChildNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitDateTimeFunction(SingleChildNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitTwoChildrenOperation(TwoChildNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitConstantNode(TreeNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitAggregateNode(TreeNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitOrderByNode(TreeNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitAliasNode(AliasNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitIgnoredNode(IgnoredNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitExplicitOrderByNode(BaseExplicitOrderByNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitExpandNode(ExpandNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitFilterNode(FilterNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitInlineCountNode(InlineCountNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitSelectNode(SelectNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitSkipNode(SkipNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }

        public virtual TResult VisitTopNode(TopNode node, TContext context)
        {
            throw new NotSupportedException("Node is not supported by this visitor.");
        }
    }
}