// ------------------------------------------------------------------------------------------------------
// Heavily inspired by Ryan Hipple, 10/04/17 from Unite 2017 - Game Architecture with Scriptable Objects
// ------------------------------------------------------------------------------------------------------

using System;
using UnityEngine.Events;
using Xprees.Core;

namespace Xprees.Variables.Base
{
    [Serializable]
    public class ReferenceBase<T> : IResettable
    {
        private T _defaultInlinedValue; // Used to reset state of inlined value

        public bool useInlined = true;
        public T inlinedValue;
        public VariableBaseSO<T> variable;

        public UnityAction<T> onValueChanged = delegate { };

        public ReferenceBase()
        {
        }

        public ReferenceBase(T value)
        {
            useInlined = true;
            inlinedValue = value;
            _defaultInlinedValue = value;
        }

        public T Value
        {
            get => useInlined ? inlinedValue : variable.CurrentValue;
            set
            {
                if (useInlined)
                {
                    inlinedValue = value;
                    onValueChanged?.Invoke(value);
                    return;
                }

                variable.SetValue(value);
                onValueChanged?.Invoke(value); // ignores that variable has its own onValueChanged
            }
        }

        public static implicit operator T(ReferenceBase<T> reference) => reference != null ? reference.Value : default;

        public virtual void ResetState()
        {
            if (useInlined)
            {
                inlinedValue = _defaultInlinedValue;
                return;
            }

            variable.ResetState();
        }
    }
}