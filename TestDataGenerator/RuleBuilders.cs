using System;
using System.Collections.Generic;

namespace TestDataGenerator
{
    public abstract class RuleBuilders
    {
        protected static readonly string RANDOM = "random";
        protected static readonly string SEQ = "seq";
        protected static readonly string FIX = "fix";
        protected static readonly string LIST = "list";

        protected readonly IDictionary<string, Type> ruleBuilders = new Dictionary<string, Type>();

        protected RuleBuilders()
        {
            ruleBuilders[FIX] = typeof(FixRule);
            ruleBuilders[LIST] = typeof(ListRule);
        }

        public IRule BuildRule(IDictionary<string, object> config)
        {
            var constructor = ruleBuilders[config["type"].ToString()]?.GetConstructor(Type.EmptyTypes);
            var rule = (IRule)constructor?.Invoke(null);
            rule?.InitConfig(config);
            return rule;
        }

        #region Default Rule

        public class FixRule : Rule<FixRule.FixRuleConfig>
        {
            public override string Calculate()
            {
                return Config.Value.ToString();
            }

            public class FixRuleConfig
            {
                public object Value { get; set; }
            }
        }

        public class ListRule : Rule<ListRule.ListRuleConfig>
        {
            private int index;

            public override string Calculate()
            {
                var v = Config.Items[index];
                index = index + 1;
                if (index >= Config.Items.Count)
                {
                    index = 0;
                }

                return v.ToString();
            }

            public class ListRuleConfig
            {
                public List<object> Items { get; set; }
            }
        }

        #endregion
    }

    public class NumberRuleBuilders : RuleBuilders
    {
        public NumberRuleBuilders()
        {
            ruleBuilders[RANDOM] = typeof(RandomRule);
            ruleBuilders[SEQ] = typeof(SeqRule);
        }

        public class RandomRule : Rule<RandomRule.RandomRuleConfig>
        {
            private readonly Random random = new Random();

            public override string Calculate()
            {
                var begin = Config.Begin ?? 1;
                var end = Config.End ?? 100;
                double dot = Config.Dot ?? 0;

                dot = dot > 15 ? 15 : dot; // 小数位不能大于15位

                var ra = random.NextDouble() * (end - begin) + begin;
                dot = Math.Pow(10, dot);

                return (Math.Round(ra * dot) / dot).ToString();
            }

            public class RandomRuleConfig
            {
                public int? Begin { get; set; }
                public int? Dot { get; set; }
                public int? End { get; set; }
            }
        }

        public class SeqRule : Rule<SeqRule.SeqRuleConfig>
        {
            private int? value;

            public override string Calculate()
            {
                if (value == null)
                {
                    value = Config.Begin;
                }
                else
                {
                    value = value + Config.Step;
                    if (value > Config.End)
                    {
                        value = value - Config.End;
                    }
                }

                return value.ToString();
            }

            public override void InitConfig(IDictionary<string, object> config)
            {
                base.InitConfig(config);

                Config.Begin = Convert.ToInt32(config["begin"]);
                Config.End = Convert.ToInt32(config["end"]);
                Config.Step = Convert.ToInt32(config["step"]);

                if (Config.Begin > Config.End &&
                    Config.Step < 0)
                {
                    var tmp = Config.End;
                    Config.End = Config.Begin;
                    Config.Begin = tmp;
                    Config.Step = -Config.Step;
                }
            }

            public class SeqRuleConfig
            {
                public int? Begin { get; set; }
                public int? End { get; set; }
                public int? Step { get; set; }
            }
        }
    }

    public class StringRuleBuilders : RuleBuilders
    {
        public StringRuleBuilders()
        {
            ruleBuilders[RANDOM] = typeof(StringRandomRule);
            ruleBuilders[SEQ] = typeof(StringSeqRule);
            ruleBuilders[LIST] = typeof(StringListRule);
            ruleBuilders[FIX] = typeof(StringFixRule);
        }

        public class StringRandomRule : Rule<StringRandomRule.RuleConfig>
        {
            private readonly Random random = new Random();

            public override string Calculate()
            {
                var seed = Config.Seed;
                var s = new char[Config.Length];
                for (var i = 0; i < Config.Length; i++)
                {
                    var next = random.Next(0, seed.Length - 1);
                    s[i] = seed[next];
                }
                return "'" + new string(s) + "'";
            }

            public class RuleConfig
            {
                public const string DEFAULT_SEED =
                    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

                public int Length { get; set; }
                public string Seed { get; set; } = DEFAULT_SEED;
            }
        }

        public class StringFixRule : FixRule
        {
            public override string Calculate()
            {
                return "'" + base.Calculate() + "'";
            }
        }

        public class StringSeqRule : Rule<StringSeqRule.RuleConfig>
        {
            public override string Calculate()
            {
                throw new NotImplementedException();
            }

            public class RuleConfig
            {
            }
        }

        public class StringListRule : ListRule
        {
            public override string Calculate()
            {
                return "'" + base.Calculate() + "'";
            }
        }
    }

    public class DateRuleBuilders : RuleBuilders
    {
        public DateRuleBuilders()
        {
        }
    }

    public class DateTimeRuleBuilders : RuleBuilders
    {
        public DateTimeRuleBuilders()
        {
        }
    }
}