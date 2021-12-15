using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TransformsAI.Unity.Utilities.Editor
{

    internal static class PropertyHandler
    {
        private static readonly Func<SerializedProperty, object> GetHandler;
        private static readonly MethodInfo OnGUIMethodInfo;
        private static readonly MethodInfo GetHeightMethodInfo;

        static PropertyHandler()
        {
            var getHandlerMethodInfo = Type.GetType("UnityEditor.ScriptAttributeUtility, UnityEditor")
                ?.GetMethod("GetHandler", BindingFlags.NonPublic | BindingFlags.Static);

            if (getHandlerMethodInfo == null) throw new Exception("Reflection failed on PropertyHandler");
            var handlerType = getHandlerMethodInfo.ReturnType;
            GetHandler =
                (Func<SerializedProperty, object>)getHandlerMethodInfo.CreateDelegate(
                    typeof(Func<SerializedProperty, object>));

            var myGuiMethod =
                typeof(PropertyHandler).GetMethod(nameof(OnGUI), BindingFlags.Public | BindingFlags.Static);
            if (myGuiMethod == null) throw new Exception("Reflection failed on PropertyHandler");
            var guiParamTypes = myGuiMethod.GetParameters().Select(it => it.ParameterType).ToArray();
            OnGUIMethodInfo = handlerType.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Instance, null,
                guiParamTypes, null);

            var myHeightMethod =
                typeof(PropertyHandler).GetMethod(nameof(GetHeight), BindingFlags.Public | BindingFlags.Static);
            if (myHeightMethod == null) throw new Exception("Reflection failed on PropertyHandler");
            var heightParamTypes = myHeightMethod.GetParameters().Select(it => it.ParameterType).ToArray();
            GetHeightMethodInfo = handlerType.GetMethod("GetHeight", BindingFlags.Public | BindingFlags.Instance, null,
                heightParamTypes, null);
        }

        private static readonly object[] OnGuiParameters = new object[4];
        private static readonly object[] GetHeightParameters = new object[3];

        public static bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            var handlerObj = GetHandler(property);

            OnGuiParameters[0] = position;
            OnGuiParameters[1] = property;
            OnGuiParameters[2] = label;
            OnGuiParameters[3] = includeChildren;

            return (bool)OnGUIMethodInfo.Invoke(handlerObj, OnGuiParameters);
        }

        public static float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            var handlerObj = GetHandler(property);

            GetHeightParameters[0] = property;
            GetHeightParameters[1] = label;
            GetHeightParameters[2] = includeChildren;

            return (float)GetHeightMethodInfo.Invoke(handlerObj, GetHeightParameters);
        }

    }
}