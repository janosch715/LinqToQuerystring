﻿namespace LinqToQuerystring.TreeNodes
{
    using Antlr.Runtime.Tree;

    using Aggregates;
    using Comparisons;
    using DataTypes;
    using Functions;

    public class TreeNodeFactory : CommonTreeAdaptor
    {
        private readonly bool forceDynamicProperties;

        public TreeNodeFactory(bool forceDynamicProperties)
        {
            this.forceDynamicProperties = forceDynamicProperties;
        }

        public override object Create(Antlr.Runtime.IToken token)
        {
            if (token == null)
            {
                return new CommonTree();
            }

            switch (token.Type)
            {
                case LinqToQuerystringLexer.TOP:
                    return new TopNode(token, this);
                case LinqToQuerystringLexer.SKIP:
                    return new SkipNode(token, this);
                case LinqToQuerystringLexer.ORDERBY:
                    return new OrderByNode(token, this);
                case LinqToQuerystringLexer.FILTER:
                    return new FilterNode(token, this);
                case LinqToQuerystringLexer.SELECT:
                    return new SelectNode(token, this);
                case LinqToQuerystringLexer.INLINECOUNT:
                    return new InlineCountNode(token, this);
                case LinqToQuerystringLexer.EXPAND:
                    return new ExpandNode(token, this);
                case LinqToQuerystringLexer.NOT:
                    return new NotNode(token, this);
                case LinqToQuerystringLexer.AND:
                    return new AndNode(token, this);
                case LinqToQuerystringLexer.OR:
                    return new OrNode(token, this);
                case LinqToQuerystringLexer.EQUALS:
                    return new EqualsNode(token, this);
                case LinqToQuerystringLexer.NOTEQUALS:
                    return new NotEqualsNode(token, this);
                case LinqToQuerystringLexer.GREATERTHAN:
                    return new GreaterThanNode(token, this);
                case LinqToQuerystringLexer.GREATERTHANOREQUAL:
                    return new GreaterThanOrEqualNode(token, this);
                case LinqToQuerystringLexer.LESSTHAN:
                    return new LessThanNode(token, this);
                case LinqToQuerystringLexer.LESSTHANOREQUAL:
                    return new LessThanOrEqualNode(token, this);
                case LinqToQuerystringLexer.STARTSWITH:
                    return new StartsWithNode(token, this);
                case LinqToQuerystringLexer.ENDSWITH:
                    return new EndsWithNode(token, this);
                case LinqToQuerystringLexer.CONTAINS:
                case LinqToQuerystringLexer.SUBSTRINGOF:
                    return new SubstringOfNode(token, this);
                case LinqToQuerystringLexer.TOLOWER:
                    return new ToLowerNode(token, this);
                case LinqToQuerystringLexer.TOUPPER:
                    return new ToUpperNode(token, this);
                case LinqToQuerystringLexer.YEAR:
                    return new YearNode(token, this);
                case LinqToQuerystringLexer.YEARS:
                    return new YearsNode(token, this);
                case LinqToQuerystringLexer.MONTH:
                    return new MonthNode(token, this);
                case LinqToQuerystringLexer.DAY:
                    return new DayNode(token, this);
                case LinqToQuerystringLexer.DAYS:
                    return new DaysNode(token, this);
                case LinqToQuerystringLexer.HOUR:
                    return new HourNode(token, this);
                case LinqToQuerystringLexer.HOURS:
                    return new HoursNode(token, this);
                case LinqToQuerystringLexer.MINUTE:
                    return new MinuteNode(token, this);
                case LinqToQuerystringLexer.MINUTES:
                    return new MinutesNode(token, this);
                case LinqToQuerystringLexer.SECOND:
                    return new SecondNode(token, this);
                case LinqToQuerystringLexer.SECONDS:
                    return new SecondsNode(token, this);
                case LinqToQuerystringLexer.ANY:
                    return new AnyNode(token, this);
                case LinqToQuerystringLexer.ALL:
                    return new AllNode(token, this);
                case LinqToQuerystringLexer.COUNT:
                    return new CountNode(token, this);
                case LinqToQuerystringLexer.AVERAGE:
                    return new AverageNode(token, this);
                case LinqToQuerystringLexer.MAX:
                    return new MaxNode(token, this);
                case LinqToQuerystringLexer.MIN:
                    return new MinNode(token, this);
                case LinqToQuerystringLexer.SUM:
                    return new SumNode(token, this);
                case LinqToQuerystringLexer.ALIAS:
                    return new AliasNode(token, this);
                case LinqToQuerystringLexer.DYNAMICIDENTIFIER:
                    return new DynamicIdentifierNode(token, this);
                case LinqToQuerystringLexer.IDENTIFIER:
                    if (this.forceDynamicProperties)
                    {
                        return new DynamicIdentifierNode(token, this);
                    }
                    return new IdentifierNode(token, this);
                case LinqToQuerystringLexer.STRING:
                    return new StringNode(token, this);
                case LinqToQuerystringLexer.BOOL:
                    return new BoolNode(token, this);
                case LinqToQuerystringLexer.INT:
                    return new IntNode(token, this);
                case LinqToQuerystringLexer.DATETIME:
                    return new DateTimeNode(token, this);
                case LinqToQuerystringLexer.DOUBLE:
                    return new DoubleNode(token, this);
                case LinqToQuerystringLexer.SINGLE:
                    return new SingleNode(token, this);
                case LinqToQuerystringLexer.DECIMAL:
                    return new DecimalNode(token, this);
                case LinqToQuerystringLexer.LONG:
                    return new LongNode(token, this);
                case LinqToQuerystringLexer.BYTE:
                    return new ByteNode(token, this);
                case LinqToQuerystringLexer.GUID:
                    return new GuidNode(token, this);
                case LinqToQuerystringLexer.DESC:
                    return new DescNode(token, this);
                case LinqToQuerystringLexer.ASC:
                    return new AscNode(token, this);
                case LinqToQuerystringLexer.NULL:
                    return new NullNode(token, this);
                case LinqToQuerystringLexer.IGNORED:
                    return new IgnoredNode(token, this);
            }

            return null;
        }
    }
}