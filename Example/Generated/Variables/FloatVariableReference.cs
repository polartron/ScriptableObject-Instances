using System;
using Fasteraune.SO.Variables;

namespace Generated.Variables
{
    [Serializable]
    public class FloatReference : VariableReference<float, FloatVariable>
    {
        public FloatReference(float Value) : base(Value)
        {
        }

        public FloatReference()
        {
        }
    }
}