// ------------------------------------------------------------------------------------------------------
// Heavily inspired by Ryan Hipple, 10/04/17 from Unite 2017 - Game Architecture with Scriptable Objects
// ------------------------------------------------------------------------------------------------------

using System;
using UnityEngine.Events;
using Xprees.Core;
using Xprees.Variables.Utils;

namespace Xprees.Variables.Base
{
    /// <summary>
    /// Base class for all variable references used in the game. It can either use an inlined value or reference a VariableBaseSO Scriptable Object. The Value property abstracts this choice away, so users of ReferenceBase don't have to care about it.
    /// </summary>
    /// <typeparam name="T">Unity Serializable</typeparam>
    [Serializable]
    public class ReferenceBase<T> : IResettable
    {
        private T _defaultInlinedValue; // Used to reset state of inlined value

        public bool useInlined = true;
        public T inlinedValue;
        public VariableBaseSO<T> variable;

        // Internal event for inlined value changes, since VariableBaseSO already has its own onValueChanged event.
        private UnityAction<T> _onInlinedValueChanged;

        // ReSharper disable once InconsistentNaming
        /// Event invoked when Value changes.
        public event UnityAction<T> onValueChanged
        {
            add
            {
                var shouldRedirectToVarEvent = !useInlined && variable != null;
                if (shouldRedirectToVarEvent)
                {
                    variable.onValueChanged += value;
                    return;
                }

                _onInlinedValueChanged += value;
            }

            remove
            {
                var shouldRedirectToVarEvent = !useInlined && variable != null;
                if (shouldRedirectToVarEvent)
                {
                    variable.onValueChanged -= value;
                    return;
                }

                _onInlinedValueChanged -= value;
            }
        }

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
                    _onInlinedValueChanged?.Invoke(value); // Invoke inlined value change event
                    return;
                }

                // VariableBaseSO will invoke its own onValueChanged event, so no need to invoke here.
                variable.SetValue(value);
            }
        }

        public static implicit operator T(ReferenceBase<T> reference) => reference != null ? reference.Value : default;

        public virtual void BackupStartState()
        {
            if (!useInlined) return; // Variables does that by themselves

            _defaultInlinedValue = CloningTools.Clone(inlinedValue);
        }

        public virtual void ResetState()
        {
            if (useInlined)
            {
                inlinedValue = CloningTools.Clone(_defaultInlinedValue);
                return;
            }

            variable?.ResetState();
        }
    }
}