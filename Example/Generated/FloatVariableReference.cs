using System;
using Fasteraune.SO.Variables;

namespace Generated.Variables
{
    [Serializable]
    public class FloatVariableReference : VariableReference<float, FloatVariable>
    {
        public FloatVariableReference(float Value) : base(Value)
        {
        }

        public FloatVariableReference()
        {
            
        }
    }
}