using System.Reflection;
using TransformsAI.Unity.Utilities.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfDrawer : PropertyDrawer
{
    public bool ShouldDraw(SerializedProperty property)
    {
        var container = property.ContainerObject();

        var drawIfAttribute = (DrawIfAttribute)attribute;

        var type = container?.GetType();

        if (type == null)
        {
            Debug.LogError("Bad container on property " + property.propertyPath, property.serializedObject.context);
            return true;
        }

        var field = type.GetField(drawIfAttribute.FieldOrPropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        if (field != null)
        {
            if (field.FieldType == typeof(bool)) return (bool)field.GetValue(container);

            Debug.LogError($"Field ({field.Name}) is not bool : {property.propertyPath}", property.serializedObject.context);
            return true;
        }

        var prop = type.GetProperty(drawIfAttribute.FieldOrPropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        if (prop != null)
        {
            if (prop.PropertyType == typeof(bool)) return (bool)prop.GetValue(container);

            Debug.LogError($"Property ({prop.Name}) is not bool : {property.propertyPath}", property.serializedObject.context);
            return true;
        }

        Debug.LogError("Could not find Field/Property " + property.propertyPath, property.serializedObject.context);

        return true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        ShouldDraw(property) ? EditorGUI.GetPropertyHeight(property, label, true) : 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldDraw(property)) EditorGUI.PropertyField(position, property, label, true);
    }
}
