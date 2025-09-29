using UnityEngine;
using UnityEngine.Events;
using Xprees.Core;

namespace Xprees.Variables.Base
{
    public abstract class VariableBaseSO : DescriptionBaseSO
    {
        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
            ForceResetState();
        }

        /// Force reset state regardless of any protection settings.
        public abstract void ForceResetState();

        public override void ResetState() => ForceResetState();
    }

    /// <summary>
    /// Base class for all variables ScriptableObjects.
    /// Object holds the state on runtime, but reset every time OnEnable to defaultValue.
    /// </summary>
    /// <typeparam name="T">Unity Serializable</typeparam>
    public class VariableBaseSO<T> : VariableBaseSO
    {
        [Tooltip("Value to which the variable will be reset on OnEnable or ResetState call.")]
        [SerializeField] protected T defaultValue;

        [Tooltip("Current value of variable - Runtime only value.")]
        [SerializeField] private T currentValue;

        [Header("Settings")]
        [Tooltip(
            "If true, the variable will not be reset on plain ResetState call. It will be reset only on OnEnable. Useful for game-system variables.")]
        [SerializeField] protected bool protectedDontReset = false;

        public virtual T CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                onValueChanged?.Invoke(value);
            }
        }

        /// Event invoked when CurrentValue changes.
        public UnityAction<T> onValueChanged;

        public void SetValue(T value) => CurrentValue = value;

        public void SetValue(VariableBaseSO<T> variable) => CurrentValue = variable.CurrentValue;

        public static implicit operator T(VariableBaseSO<T> variable) =>
            variable != null ? variable.CurrentValue : default;

        public override void ResetState()
        {
            if (protectedDontReset) return; // skip reset if protected

            ForceResetState();
        }

        public override void ForceResetState() => CurrentValue = defaultValue;

    }
}