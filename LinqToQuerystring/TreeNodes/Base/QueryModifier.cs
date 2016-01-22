using System;

namespace LinqToQuerystring.TreeNodes.Base
{
    using System.Linq;
    using System.Linq.Expressions;
    using Antlr.Runtime;

    public abstract class QueryModifier : TreeNode
    {
        protected QueryModifier(Type inputType, IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(IQueryable query, Type inputType, Expression expression, Expression item)
        {
            throw new NotSupportedException(
               string.Format("{0} is just a placeholder and should be handled differently in Extensions.cs", this.GetType().Name));
        }

        public abstract IQueryable ModifyQuery(IQueryable query, Type inputType);
    }
}
