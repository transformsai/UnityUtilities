
using JetBrains.Annotations;
using UnityEngine;

namespace TransformsAI.Unity.Utilities
{
    [System.AttributeUsage(System.AttributeTargets.Field), MeansImplicitUse]
    public class ButtonAttribute : PropertyAttribute
    {
        public readonly string MethodName;
        public ButtonAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}