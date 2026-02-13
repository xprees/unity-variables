using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace Xprees.Variables.Utils
{
    public static class CloningTools
    {
        /// <summary>
        /// Helper to clone a value of type T. It handles value types, reference types that implement ICloneable, and other reference types by serializing and deserializing them using Unity's JsonUtility. Note that the serialization approach only works for types that are compatible with JsonUtility (e.g., they must be Unity serializable). For more complex types, you might need to implement custom cloning logic or use a different serialization method.
        /// </summary>
        /// <param name="value">Value to clone</param>
        /// <typeparam name="T">primitive/value type/Unity serializable</typeparam>
        /// <returns>A clone of input value</returns>
        [Preserve]
        public static T Clone<T>(T value)
        {
            if (value == null) return default;

            // Value types can be returned directly since they are copied by value. However, we can optimize for value types that do not contain references to avoid unnecessary cloning.
            var canReturnAsIs = typeof(T).IsValueType && !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
            if (canReturnAsIs) return value;

            if (value is ICloneable cloneable) return (T) cloneable.Clone();

            // Handle collections (List<T>, arrays, etc.) - JsonUtility doesn't support them directly
            if (value is IList sourceList)
            {
                var listType = value.GetType();
                if (listType.IsArray)
                {
                    var elementType = listType.GetElementType();
                    var array = Array.CreateInstance(elementType!, sourceList.Count);
                    for (var i = 0; i < sourceList.Count; i++)
                    {
                        var clonedItem = CloneItem(sourceList[i]);
                        array.SetValue(clonedItem, i);
                    }

                    return (T) (object) array;
                }

                var clonedList = (IList) Activator.CreateInstance(listType);
                foreach (var item in sourceList)
                {
                    clonedList.Add(CloneItem(item));
                }

                return (T) clonedList;
            }

            // For other types, we can try to serialize and deserialize to create a deep clone.
            // JsonUtility only supports a subset of types. Guard against unsupported types and failed serialization.
            var type = typeof(T);

            // Allow UnityEngine.Object subclasses, or types explicitly marked as [Serializable].
            var isUnityObject = typeof(Object).IsAssignableFrom(type);
            var isSerializable = Attribute.IsDefined(type, typeof(SerializableAttribute));
            if (!isUnityObject && !isSerializable)
            {
                throw new InvalidOperationException(
                    $"Type '{type.FullName}' is not supported for JsonUtility-based cloning. " +
                    "Ensure the type is Unity-serializable (e.g., marked with [Serializable]) or implements ICloneable.");
            }

            try
            {
                var json = JsonUtility.ToJson(value);
                if (string.IsNullOrEmpty(json) || json == "{}" || json == "null")
                {
                    throw new InvalidOperationException(
                        $"JsonUtility failed to serialize an instance of type '{type.FullName}' " +
                        "for cloning. The resulting clone would be incomplete or empty.");
                }

                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"AOT/IL2CPP error cloning type '{type.FullName}'. " +
                    "Ensure the type is preserved and AOT-compatible.", ex);
            }
        }

        /// Helper to clone individual items in collections, using the same logic as the main Clone method.
        private static object CloneItem(object item)
        {
            if (item == null) return null;

            var itemType = item.GetType();

            // Value types are copied by value
            if (itemType.IsValueType) return item;

            // ICloneable
            if (item is ICloneable cloneable) return cloneable.Clone();

            // UnityEngine.Object - return reference (can't deep clone ScriptableObjects easily)
            if (item is Object) return item;

            // Try JSON serialization for other serializable types
            if (Attribute.IsDefined(itemType, typeof(SerializableAttribute)))
            {
                var json = JsonUtility.ToJson(item);
                if (!string.IsNullOrEmpty(json) && json != "{}" && json != "null")
                {
                    return JsonUtility.FromJson(json, itemType);
                }
            }

            // Fallback: return the same reference
            return item;
        }
    }
}