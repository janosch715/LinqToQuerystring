namespace LinqToQuerystring
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Antlr.Runtime;
    using Antlr.Runtime.Tree;

    using LinqToQuerystring.TreeNodes;
    using LinqToQuerystring.TreeNodes.Base;
    using Visitor;

    public static class Extensions
    {
        private static readonly LingToQueryRootNodesVisitor LingToQueryRootNodesVisitor = 
            new LingToQueryRootNodesVisitor();

        private static readonly  LingToQueryOrderByNodeVisitor LingToQueryOrderByNodeVisitor = 
            new LingToQueryOrderByNodeVisitor(LingToQueryRootNodesVisitor);

        public static TResult LinqToQuerystring<T, TResult>(this IQueryable<T> query, string queryString = "", bool forceDynamicProperties = false, int maxPageSize = -1)
        {
            return (TResult)LinqToQuerystring(query, typeof(T), queryString, forceDynamicProperties, maxPageSize);
        }

        public static IQueryable<T> LinqToQuerystring<T>(this IQueryable<T> query, string queryString = "", bool forceDynamicProperties = false, int maxPageSize = -1, BuildLinqExpressionConfiguration configuration = null)
        {
            return (IQueryable<T>)LinqToQuerystring(query, typeof(T), queryString, forceDynamicProperties, maxPageSize, configuration);
        }

        public static object LinqToQuerystring(this IQueryable query, Type inputType, string queryString = "", bool forceDynamicProperties = false, int maxPageSize = -1, BuildLinqExpressionConfiguration configuration = null)
        {
            var queryResult = query;
            var constrainedQuery = query;
            var expressionConfiguration = configuration ?? new BuildLinqExpressionConfiguration();

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            if (queryString.StartsWith("?"))
            {
                queryString = queryString.Substring(1);
            }

            var odataQueries = queryString.Split('&').Where(o => o.StartsWith("$")).ToList();
            if (maxPageSize > 0)
            {
                var top = odataQueries.FirstOrDefault(o => o.StartsWith("$top"));
                if (top != null)
                {
                    int pagesize;
                    if (!int.TryParse(top.Split('=')[1], out pagesize) || pagesize >= maxPageSize)
                    {
                        odataQueries.Remove(top);
                        odataQueries.Add("$top=" + maxPageSize);
                    }
                }
                else
                {
                    odataQueries.Add("$top=" + maxPageSize);
                }
            }

            var odataQuerystring = Uri.UnescapeDataString(string.Join("&", odataQueries.ToArray()));

            var input = new ANTLRReaderStream(new StringReader(odataQuerystring));
            var lexer = new LinqToQuerystringLexer(input);
            var tokStream = new CommonTokenStream(lexer);

            var parser = new LinqToQuerystringParser(tokStream) { TreeAdaptor = new TreeNodeFactory(forceDynamicProperties) };

            var result = parser.prog();

            var singleNode = result.Tree as TreeNode;
            if (singleNode != null && !(singleNode is IdentifierNode))
            {
                if (!(singleNode is SelectNode) && !(singleNode is InlineCountNode))
                {
                    BuildQuery(singleNode, inputType, ref queryResult, ref constrainedQuery, expressionConfiguration);
                    return constrainedQuery;
                }

                if (singleNode is SelectNode)
                {
                    return ProjectQuery(constrainedQuery, inputType, singleNode, expressionConfiguration);
                }

                return PackageResults(queryResult, constrainedQuery);
            }

            var tree = result.Tree as CommonTree;
            if (tree != null)
            {
                var children = tree.Children.Cast<TreeNode>().ToList();
                children.Sort();

                // These should always come first
                foreach (var node in children.Where(o => !(o is SelectNode) && !(o is InlineCountNode)))
                {
                    BuildQuery(node, inputType, ref queryResult, ref constrainedQuery, expressionConfiguration);
                }

                var selectNode = children.FirstOrDefault(o => o is SelectNode);
                if (selectNode != null)
                {
                    constrainedQuery = ProjectQuery(constrainedQuery, inputType, selectNode, expressionConfiguration);
                }

                var inlineCountNode = children.FirstOrDefault(o => o is InlineCountNode);
                if (inlineCountNode != null)
                {
                    return PackageResults(queryResult, constrainedQuery);
                }
            }

            return constrainedQuery;
        }

        private static void BuildQuery(TreeNode node, Type inputType, ref IQueryable queryResult, ref IQueryable constrainedQuery, BuildLinqExpressionConfiguration configuration)
        {
            var type = queryResult.Provider.GetType().Name;

            var mappings = (!string.IsNullOrEmpty(type) && Configuration.CustomNodes.ContainsKey(type))
                               ? Configuration.CustomNodes[type]
                               : null;

            if (mappings != null)
            {
                node = mappings.MapNode(node, inputType, queryResult.Expression);
            }

            if (!(node is TopNode) && !(node is SkipNode))
            {
                var modifier = node as QueryModifier;
                if (modifier != null)
                {
                    var newBuildLinqExpressionParameters =
                        new BuildLinqExpressionParameters(
                            configuration,
                            queryResult,
                            inputType,
                            null,
                            null);

                    queryResult = LingToQueryOrderByNodeVisitor.Visit(modifier, newBuildLinqExpressionParameters);
                }
                else
                {
                    var newBuildLinqExpressionParameters =
                        new BuildLinqExpressionParameters(
                            configuration, 
                            queryResult,
                            inputType,
                            queryResult.Expression,
                            null);

                    queryResult = 
                        queryResult.Provider.CreateQuery(
                            LingToQueryRootNodesVisitor.Visit(node, newBuildLinqExpressionParameters));
                }
            }

            var queryModifier = node as QueryModifier;
            if (queryModifier != null)
            {
                var newBuildLinqExpressionParameters =
                    new BuildLinqExpressionParameters(
                        configuration,
                        constrainedQuery,
                        inputType,
                        null,
                        null);

                constrainedQuery = LingToQueryOrderByNodeVisitor.Visit(queryModifier, newBuildLinqExpressionParameters);
            }
            else
            {
                var newBuildLinqExpressionParameters =
                        new BuildLinqExpressionParameters(
                            configuration,
                            constrainedQuery,
                            inputType,
                            constrainedQuery.Expression,
                            null);

                constrainedQuery =
                    constrainedQuery.Provider.CreateQuery(
                        LingToQueryRootNodesVisitor.Visit(node, newBuildLinqExpressionParameters));
            }
        }

        private static IQueryable ProjectQuery(IQueryable constrainedQuery, Type inputType, TreeNode node, BuildLinqExpressionConfiguration configuration)
        {
            // TODO: Find a solution to the following:
            // Currently the only way to perform the SELECT part of the query is to call ToList and then project onto a dictionary. Two main problems:
            // 1. Linq to Entities does not support projection onto list initialisers with more than one value
            // 2. We cannot build an anonymous type using expression trees as there is compiler magic that must happen.
            // There is a solution involving reflection.emit, but is it worth it? Not sure...

            var result = constrainedQuery.GetEnumeratedQuery().AsQueryable();

            var newBuildLinqExpressionParameters =
                new BuildLinqExpressionParameters(
                    configuration,
                    result,
                    inputType,
                    result.Expression,
                    null);

            return result.Provider.CreateQuery<Dictionary<string, object>>(
                LingToQueryRootNodesVisitor.Visit(node, newBuildLinqExpressionParameters));
        }

        private static object PackageResults(IQueryable query, IQueryable constrainedQuery)
        {
            var result = query.GetEnumeratedQuery();
            return new Dictionary<string, object> { { "Count", result.Count() }, { "Results", constrainedQuery } };
        }

        public static IEnumerable<object> GetEnumeratedQuery(this IQueryable query)
        {
            return Iterate(query.GetEnumerator()).Cast<object>().ToList();
        }

        static IEnumerable Iterate(this IEnumerator iterator)
        {
            while (iterator.MoveNext())
                yield return iterator.Current;
        }
    }
}