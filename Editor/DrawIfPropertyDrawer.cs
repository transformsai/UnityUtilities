using System;
using TransformsAI.Unity.Utilities;
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

        try
        {
            return type.GetFieldOrPropertyValue<bool>(drawIfAttribute.FieldOrPropertyName, container);
        }
        catch (Exception e)
        {
            Debug.LogException(e, property.serializedObject.targetObject);
            return true;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        ShouldDraw(property) ? EditorGUI.GetPropertyHeight(property, label, true) : 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldDraw(property)) EditorGUI.PropertyField(position, property, label, true);
    }
}
