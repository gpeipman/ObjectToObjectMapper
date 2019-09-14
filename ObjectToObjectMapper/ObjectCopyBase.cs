using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectToObjectMapper
{
    public abstract class ObjectCopyBase
    {
        public abstract void MapTypes(Type source, Type target);
        public abstract void Copy(object source, object target);

        protected virtual IList<PropertyMap> GetMatchingProperties(Type sourceType, Type targetType)
        {
            var sourceProperties = sourceType.GetProperties();
            var targetProperties = targetType.GetProperties();

            var properties = (from s in sourceProperties
                              from t in targetProperties
                              where s.Name == t.Name &&
                                    s.CanRead &&
                                    t.CanWrite &&
                                    s.PropertyType == t.PropertyType
                              select new PropertyMap
                              {
                                  SourceProperty = s,
                                  TargetProperty = t
                              }).ToList();
            return properties;
        }

        protected virtual string GetMapKey(Type sourceType, Type targetType)
        {
            var keyName = "Copy_";
            keyName += sourceType.FullName.Replace(".", "_").Replace("+", "_");
            keyName += "_";
            keyName += targetType.FullName.Replace(".", "_").Replace("+", "_");

            return keyName;
        }
    }
}