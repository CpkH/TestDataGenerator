using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using SimpleJson;
using SimpleJson.Reflection;

namespace TestDataGenerator
{
    public class Configuration
    {
        public string Table { get; set; }

        public int? Times { get; set; }

        public List<Field> Fields { get; set; }

        public static Configuration Parse(string configFile)
        {
            var configContent = File.ReadAllText(configFile);
            return SimpleJson.SimpleJson.DeserializeObject<Configuration>(
                configContent,
                new JsonSerializerStrategy());
        }

        private class JsonSerializerStrategy : PocoJsonSerializerStrategy
        {
            // convert string to int
            public override object DeserializeObject(object value, Type type)
            {
                if (type == typeof(int) &&
                    value is string)
                {
                    return int.Parse(value.ToString());
                }
                if (value is IDictionary<string, object>)
                {
                    var jsonObject = (IDictionary<string, object>)value;

                    if (type == typeof(Field))
                    {
                        var instance = (Field)CacheResolver.GetNewInstance(type);
                        var maps = CacheResolver.LoadMaps(type);
                        foreach (var keyValuePair in maps)
                        {
                            var v = keyValuePair.Value;
                            if (v.Setter == null)
                            {
                                continue;
                            }

                            var jsonKey = keyValuePair.Key;
                            if (jsonKey == "rule")
                            {
                                continue;
                            }
                            if (jsonObject.ContainsKey(jsonKey))
                            {
                                var jsonValue = DeserializeObject(jsonObject[jsonKey], v.Type);
                                v.Setter(instance, jsonValue);
                            }
                        }

                        var rules = Types.Instance[instance.Type];
                        instance.Rule = rules.BuildRule((IDictionary<string, object>)jsonObject["rule"]);
                        return instance;
                    }
                }
                return base.DeserializeObject(value, type);
            }

            protected override void BuildMap(
                Type type,
                SafeDictionary<string, CacheResolver.MemberMap> memberMaps)
            {
                foreach (var info in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    memberMaps.Add(info.Name.ToLower(), new CacheResolver.MemberMap(info));
                }
                foreach (var info in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    memberMaps.Add(info.Name.ToLower(), new CacheResolver.MemberMap(info));
                }
            }
        }
    }

    public class Field
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public IRule Rule { get; set; }
    }
}