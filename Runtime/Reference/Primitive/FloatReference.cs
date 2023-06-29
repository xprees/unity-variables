using System;
using Xprees.Variables.Base;

namespace Xprees.Variables.Reference.Primitive
{
    [Serializable]
    public class FloatReference : ReferenceBase<float>
    {
        public FloatReference(float value) : base(value)
        {
        }
    }
}