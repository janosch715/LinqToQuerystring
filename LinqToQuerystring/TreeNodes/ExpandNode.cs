namespace LinqToQuerystring.TreeNodes
{
    using System;
    using System.Linq;

    using Antlr.Runtime;

    using LinqToQuerystring.TreeNodes.Base;

    public class ExpandNode : QueryModifier
    {
        public ExpandNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public override IQueryable ModifyQuery(BuildLinqExpressionParameters buildLinqExpressionParameters)
        {
            throw new NotSupportedException("The Expand query option is not supported by this provder");
        }

        public override int CompareTo(TreeNode other)
        {
            if (other is ExpandNode)
            {
                return 0;
            }

            return -1;
        }
    }
}