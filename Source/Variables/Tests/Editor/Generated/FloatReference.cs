using System;

namespace Fasteraune.SO.Variables
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