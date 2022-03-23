using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace TransformsAI.Unity.Utilities.Editor
{
    public static class ReflectionUtils
    {
        private static readonly Regex IndexMatcher = new Regex("^([^\\s\\[]+)\\[([0-9]+)\\]");

        private static readonly Dictionary<string, Func<object, object>> GetterCache = new Dictionary<string, Func<object, object>>();

        
        public static object ReflectedValue(this SerializedProperty prop)
        {
            var getter = MakeGetter(prop);
            var target = prop.serializedObject.targetObject;
            return getter(target);
        }

        public static object ContainerObject(this SerializedProperty prop)
        {
            var getter = MakeGetter(prop, true);
            var target = prop.serializedObject.targetObject;
            return getter(target);
        }

        public static Func<object, object> MakeGetter(SerializedProperty prop, bool getParent = false)
        {
            object obj = prop.serializedObject.targetObject;
            if (obj == null) throw new Exception("Target object is null!");

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');

            if (getParent)
            {
                elements = elements.Reverse().Skip(1).Reverse().ToArray();
                path = string.Join(".", elements);
            }

            if (elements.Length == 0) return so => so;

            var type = obj.GetType();
            var tag = $"{type.FullName}:{path}";

            if (GetterCache.TryGetValue(tag, out var cachedGetter)) return cachedGetter;

            Func<object, object> getter = null;
            foreach (var element in elements)
            {
                var newGetter = MakeGetter(type, element, out type);
                // save old getter to capture in lambda
                var oldGetter = getter;
                getter = getter == null ? newGetter : source => newGetter(oldGetter(source));
            }

            GetterCache[tag] = getter;
            return getter;
        }

        private static Func<object, object> MakeGetter(Type type, string element, out Type innerType)
        {
            var indexMatch = IndexMatcher.Match(element);
            Func<object, object> newGetter;
            if (indexMatch.Success)
            {
                var elementName = indexMatch.Groups[1].Value;
                var index = int.Parse(indexMatch.Groups[2].Value);
                newGetter = MakeListGetter(type, elementName, index, out innerType);
            }
            else
            {
                newGetter = MakeObjectGetter(type, element, out innerType);
            }

            return newGetter;
        }

        private static Func<object, object> MakeObjectGetter(Type type, string name, out Type innerType)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (f == null) throw new Exception($"Could not Make getter for {type.Name}.{name}");

            innerType = f.FieldType;
            return source => f.GetValue(source);
        }

        private static Func<object, object> MakeListGetter(Type type, string name, int index, out Type innerType)
        {
            var listGetter = MakeGetter(type, name, out var listType);
            innerType = listType.GenericTypeArguments[0];
            return obj =>
            {
                var list = (IList) listGetter(obj);
                if (index > list.Count) return null;
                return list[index];
            };

        }
    }
}