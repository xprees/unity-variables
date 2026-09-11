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

        public override T CurrentValue
        {
            get => ModifyValue(variable.CurrentValue);
            set
            {
                if (disableWrite.Value)
                {
                    Debug.LogWarning($"Trying to set value of {name} but writing is disabled.", this);
                    return;
                }

                variable.CurrentValue = ModifyValue(value);
            }
        }

        protected abstract T ModifyValue(T value);

        public override void ResetToDefault()
        {
            // Modifiers do not hold independent state; state is owned by the wrapped variable.
        }

        public override void ClearTransientState()
        {
            // Modifiers do not hold independent state; state is owned by the wrapped variable.
        }
    }
}