// -----------------------------------------------------------------------
//  <copyright file="BuildLinqExpressionParameters.cs" company="j-consulting GmbH">
//      Copyright (c) j-consulting GmbH. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class BuildLinqExpressionParameters
    {
        public BuildLinqExpressionParameters(
            BuildLinqExpressionConfiguration configuration, 
            IQueryable query, 
            Type inputType, 
            Expression expression,
            Expression item,
            bool isFirstOrderBy = false)
        {
            this.Configuration = configuration;
            this.Query = query;
            this.InputType = inputType;
            this.Expression = expression;
            this.Item = item;
            this.IsFirstOrderBy = isFirstOrderBy;
        }

        public bool IsFirstOrderBy { get; }

        public IQueryable Query { get; }

        public Type InputType { get; }

        public Expression Expression { get; }

        public Expression Item { get; }

        public BuildLinqExpressionConfiguration Configuration { get; }

        public BuildLinqExpressionParameters ChangeItem(Expression newItem)
        {
            return new BuildLinqExpressionParameters(
                this.Configuration,
                this.Query,
                this.InputType,
                this.Expression,
                newItem);
        }
    }
}