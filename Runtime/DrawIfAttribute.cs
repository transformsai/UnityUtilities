using UnityEngine;

public class DrawIfAttribute : PropertyAttribute
{
    public readonly string FieldOrPropertyName;
    public DrawIfAttribute(string fieldOrPropertyName) => FieldOrPropertyName = fieldOrPropertyName;
}
