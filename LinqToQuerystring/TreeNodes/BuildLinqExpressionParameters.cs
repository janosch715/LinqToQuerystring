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
        public BuildLinqExpressionParameters(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            this.Query = query;
            this.InputType = inputType;
            this.Expression = expression;
            this.Item = item;
        }

        public IQueryable Query { get; set; }

        public Type InputType { get; }

        public Expression Expression { get; }

        public Expression Item { get; }
    }
}