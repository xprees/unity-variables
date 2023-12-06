using UnityEngine;

namespace Xprees.Variables.Base
{
    public abstract class VariableAggregationBaseSO<T> : VariableBaseSO<T>
    {
        [Header("Aggregation")]
        [Tooltip("Aggregated variables.")]
        [SerializeField] protected VariableBaseSO<T>[] variables;

        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
            ResetState();
        }

        public override T CurrentValue
        {
            get => AggregateValue(variables);
            set => Debug.LogWarning($"{GetType().Name} on {name} - cannot set value for aggregation!"); // cannot set value of aggregation
        }

        protected abstract T AggregateValue(VariableBaseSO<T>[] variableBases);
    }
}