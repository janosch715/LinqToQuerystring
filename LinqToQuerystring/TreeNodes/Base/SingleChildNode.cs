namespace LinqToQuerystring.TreeNodes.Base
{
    using System;
    using System.Linq;

    using Antlr.Runtime;

    public abstract class SingleChildNode : TreeNode
    {
        protected SingleChildNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public TreeNode ChildNode
            => this.ChildNodes.FirstOrDefault();
    }
}