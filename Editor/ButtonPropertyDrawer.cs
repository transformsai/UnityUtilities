using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TransformsAI.Unity.Utilities.Editor
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        { 
            var attr = (ButtonAttribute)attribute;
            if (GUI.Button(position, label))
            {
                var parent = prop.ContainerObject();
                if (parent == null) throw new Exception("Could not get containing object");

                var method = parent.GetType().GetMethod(attr.MethodName,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (method == null) throw new Exception("Could not find method");
                
                Undo.RecordObject(prop.serializedObject.targetObject, label.text);
                method.Invoke(method.IsStatic ? null : parent, null);
            }
        }
    }
}