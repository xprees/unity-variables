using UnityEngine;
using Xprees.Core;
using Xprees.Variables.Utils;

namespace Xprees.Variables.Base
{
    /// Base class for variable ScriptableObjects, whose value is backed by PlayerPrefs and survives across sessions.
    /// The value is written through on every set and loaded when the asset is enabled.
    /// Opted out of both the Play Mode reset and the StateSnapshotService scenario restore.
    /// <typeparam name="T">Unity Serializable or System.Serializable</typeparam>
    [StatefulLifetime(StateLifetime.Persistent)]
    [ResetOnPlayMode(PlayModeResetTiming.None)]
    public abstract class PersistentVariableBaseSO<T> : VariableBaseSO<T>
    {
        [Header("Persistence Settings")]
        [Tooltip("PlayerPrefs key backing this variable. Required and must be unique across the project.")]
        [SerializeField] private string playerPrefsKey;

        public string PlayerPrefsKey => playerPrefsKey;

        public bool HasStoredValue => !string.IsNullOrEmpty(playerPrefsKey) && PlayerPrefs.HasKey(playerPrefsKey);

        public override T CurrentValue
        {
            get => base.CurrentValue;
            set
            {
                // Persist before notifying so listeners always observe an already stored value
                Persist(value);
                base.CurrentValue = value;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (string.IsNullOrEmpty(playerPrefsKey))
            {
                Debug.LogError($"Persistent variable '{name}' has no PlayerPrefs key assigned and will not be stored.", this);
            }

            // Unlike a regular variable, the Editor should show what is actually stored, not the authored default.
            if (!Application.isPlaying) currentValue = ReadStoredOrDefault();
        }
#endif

        /// Reads the stored value from PlayerPrefs, returning the fallback when no value is stored.
        protected abstract T ReadStoredValue(string key, T fallback);

        /// Writes the value to PlayerPrefs under the given key.
        protected abstract void WriteStoredValue(string key, T value);

        protected override void InitializeValue() => Load();

        /// Loads the value from PlayerPrefs, falling back to the authored default when nothing is stored, and notifies listeners.
        public void Load()
        {
            currentValue = ReadStoredOrDefault();
            onValueChanged?.Invoke(currentValue);
        }

        /// Resets the variable to its authored default value and overwrites the stored value with it.
        public override void ResetToDefault() => CurrentValue = CloningTools.Clone(defaultValue);

        /// Deletes the stored value and resets the variable to its authored default value.
        public void ClearStoredValue()
        {
            if (!string.IsNullOrEmpty(playerPrefsKey)) PlayerPrefs.DeleteKey(playerPrefsKey);
            base.ResetToDefault();
        }

        /// Forces PlayerPrefs to write to disk immediately.
        /// Rarely needed - Unity flushes on quit and on focus loss.
        public void Flush() => PlayerPrefs.Save();

        /// Reads the stored value without notifying listeners, falling back to a fresh clone of the authored default value.
        private T ReadStoredOrDefault()
        {
            var fallback = CloningTools.Clone(defaultValue);
            return HasStoredValue ? ReadStoredValue(playerPrefsKey, fallback) : fallback;
        }

        private void Persist(T value)
        {
            if (string.IsNullOrEmpty(playerPrefsKey))
            {
                Debug.LogError($"Persistent variable '{name}' has no PlayerPrefs key assigned - value was not stored.", this);
                return;
            }

            WriteStoredValue(playerPrefsKey, value);
        }
    }
}