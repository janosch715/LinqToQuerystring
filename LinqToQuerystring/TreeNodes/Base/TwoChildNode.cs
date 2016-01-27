namespace LinqToQuerystring.TreeNodes.Base
{
    using System;
    using System.Linq;

    using Antlr.Runtime;

    public abstract class TwoChildNode : TreeNode
    {
        protected TwoChildNode(IToken payload, TreeNodeFactory treeNodeFactory)
            : base(payload, treeNodeFactory)
        {
        }

        public TreeNode LeftNode
            => this.ChildNodes.ElementAtOrDefault(0);

        public TreeNode RightNode
            => this.ChildNodes.ElementAtOrDefault(1);
    }
}