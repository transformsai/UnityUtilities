using UnityEditor;
using UnityEngine;

public class DrawIfAttribute : PropertyAttribute
{
    public readonly string OtherBoolPropertyRelativePath;
    public readonly bool? BoolValue;
    public readonly int? IntValue;
    public readonly string StringValue;

    public DrawIfAttribute(string otherBoolPropertyRelativePath, bool value)
    {
        OtherBoolPropertyRelativePath = otherBoolPropertyRelativePath;
        BoolValue = value;
    }
    
    public DrawIfAttribute(string otherBoolPropertyRelativePath, string value)
    {
        OtherBoolPropertyRelativePath = otherBoolPropertyRelativePath;
        StringValue = value;
    }
    public DrawIfAttribute(string otherBoolPropertyRelativePath, int value)
    {
        OtherBoolPropertyRelativePath = otherBoolPropertyRelativePath;
        IntValue = value;
    }

}

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfDrawer : UnityEditor.PropertyDrawer
{
    public bool ShouldDraw(UnityEditor.SerializedProperty property)
    {
        var parentPrefix = "";
        var lastPath = property.propertyPath.LastIndexOf('.');
        if (lastPath > 0)
        {
            parentPrefix = property.propertyPath.Remove(lastPath);
        }

        var drawIfAttribute = ((DrawIfAttribute)attribute);
        var determinantPath = parentPrefix + drawIfAttribute.OtherBoolPropertyRelativePath;

        var determinantProp = property.serializedObject.FindProperty(determinantPath);


        switch (determinantProp?.propertyType)
        {
            case SerializedPropertyType.Integer:
                return drawIfAttribute.IntValue == determinantProp.intValue;
            case SerializedPropertyType.Boolean:
                return drawIfAttribute.BoolValue == determinantProp.boolValue;
            case SerializedPropertyType.String:
                return drawIfAttribute.StringValue == determinantProp.stringValue;
            case SerializedPropertyType.Enum:
                return drawIfAttribute.StringValue == determinantProp.enumNames[determinantProp.enumValueIndex];
            default:
                Debug.LogError($"Bad relative property: {determinantPath}", property.serializedObject.targetObject);
                return true;
        }
    }

    public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label) =>
        ShouldDraw(property) ? UnityEditor.EditorGUI.GetPropertyHeight(property, label, true) : 0;

    public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
    {
        if (ShouldDraw(property)) UnityEditor.EditorGUI.PropertyField(position, property, label, true);
    }
}
#endif
