using System;
using UnityEngine;

namespace Xprees.Variables.Base
{
    /// Base class for persistent variables of arbitrary serializable types, stored as JSON in PlayerPrefs.
    /// Only [Serializable] classes and structs round-trip through JsonUtility - top level primitives,
    /// enums, and arrays are not supported and need a dedicated subclass of PersistentVariableBaseSO.
    /// <typeparam name="T">Unity-Serializable or System.Serializable class or struct</typeparam>
    public abstract class PersistentJsonVariableSO<T> : PersistentVariableBaseSO<T>
    {
        protected override T ReadStoredValue(string key, T fallback)
        {
            var json = PlayerPrefs.GetString(key, string.Empty);
            if (string.IsNullOrEmpty(json) || json == "{}" || json == "null") return fallback;

            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to deserialize persistent variable '{name}' from PlayerPrefs key '{key}': {exception.Message}", this);
                return fallback;
            }
        }

        protected override void WriteStoredValue(string key, T value) => PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
    }
}