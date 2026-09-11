using UnityEngine;

namespace Xprees.Variables.Base
{
    public abstract class VariableAggregationBaseSO<T> : VariableBaseSO<T>
    {
        [Header("Aggregation")]
        [Tooltip("Aggregated variables.")]
        [SerializeField] protected VariableBaseSO<T>[] variables;

        public override T CurrentValue
        {
            get => AggregateValue(variables);
            set => Debug.LogWarning($"{GetType().Name} on {name} - cannot set value for aggregation!"); // cannot set value of aggregation
        }

        protected abstract T AggregateValue(VariableBaseSO<T>[] variableBases);

        public override void ResetToDefault()
        {
            // Aggregations do not hold independent state; values are computed dynamically.
        }

        public override void ClearTransientState()
        {
            // Aggregations do not hold independent state; values are computed dynamically.
        }
    }
}