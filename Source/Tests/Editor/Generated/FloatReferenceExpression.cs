using System;
using B83.LogicExpressionParser;
using Fasteraune.Variables;

namespace Fasteraune.Variables
{
	[Serializable]
	public class FloatReferenceExpression : IDisposable
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
	            return (double) reference.Value;
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
	                        return default(float);
	                    }
	                    
	                    context[variable.Name].Set(new ExpressionNumberProvider(variable.Reference));
	                    numberExpression = parser.ParseNumber(Expression, context);
	                }
	            }
	            
	            return (float) numberExpression.GetNumber();
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
	public class FloatReferenceAddded : OperationFloatReference
	{
	    public override float Value()
	    {
	        return (float) (First + Second);
	    }
	}

	[Serializable]
	public class FloatReferenceMultiplied : OperationFloatReference
	{
	    public override float Value()
	    {
	        return (float) (First * Second);
	    }
	}

	[Serializable]
	public class FloatReferenceSubtracted : OperationFloatReference
	{
	    public override float Value()
	    {
	        return (float) (First - Second);
	    }
	}

	[Serializable]
	public class FloatReferenceDivided : OperationFloatReference
	{
	    public override float Value()
	    {
	        return (float) (First / Second);
	    }
	}
}