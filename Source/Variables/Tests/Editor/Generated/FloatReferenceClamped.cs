using System;

namespace Fasteraune.SO.Variables
{
	[Serializable]
	public class FloatReferenceClamped : IDisposable
	{
	    public FloatReference Target = new FloatReference();
	    public FloatReference Min = new FloatReference();
	    public FloatReference Max = new FloatReference();

	    public float Value
	    {
	        get
	        {
	            float value = Target.Value;
	            float maxValue = Max.Value;
	                    
	            if (value.CompareTo(maxValue) > 0)
	            {
	                Target.Value = maxValue;
	                return maxValue;
	            }
	            
	            float minValue = Min.Value;
	                 
	            if (value.CompareTo(minValue) < 0)
	            {
	                Target.Value = minValue;
	                return minValue;
	            }

	            return value;
	        }
	        set
	        {
	            float maxValue = Max.Value;
	                    
	            if (value.CompareTo(maxValue) > 0)
	            {
	                Target.Value = maxValue;
	                return;
	            }
	            
	            float minValue = Min.Value;
	                 
	            if (value.CompareTo(minValue) < 0)
	            {
	                Target.Value = minValue;
	                return;
	            }
	            
	            Target.Value = value; 
	        }
	    }

	    private void OnMinValueChanged(float minValue)
	    {
	        if (minValue.CompareTo(Target.Value) < 0)
	        {
	            Target.Value = minValue;
	        }
	    }
	    
	    private void OnMaxValueChanged(float maxValue)
	    {
	        if (maxValue.CompareTo(Target.Value) > 0)
	        {
	            Target.Value = maxValue;
	        }
	    }

	    public void AddListener(Action<float> listener)
	    {
	        Min.AddListener(OnMinValueChanged);
	        Max.AddListener(OnMaxValueChanged);
	        Target.AddListener(listener);
	    }
	    
	    public void RemoveListener(Action<float> listener)
	    {
	        Min.RemoveListener(OnMinValueChanged);
	        Max.RemoveListener(OnMaxValueChanged);
	        Target.RemoveListener(listener);
	    }

	    public void EnableCaching()
	    {
	        Min.EnableCaching();
	        Max.EnableCaching();
	        Target.EnableCaching();
	    }

	    public void Dispose()
	    {
	        Min.Dispose();
	        Max.Dispose();
	        Target.Dispose();
	    }
	}
}