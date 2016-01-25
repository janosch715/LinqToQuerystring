namespace LinqToQuerystring.TreeNodes.Functions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class ToUpperNode : SingleChildNode
    {
        public ToUpperNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override Expression BuildLinqExpression(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            var childexpression = this.ChildNode.BuildLinqExpression(buildLinqExpressionParameters);

            if (!typeof(string).IsAssignableFrom(childexpression.Type))
            {
                childexpression = Expression.Convert(childexpression, typeof(string));
            }
            
            return Expression.Call(childexpression, "ToUpper", null, null);
        }
    }
}