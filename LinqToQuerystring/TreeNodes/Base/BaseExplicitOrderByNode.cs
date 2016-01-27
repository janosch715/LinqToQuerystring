namespace LinqToQuerystring.TreeNodes.Base
{
    using System;

    using Antlr.Runtime;

    public abstract class BaseExplicitOrderByNode : TreeNode
    {
        protected BaseExplicitOrderByNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }
    }
}