using System;
using B83.LogicExpressionParser;
using Fasteraune.SO.Variables;

namespace Generated.Variables
{
    [Serializable]
    public class FloatVariableReferenceExpression : IDisposable
    {
        public class ExpressionNumberProvider : INumberProvider
        {
            private FloatVariableReference reference;
            
            public ExpressionNumberProvider(FloatVariableReference reference)
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
            public FloatVariableReference Reference;
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
        public FloatVariableReference First = new FloatVariableReference();
        public FloatVariableReference Second = new FloatVariableReference();

        public abstract float Value { get; }

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
    public class FloatReferenceAdded : OperationFloatReference
    {
        public override float Value => First + Second;
    }

    [Serializable]
    public class FloatReferenceSubtracted : OperationFloatReference
    {
        public override float Value => First - Second;
    }

    [Serializable]
    public class FloatReferenceMultiplied : OperationFloatReference
    {
        public override float Value => First * Second;
    }

    [Serializable]
    public class FloatReferenceDivided : OperationFloatReference
    {
        public override float Value => First / Second;
    }
}