using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestDataGenerator
{
    public interface IRule
    {
        object Config { get; }

        void InitConfig(IDictionary<string, object> config);

        string Calculate();
    }

    public abstract class Rule<T> : IRule
        where T : new()
    {
        public T Config { get; private set; }

        object IRule.Config => Config;

        public abstract string Calculate();

        public virtual void InitConfig(IDictionary<string, object> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var type = typeof(T);
            Config = new T();
            foreach (var info in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (info.CanWrite == false)
                {
                    continue;
                }
                var propertyType = info.PropertyType;
                if (propertyType.IsGenericType &&
                    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propertyType = propertyType.GetGenericArguments()[0];
                }
                foreach (var item in config)
                {
                    if (info.Name.ToLower() == item.Key.ToLower())
                    {
                        var value = item.Value;
                        if (!(item.Value is IList<object>))
                        {
                            value = Convert.ChangeType(item.Value, propertyType);
                        }
                        info.SetValue(Config, value, null);
                        break;
                    }
                }
            }
        }
    }
}