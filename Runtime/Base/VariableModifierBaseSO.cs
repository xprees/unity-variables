using UnityEngine;
using Xprees.Variables.Reference.Primitive;

namespace Xprees.Variables.Base
{
    public abstract class VariableModifierBaseSO<T> : VariableBaseSO<T>
    {
        [Header("Modifier")]
        [Tooltip("Variable to modify.")]
        [SerializeField] protected VariableBaseSO<T> variable;

        [Space]
        [SerializeField] private BoolReference disableWrite = new(true);

        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
            ResetState();
        }

        public override T CurrentValue
        {
            get => ModifyValue(variable.CurrentValue);
            set
            {
                if (disableWrite.Value) return;
                variable.CurrentValue = ModifyValue(value);
            }
        }

        protected abstract T ModifyValue(T value);
    }
}