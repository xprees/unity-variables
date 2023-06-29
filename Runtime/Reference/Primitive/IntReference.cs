using System;
using Xprees.Variables.Base;

namespace Xprees.Variables.Reference.Primitive
{
    [Serializable]
    public class IntReference : ReferenceBase<int>
    {
        public IntReference()
        {
        }

        public IntReference(int value) : base(value)
        {
        }
    }
}