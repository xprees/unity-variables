using System;
using Xprees.Variables.Base;

namespace Xprees.Variables.Reference.Primitive
{
    [Serializable]
    public class StringReference : ReferenceBase<string>
    {
        public StringReference()
        {
        }

        public StringReference(string value = null) : base(value)
        {
        }
    }
}