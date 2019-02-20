using System;
using Fasteraune.Variables;

[Serializable]
public class FloatReference : BaseReference<float, FloatVariable>
{
    public FloatReference(float Value) : base(Value)
    {
    }

    public FloatReference()
    {
    }
}