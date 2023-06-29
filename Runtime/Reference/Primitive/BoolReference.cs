using System;
using Xprees.Variables.Base;

namespace Xprees.Variables.Reference.Primitive
{
    [Serializable]
    public class BoolReference : ReferenceBase<bool>
    {
        public BoolReference(bool value = false) : base(value)
        {
        }
    }
}