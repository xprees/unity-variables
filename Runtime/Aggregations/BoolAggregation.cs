using System;
using System.Linq;
using UnityEngine;
using Xprees.Variables.Base;

namespace Xprees.Variables.Aggregations
{
    [CreateAssetMenu(menuName = "Variables/Aggregations/Bool", fileName = "BoolVarAgg")]
    public class BoolAggregation : VariableAggregationBaseSO<bool>
    {
        [SerializeField] protected BoolAggregator aggregator = BoolAggregator.And;

        protected override bool AggregateValue(VariableBaseSO<bool>[] variableBases) => aggregator switch
        {
            BoolAggregator.And => variables.All(v => v.CurrentValue),
            BoolAggregator.Or => variables.Any(v => v.CurrentValue),
            BoolAggregator.Xor => variables.Aggregate(false, (current, v) => current ^ v.CurrentValue),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public enum BoolAggregator
    {
        And,
        Or,
        Xor,
    }
}