using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using B83.LogicExpressionParser;
using Fasteraune.Variables;
using UnityEngine;

[Serializable]
public class ExpressionFloatReference
{
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
                    
                    if (variable == null)
                    {
                        return 0f;
                    }

                    int copy = i;
                    
                    context[variable.Name].Set(() => GetReferenceValue(copy));
                    numberExpression = parser.ParseNumber(Expression, context);
                }
            }
            
            return Convert.ToSingle(numberExpression.GetNumber());
        }
    }

    private float GetReferenceValue(int index)
    {
        return Variables[index].Reference.Value;
    }
}
