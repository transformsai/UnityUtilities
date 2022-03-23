using UnityEditor;
using UnityEngine;

namespace TransformsAI.Unity.Utilities.Editor
{

    [CustomPropertyDrawer(typeof(PlayerPrefAttribute))]
    public class PlayerPrefsPropertyDrawer : DecoratorDrawer
    {
        public override float GetHeight() => EditorUtils.SingleLineHeight;

        public override void OnGUI(Rect position)
        {
            var attr = (PlayerPrefAttribute)attribute;

            var buttonRect = position;
            position.xMax -= 50;
            buttonRect.xMin = position.xMax;
            var setting = attr.Setting;

            var didReset = GUI.Button(buttonRect, "Reset");
            if (didReset) setting.Reset();

            var name = attr.DisplayLabel ?? ObjectNames.NicifyVariableName(setting.Name);

            if (setting.IsBoolValue)
            {
                var val = setting.BoolValue;
                var newVal = EditorGUI.Toggle(position, new GUIContent(name), val);
                if (newVal != val) setting.BoolValue = newVal;
            }
            else
            {
                var val = setting.Value;
                var newVal = EditorGUI.DelayedTextField(position, name, val);
                if (newVal != val) setting.Value = newVal;
            }
        }
    }
}