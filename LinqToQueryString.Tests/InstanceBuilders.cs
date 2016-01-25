namespace LinqToQueryString.Tests
{
    using System;

    public static class InstanceBuilders
    {
        public static ConcreteClass BuildConcrete(string name, int age, DateTime date, bool complete)
        {
            return new ConcreteClass { Name = name, Date = date, Age = age, Complete = complete };
        }

        public static EdgeCaseClass BuildEdgeCase(string name, int age, DateTime date, bool complete)
        {
            return new EdgeCaseClass { Name = name, Date = date, Age = age, Complete = complete };
        }

        public static ConcreteClass BuildConcrete(string name, int age, DateTime date, bool complete, long population, double value, float cost, byte code, decimal score, Guid guid, string edgeCaseClassString = null)
        {
            return new ConcreteClass
            {
                Name = name,
                Date = date,
                Age = age,
                Complete = complete,
                Population = population,
                Value = value,
                Cost = cost,
                Code = code,
                Score = score,
                Guid = guid,
                EdgeCaseClass = edgeCaseClassString == null ? new EdgeCaseClass() : new EdgeCaseClass()
                {
                    Name = edgeCaseClassString
                }
            };
        }

        public static NullableClass BuildNull()
        {
            return new NullableClass();
        }

        public static NullableClass BuildNull(int? age, DateTime? date, bool? complete, long? population, double? value, float? cost, byte? code, Guid? guid)
        {
            return new NullableClass { Date = date, Age = age, Complete = complete, Population = population, Value = value, Cost = cost, Code = code, Guid = guid };
        }
    }
}