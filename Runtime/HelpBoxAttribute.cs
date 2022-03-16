using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TransformsAI.Unity.Utilities
{
    public class HelpBoxAttribute : PropertyAttribute
    {
        public HelpBoxAttribute(string message, MessageType messageType = MessageType.None)
        {
            _data = new HelpBoxData {Message = message, MessageType = messageType};
        }
        public HelpBoxAttribute(Type type, string staticHelpBoxDataMethodName)
        {
            var method = type.GetMethod(
                staticHelpBoxDataMethodName, 
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            if (method == null)
                throw new MethodAccessException($"Could not find static method {staticHelpBoxDataMethodName} in type {type.Name}");

            if(method.ReturnType != typeof(HelpBoxData)) 
                throw new MethodAccessException($"Method {staticHelpBoxDataMethodName} in type {type.Name} does not return {nameof(HelpBoxData)}");

            _dataGetter = (Func<HelpBoxData>) method.CreateDelegate(typeof(Func<HelpBoxData>));
        }

        private readonly HelpBoxData? _data = null;
        private readonly Func<HelpBoxData> _dataGetter = null;
        public HelpBoxData Data => _data ?? _dataGetter();
        public string Message => Data.Message;
        public MessageType MessageType => Data.MessageType;
    }

    public struct HelpBoxData
    {
        public string Message { get; set; }
        public MessageType MessageType { get; set; }
    }
}