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
            ResetState();
        }

        public override abstract void ResetState();
    }

    /// <summary>
    /// Base class for all variables ScriptableObjects.
    /// Object holds the state on runtime, but reset every time OnEnable to defaultValue.
    /// </summary>
    /// <typeparam name="T">Unity Serializable</typeparam>
    public class VariableBaseSO<T> : VariableBaseSO
    {
        [SerializeField] protected T defaultValue;
        [SerializeField] private T currentValue;

        public virtual T CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                onValueChanged?.Invoke(value);
            }
        }

        public UnityAction<T> onValueChanged;

        public void SetValue(T value) => CurrentValue = value;

        public void SetValue(VariableBaseSO<T> variable) => CurrentValue = variable.CurrentValue;

        public static implicit operator T(VariableBaseSO<T> variable) =>
            variable != null ? variable.CurrentValue : default;

        public override void ResetState() => CurrentValue = defaultValue;
    }
}