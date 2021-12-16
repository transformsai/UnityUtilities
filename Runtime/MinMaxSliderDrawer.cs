// https://frarees.github.io/default-gist-license

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace TransformsAI.Unity.Utilities
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    class MinMaxSliderDrawer : PropertyDrawer
    {
        const string VectorMinName = "x";
        const string VectorMaxName = "y";
        const float FloatFieldWidth = 30f;
        const float Spacing = 2f;
        const float RoundingValue = 100f;

        float Round(float value, float roundingValue)
        {
            if (roundingValue == 0)
            {
                return value;
            }

            return Mathf.Round(value * roundingValue) / roundingValue;
        }

        void SetVectorValue(SerializedProperty property, float min, float max)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                min = Round(min, RoundingValue);
                max = Round(max, RoundingValue);
                property.vector2Value = new Vector2(min, max);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                property.vector2IntValue = new Vector2Int((int)min, (int)max);
            }
            else
            {
                throw new InvalidOperationException("Cannot use MinMaxSlider on non Vector2 field:" + property.propertyPath);
            }

        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float min;
            float max;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                {
                    var v = property.vector2Value;
                    min = v.x;
                    max = v.y;
                    break;
                }
                case SerializedPropertyType.Vector2Int:
                {
                    var v = property.vector2IntValue;
                    min = v.x;
                    max = v.y;
                    break;
                }
                default:
                {
                    throw new InvalidOperationException("Cannot use MinMaxSlider on non Vector2 field:" + property.propertyPath);
                }
            }
            label = EditorGUI.BeginProperty(position, label, property);
            float ppp = EditorGUIUtility.pixelsPerPoint;
            float spacing = Spacing * ppp;
            float fieldWidth = FloatFieldWidth * ppp;

            var indent = EditorGUI.indentLevel;

            var attr = (MinMaxSliderAttribute) attribute;

            var r = EditorGUI.PrefixLabel(position, label);

            Rect sliderPos = r;

            sliderPos.x += fieldWidth + spacing;
            sliderPos.width -= (fieldWidth + spacing) * 2;

            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel = 0;
            EditorGUI.MinMaxSlider(sliderPos, ref min, ref max, attr.min, attr.max);
            EditorGUI.indentLevel = indent;
            if (EditorGUI.EndChangeCheck())
            {
                SetVectorValue(property, min, max);
            }

            Rect minPos = r;
            minPos.width = fieldWidth;

            var vectorMinProp = property.FindPropertyRelative(VectorMinName);
            EditorGUI.showMixedValue = vectorMinProp.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel = 0;
            min = EditorGUI.DelayedFloatField(minPos, min);
            EditorGUI.indentLevel = indent;
            if (EditorGUI.EndChangeCheck())
            {
                min = Mathf.Max(min, attr.min);
                min = Mathf.Min(min, max);
                SetVectorValue(property, min, max);
            }

            Rect maxPos = position;
            maxPos.x += maxPos.width - fieldWidth;
            maxPos.width = fieldWidth;

            var vectorMaxProp = property.FindPropertyRelative(VectorMaxName);
            EditorGUI.showMixedValue = vectorMaxProp.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel = 0;
            max = EditorGUI.DelayedFloatField(maxPos, max);
            EditorGUI.indentLevel = indent;
            if (EditorGUI.EndChangeCheck())
            {
                max = Mathf.Min(max, attr.max);
                max = Mathf.Max(max, min);
                SetVectorValue(property, min, max);
            }

            EditorGUI.showMixedValue = false;
        }
    }
}
#endif

namespace TransformsAI.Unity.Utilities
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;

        public MinMaxSliderAttribute() : this(0, 1) { }

        public MinMaxSliderAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}