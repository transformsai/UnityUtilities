using System;
using System.Reflection;
using UnityEngine;

namespace TransformsAI.Unity.Utilities
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class PlayerPrefAttribute : PropertyAttribute
    {
        private readonly Type _sourceType;
        private readonly string _propName;
        public readonly string DisplayLabel;
        private PlayerPrefsSetting _setting;

        public PlayerPrefAttribute(Type sourceType, string staticPlayerPrefsSettingPropertyName, string displayLabel = null)
        {
            _sourceType = sourceType;
            _propName = staticPlayerPrefsSettingPropertyName;
            DisplayLabel = displayLabel;
        }

        public PlayerPrefsSetting Setting => 
            _setting ??= _sourceType.GetFieldOrPropertyValue<PlayerPrefsSetting>(_propName);
    }

    public class PlayerPrefsSetting
    {
        public readonly string Prefix;
        public readonly string FullKey;
        public readonly string Name;
        public readonly string DefaultValue;
        public readonly bool IsBoolValue;

        public string Value
        {
            get => PlayerPrefs.GetString(FullKey, DefaultValue);
            set => PlayerPrefs.SetString(FullKey, value);
        }

        public bool BoolValue
        {
            get => Value == true.ToString();
            set => Value = value.ToString();
        }

        public PlayerPrefsSetting(string prefix, string key, string defaultValue = null)
        {
            Prefix = prefix;
            FullKey = $"{Prefix}_{key}";
            Name = key;
            DefaultValue = defaultValue;
            IsBoolValue = false;
        }


        public PlayerPrefsSetting(string prefix, string key, bool defaultValue)
        {
            Prefix = prefix;
            FullKey = $"{Prefix}_{key}";
            Name = key;
            IsBoolValue = true;
            DefaultValue = defaultValue.ToString();

        }

        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}