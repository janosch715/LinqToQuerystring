namespace LinqToQuerystring.TreeNodes.Base
{
    using System;

    using Antlr.Runtime;

    public abstract class ExplicitOrderByBase : TreeNode
    {
        protected ExplicitOrderByBase(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
            this.IsFirstChild = false;
        }

        public bool IsFirstChild { get; set; }
    }
}