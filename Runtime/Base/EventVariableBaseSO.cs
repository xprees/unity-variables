using System;
using UnityEngine.Events;

namespace Xprees.Variables.Base
{
    [Obsolete]
    public class EventVariableBaseSO<T> : VariableBaseSO<T>
    {
        public UnityAction onValueChanged;

        private T _currentValue;

        public override T CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                onValueChanged?.Invoke();
            }
        }
    }
}