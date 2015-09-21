using System.Collections.Generic;

namespace TestDataGenerator
{
    public class Types
    {
        private static readonly string NUMBER = "number";
        private static readonly string STRING = "string";
        private static readonly string DATE = "date";
        private static readonly string DATETIME = "datetime";

        private readonly IDictionary<string, RuleBuilders> typeMap = new Dictionary<string, RuleBuilders>()
        {
            [NUMBER] = new NumberRuleBuilders(),
            [STRING] = new StringRuleBuilders(),
            [DATE] = new DateRuleBuilders(),
            [DATETIME] = new DateTimeRuleBuilders()
        };

        private Types()
        {
        }

        public static Types Instance => new Types();

        public RuleBuilders this[string index] => typeMap[index];
    }
}