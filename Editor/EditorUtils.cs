using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorUtils
{

    public static string Tooltip;
    public static GUIStyle TooltipStyle;
    public static float PaddingSize = 1;
    public static float IndentSize = 10;
    public static readonly float SingleLineHeight = EditorGUIUtility.singleLineHeight + 2 * PaddingSize;

    public static string DrawScriptField(Editor editor)
    {
        var mScript = editor.serializedObject.FindProperty("m_Script");
        using var x = new EditorGUI.DisabledGroupScope(true);
        EditorGUILayout.PropertyField(mScript);
        return "m_Script";
    }

    public static void DrawTooltip()
    {

        if (string.IsNullOrEmpty(Tooltip)) return;

        if (TooltipStyle == null) TooltipStyle = GUI.skin.FindStyle("Tooltip");
        if (TooltipStyle == null) TooltipStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("Tooltip");

        var size = TooltipStyle.CalcSize(new GUIContent(Tooltip));
        var ttRect = new Rect(Event.current.mousePosition, size);
        ttRect.y -= ttRect.height;
        GUI.Box(ttRect, Tooltip, TooltipStyle);
        Tooltip = null;
    }

    public static void AddTooltip(this Rect rect, string toolTip)
    {
        if (rect.Contains(Event.current.mousePosition)) Tooltip = toolTip;
    }

    public static Rect Offset(this Rect rect, float height)
    {
        var ret = new Rect(rect);
        ret.y += height + PaddingSize;
        ret.height = SingleLineHeight - 2 * PaddingSize;
        return ret;
    }

    public static Rect Split(this Rect rect, float height, out Rect remainder)
    {
        var ret = rect;
        ret.height = height;
        remainder = rect;
        remainder.yMin += height;
        return ret;
    }


    public static Rect Indent(this Rect rect, float? indent = null)
    {
        rect.xMin += indent ?? IndentSize;
        return rect;
    }


    public static Rect Move(this Rect rect, float x = 0, float y = 0)
    {
        rect.x += x;
        rect.y += y;
        return rect;
    }



    private static class PropertyHandler
    {
        private static readonly Func<SerializedProperty, object> GetHandler;
        private static readonly MethodInfo OnGUIMethodInfo;
        private static readonly MethodInfo GetHeightMethodInfo;

        static PropertyHandler()
        {
            var getHandlerMethodInfo = Type.GetType("UnityEditor.ScriptAttributeUtility, UnityEditor").GetMethod("GetHandler", BindingFlags.NonPublic | BindingFlags.Static);

            var handlerType = getHandlerMethodInfo.ReturnType;
            GetHandler = (Func<SerializedProperty, object>)getHandlerMethodInfo.CreateDelegate(typeof(Func<SerializedProperty, object>));

            var myGuiMethod = typeof(PropertyHandler).GetMethod(nameof(OnGUI), BindingFlags.Public | BindingFlags.Static);
            var guiParamTypes = myGuiMethod.GetParameters().Select(it => it.ParameterType).ToArray();
            OnGUIMethodInfo = handlerType.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Instance, null, guiParamTypes, null);

            var myHeightMethod = typeof(PropertyHandler).GetMethod(nameof(GetHeight), BindingFlags.Public | BindingFlags.Static); 
            var heightParamTypes = myHeightMethod.GetParameters().Select(it => it.ParameterType).ToArray();
            GetHeightMethodInfo = handlerType.GetMethod("GetHeight", BindingFlags.Public | BindingFlags.Instance, null, heightParamTypes, null);
        }

        private static readonly object[] onGuiParameters = new object[4];
        private static readonly object[] getHeightParameters = new object[3];

        public static bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            object handlerObj = GetHandler(property);

            onGuiParameters[0] = position;
            onGuiParameters[1] = property;
            onGuiParameters[2] = label;
            onGuiParameters[3] = includeChildren;

            return (bool)OnGUIMethodInfo.Invoke(handlerObj, onGuiParameters);
        }

        public static float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            object handlerObj = GetHandler(property);

            getHeightParameters[0] = property;
            getHeightParameters[1] = label;
            getHeightParameters[2] = includeChildren;

            return (float)GetHeightMethodInfo.Invoke(handlerObj, getHeightParameters);
        }

    }


    public static float GetDefaultDrawerHeight(this SerializedProperty property, GUIContent label = null, bool includeChildren = true) =>
        PropertyHandler.GetHeight(property, label, includeChildren);

    public static bool DrawDefaultDrawer(this SerializedProperty prop, Rect position, GUIContent label = null, bool includeChildren = true) =>
        PropertyHandler.OnGUI(position, prop, label, includeChildren);

    public static void DrawContents(this SerializedProperty property, Rect position, List<string> excludedProperties = null)
    {
        property = property.Copy();
        var yDiff = 0f;
        var enter = true;
        var myDepth = property.depth;
        while (property.NextVisible(enter))
        {
            if(property.depth <= myDepth) break;
            enter = false;
            if (excludedProperties != null && excludedProperties.Contains(property.name)) continue;
            yDiff += PaddingSize;
            var height = property.GetDefaultDrawerHeight();
            var myRect = new Rect(position.x, position.y + yDiff, position.width, height);
            property.DrawDefaultDrawer(myRect);
            yDiff += height + PaddingSize;
        }
    }

    public static float GetContentHeight(this SerializedProperty property, List<string> excludedProperties = null)
    {
        property = property.Copy();
        var yDiff = 0f;
        var enter = true;
        var myDepth = property.depth;

        while (property.NextVisible(enter))
        {
            if (property.depth <= myDepth) break;
            enter = false;
            if (excludedProperties != null && excludedProperties.Contains(property.name)) continue;
            yDiff += property.GetDefaultDrawerHeight() + 2 * PaddingSize;
        }

        return yDiff;
    }
}
