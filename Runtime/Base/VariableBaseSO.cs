using UnityEngine;
using UnityEngine.Events;
using Xprees.Core;
using Xprees.Variables.Utils;

namespace Xprees.Variables.Base
{
    public abstract class VariableBaseSO : DescriptionBaseSO
    {
        protected virtual void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
            InitializeValue();
        }

        /// Initializes the runtime value when the asset is loaded. Defaults to the authored default value.
        protected virtual void InitializeValue() => ResetToDefault();

        /// Resets the variable to its authored default value.
        public abstract void ResetToDefault();
    }

    /// Base class for all variable ScriptableObjects.
    /// Manages its own default initialization self-contained, and integrates with StateSnapshotService for scenario boundaries.
    /// By default, resets on EnterPlayMode unless decorated with a custom [ResetOnPlayMode] or marked Persistent.
    /// <typeparam name="T">Unity Serializable or System.Serializable</typeparam>
    [ResetOnPlayMode(PlayModeResetTiming.EnterPlayMode)]
    public class VariableBaseSO<T> : VariableBaseSO, IRuntimeStateOwner
    {
        [Tooltip("Authored default value of the variable.")]
        [SerializeField] protected T defaultValue;

        [Tooltip("Current value of variable - Runtime only value. Replaced with default value on OnEnable/PlayMode start.")]
        [SerializeField] protected T currentValue;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // If not playing, reset current value to default value on validate to ensure that the variable is always in a valid state in Editor.
            if (!Application.isPlaying) currentValue = CloningTools.Clone(defaultValue);
        }
#endif

        public virtual T CurrentValue
        {
            get => currentValue;
            set
            {
#if UNITY_EDITOR
                // If not already captured in Editor capture before changing the value
                if (PlayModeStateTracker.IsPlaying) StateSnapshotService.EnsureCaptured(this);
#endif
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

        /// Resets the variable's current value to a fresh clone of its authored default value and notifies listeners.
        public override void ResetToDefault()
        {
            currentValue = CloningTools.Clone(defaultValue);
            onValueChanged?.Invoke(currentValue);
        }

        /// Invoked by StateSnapshotService when restoring state at scenario boundaries.
        /// Notifies runtime listeners that the value has reverted to its snapshot baseline.
        public virtual void ClearTransientState()
        {
            onValueChanged?.Invoke(currentValue);
        }
    }
}