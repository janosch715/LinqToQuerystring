// -----------------------------------------------------------------------
//  <copyright file="BuildLinqExpressionConfiguration.cs" company="j-consulting GmbH">
//      Copyright (c) j-consulting GmbH. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace LinqToQuerystring.TreeNodes
{
    public class BuildLinqExpressionConfiguration
    {
        public BuildLinqExpressionConfiguration()
            : this(false)
        {
        }
        
        public BuildLinqExpressionConfiguration(bool useNullForInvalidIdentifiers)
        {
            this.UseNullForInvalidIdentifiers = useNullForInvalidIdentifiers;
        }

        public bool UseNullForInvalidIdentifiers { get; }
    }
}