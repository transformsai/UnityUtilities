using System;
using System.Reflection;

namespace TransformsAI.Unity.Utilities
{
    public static class ReflectionUtils
    {
        public static T GetFieldOrPropertyValue<T>(this Type sourceType, string propName, object instance = null)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var prop = sourceType.GetProperty(propName, bindingFlags);

            if (prop != null)
            {
                if (prop.PropertyType != typeof(T))
                    throw new Exception($"Property {propName} in {sourceType} should be of type {typeof(T).Name}");

                if (prop.GetMethod.IsStatic) return (T)prop.GetValue(null);

                if (instance == null) throw new Exception("Tried to access instance property without instance");
                
                return (T)prop.GetValue(instance);

            }

            var field = sourceType.GetField(propName, bindingFlags);

            if (field != null)
            {
                if (field.FieldType != typeof(T))
                    throw new Exception($"Field {propName} in {sourceType} should be of type {typeof(T).Name}");
                
                if (field.IsStatic) return (T)field.GetValue(null);

                if (instance == null) throw new Exception("Tried to access instance field without instance");
                
                return (T)field.GetValue(instance);
            }

            throw new Exception($"Missing property {propName} in {sourceType}");
        }

    }
}
