using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectToObjectMapper
{
    public static class ObjectExtensions
    {
        public static IList<PropertyMap> GetMatchingProperties(this object source, object target)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            var sourceProperties = source.GetType().GetProperties();
            var targetProperties = target.GetType().GetProperties();

            var properties = (from s in sourceProperties
                              from t in targetProperties
                              where s.Name == t.Name &&
                                  s.CanRead &&
                                  t.CanWrite &&
                                  s.PropertyType.IsPublic &&
                                  t.PropertyType.IsPublic &&
                                  s.PropertyType == t.PropertyType &&
                                  (
                                      (s.PropertyType.IsValueType &&
                                      t.PropertyType.IsValueType
                                      ) ||
                                      (s.PropertyType == typeof(string) &&
                                      t.PropertyType == typeof(string)
                                      )
                                  )
                              select new PropertyMap
                              {
                                  SourceProperty = s,
                                  TargetProperty = t
                              }).ToList();

            return properties;
        }
    }
}