using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TransformsAI.Unity.Utilities.Editor
{
    public static class EditorUtils
    {

        public static string Tooltip;
        public static GUIStyle TooltipStyle;
        public static float PaddingSize = 1;
        public static float IndentSize = 10;
        public static readonly float SingleLineHeight = EditorGUIUtility.singleLineHeight + 2 * PaddingSize;

        public static string DrawScriptField(UnityEditor.Editor editor)
        {
            var mScript = editor.serializedObject.FindProperty("m_Script");
            using var x = new EditorGUI.DisabledGroupScope(true);
            EditorGUILayout.PropertyField(mScript);
            return "m_Script";
        }

        public static void DrawTooltip()
        {

            if (string.IsNullOrEmpty(Tooltip)) return;

            TooltipStyle ??= GUI.skin.FindStyle("Tooltip") ??
                             EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("Tooltip");

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

        
        public static (Rect left, Rect right) SplitHorizontally(this Rect rect, float splitPercent)
        {
            var left = rect;
            left.xMax -= rect.width * splitPercent;
            var right = rect;
            right.xMin += left.width;
            return (left, right);
        }

        
        public static (Rect top, Rect bot) SplitVertically(this Rect rect, float splitPercent)
        {
            var first = rect;
            first.yMax -= rect.height * splitPercent;
            var second = rect;
            second.yMin += first.height;
            return (first, second);
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
        
        public static Rect SplitLabel(this Rect rect, GUIContent label)
        {
            var labelRect = rect;
            labelRect.xMax = labelRect.xMin + EditorGUIUtility.labelWidth;

            EditorGUI.LabelField(labelRect, label);

            var remainder = rect;
            remainder.xMin = labelRect.xMax;
            return remainder;
        }
        public static bool SplitFoldout(this Rect rect, bool value, GUIContent label, out Rect remainder)
        {
            var labelRect = rect;
            labelRect.xMax = labelRect.xMin + EditorGUIUtility.labelWidth;
            remainder = rect;
            remainder.xMin = labelRect.xMax;

            return EditorGUI.Foldout(labelRect,value, label);

            
        }



        public static float GetDefaultDrawerHeight(this SerializedProperty property, GUIContent label = null, bool includeChildren = true) =>
            PropertyHandler.GetHeight(property, label, includeChildren);

        public static bool DrawDefaultDrawer(this SerializedProperty prop, Rect position, GUIContent label = null, bool includeChildren = true) =>
            PropertyHandler.OnGUI(position, prop, label, includeChildren);

        public static bool DrawContents(this SerializedProperty property, Rect position, List<string> excludedProperties = null)
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
                yDiff += PaddingSize;
                var height = property.GetDefaultDrawerHeight();
                var myRect = new Rect(position.x, position.y + yDiff, position.width, height);
                property.DrawDefaultDrawer(myRect);
                yDiff += height + PaddingSize;
            }

            return yDiff != 0;
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
    public readonly struct ContentColorScope : IDisposable
    {
        private readonly Color _lastColor;
        public ContentColorScope(Color newColor)
        {
            _lastColor = GUI.contentColor;
            GUI.contentColor = newColor;
        }

        public void Dispose()
        {
            GUI.contentColor = _lastColor;
        }
    }
}
