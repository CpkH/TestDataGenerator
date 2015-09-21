using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestDataGenerator
{
    public class Generator
    {
        private readonly Configuration configuration;
        private readonly List<string> fields = new List<string>();

        public Generator(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Generate()
        {
            int? times = configuration.Times;

            foreach (var field in configuration.Fields)
            {
                fields.Add(field.Name);

                if (configuration.Times == null)
                {
                    if (field.Rule is RuleBuilders.ListRule)
                    {
                        var rule = (RuleBuilders.ListRule)field.Rule;
                        var count = rule.Config.Items.Count;
                        times = times == 0 ? count : times * count;
                    }
                }
            }

            using (var f = File.CreateText(configuration.Table + ".sql"))
            {
                for (int i = 1; i <= times; i++)
                {
                    var sql = new StringBuilder();
                    var values = new List<string>();

                    foreach (var field in configuration.Fields)
                    {
                        values.Add(field.Rule.Calculate());
                    }

                    sql.AppendFormat("/* times: {0} */ ", i)
                        .AppendFormat("INSERT INTO {0}", configuration.Table)
                        .AppendFormat(" ({0}) ", string.Join(", ", fields))
                        .AppendFormat("VALUES({0});", string.Join(", ", values));

                    f.WriteLine(sql);
                    Console.WriteLine(sql);
                }
            }
        }
    }
}