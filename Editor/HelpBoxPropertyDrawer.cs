
using System;
using UnityEditor;
using UnityEngine;

namespace TransformsAI.Unity.Utilities.Editor
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxPropertyDrawer : DecoratorDrawer
    {
        HelpBoxAttribute HelpBoxAttribute => (HelpBoxAttribute)attribute;
        public override float GetHeight()
        {
            var content = new GUIContent(HelpBoxAttribute.Message);
            var calc = EditorStyles.helpBox.CalcHeight(content, EditorGUIUtility.currentViewWidth - 69);
            var min = EditorUtils.SingleLineHeight * 2;
            return Mathf.Max(calc, min);
        }
        public override void OnGUI(Rect position)
        {
            EditorGUI.HelpBox(position, HelpBoxAttribute.Message, Convert(HelpBoxAttribute.MessageType));
        }
        public static MessageType Convert(MessageTypes type)
        {
            switch (type)
            {
                case MessageTypes.None:     return MessageType.None;
                case MessageTypes.Info:     return MessageType.Info;
                case MessageTypes.Warning:  return MessageType.Warning;
                case MessageTypes.Error:    return MessageType.Error;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    } 
   
}