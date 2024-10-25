using UnityEngine;
using Xprees.Variables.Base;

namespace Xprees.Variables.Modifiers
{
    [CreateAssetMenu(menuName = "Variables/Modifiers/Bool modifier", fileName = "BoolVarMod")]
    public class BoolModifier : VariableModifierBaseSO<bool>
    {
        [SerializeField] protected BoolModifiers variableModifier = BoolModifiers.Identity;

        protected override bool ModifyValue(bool value) => variableModifier switch
        {
            BoolModifiers.Identity => value,
            BoolModifiers.Not => !value,
            _ => value,
        };
    }

    public enum BoolModifiers
    {
        Identity,
        Not,
    }
}