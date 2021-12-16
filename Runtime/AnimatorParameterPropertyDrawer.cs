using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace TransformsAI.Unity.Utilities
{
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public class AnimatorParameterPropertyDrawer : PropertyDrawer
    {
        private const string NoParameterLabel = "SELECT A PARAMETER";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Animator animator = null;
            if (property.serializedObject.targetObject is MonoBehaviour monoBehaviour)
            {
                animator = monoBehaviour.GetComponent<Animator>();
            }

            if (!animator ||
                property.propertyType != SerializedPropertyType.String ||
                !CheckHasParameters(animator))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);

            List<string> parameters = animator.parameters.Select(p => p.name).ToList();
            parameters.Insert(0, NoParameterLabel);
            int currentIndex = parameters.IndexOf(property.stringValue);
            currentIndex = Mathf.Clamp(currentIndex, 0, parameters.Count - 1);
            currentIndex = EditorGUI.Popup(position, label, currentIndex,
                parameters.Select(p => new GUIContent(p)).ToArray());

            property.stringValue = parameters[currentIndex];
        }

        private bool CheckHasParameters(Animator animator)
        {
            // Refresh the animator to get the right parameter count.
            if (!animator.isInitialized)
            {
                animator.enabled = !animator.enabled;
                // ReSharper disable twice Unity.InefficientPropertyAccess -- Needed to refresh
                animator.enabled = !animator.enabled;
            }
            return 0 != animator.parameterCount;
        }
    }
}
#endif

namespace TransformsAI.Unity.Utilities
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class AnimatorParameterAttribute : PropertyAttribute { }
}