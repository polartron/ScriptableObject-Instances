using System;
using B83.LogicExpressionParser;
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

[Serializable]
public class ExpressionFloatReference : IDisposable
{
    public class ExpressionNumberProvider : INumberProvider
    {
        private FloatReference reference;
        
        public ExpressionNumberProvider(FloatReference reference)
        {
            this.reference = reference;
        }

        public double GetNumber()
        {
            return reference.Value;
        }
    }

    [Serializable]
    public class ExpressionVariables
    {
        public string Name;
        public FloatReference Reference;
    }
    
    public string Expression;
    public ExpressionVariables[] Variables;

    private Parser parser;
    private NumberExpression numberExpression;
    private ExpressionContext context;

    public float Value
    {
        get
        {
            if (parser == null)
            {
                parser = new Parser();
        
                context = new ExpressionContext();
                
                for (int i = 0; i < Variables.Length; i++)
                {
                    var variable = Variables[i];
                    
                    if (variable == null || variable.Reference == null)
                    {
                        return 0f;
                    }
                    
                    context[variable.Name].Set(new ExpressionNumberProvider(variable.Reference));
                    numberExpression = parser.ParseNumber(Expression, context);
                }
            }
            
            return Convert.ToSingle(numberExpression.GetNumber());
        }
    }

    public void EnableCaching()
    {
        for (int i = 0; i < Variables.Length; i++)
        {
            var variable = Variables[i];
            
            variable?.Reference?.EnableCaching();
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < Variables.Length; i++)
        {
            var variable = Variables[i];
            
            variable?.Reference?.Dispose();
        }
    }
}

[Serializable]
public abstract class OperationFloatReference : IDisposable
{
    public FloatReference First = new FloatReference();
    public FloatReference Second = new FloatReference();

    public abstract float Value();

    public void EnableCaching()
    {
        First.EnableCaching();
        Second.EnableCaching();
    }

    public void Dispose()
    {
        First.Dispose();
        Second.Dispose();
    }
}

[Serializable]
public class AdddedFloatReference : OperationFloatReference
{
    public override float Value()
    {
        return First + Second;
    }
}

[Serializable]
public class MultipliedFloatReference : OperationFloatReference
{
    public override float Value()
    {
        return First * Second;
    }
}

[Serializable]
public class SubtractedFloatReference : OperationFloatReference
{
    public override float Value()
    {
        return First + Second;
    }
}

[Serializable]
public class DividedFloatReference : OperationFloatReference
{
    public override float Value()
    {
        return First + Second;
    }
}

[Serializable]
public class ClampedFloatReference : IDisposable
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