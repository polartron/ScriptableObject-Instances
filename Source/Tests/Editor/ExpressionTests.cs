using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Fasteraune.Variables.Tests
{
    public class ExpressionTests
    {
        private void Clamped_Reference_Clamps(FloatReference first, FloatReference second,
            FloatReferenceClamped clamped)
        {
            first.Value = 0;
            second.Value = 20;

            clamped.Min = first;
            clamped.Max = second;

            Assert.AreEqual(first.Value, clamped.Value);
            clamped.Value = 50;
            Assert.AreEqual(second.Value, clamped.Value);
        }

        [Test]
        public void Constant_Reference_Clamps()
        {
            var clamped = new FloatReferenceClamped();

            var first = new FloatReference();
            var second = new FloatReference();

            first.Type = ReferenceType.ConstantValue;
            second.Type = ReferenceType.ConstantValue;

            Clamped_Reference_Clamps(first, second, clamped);
        }

        [Test]
        public void Shared_Reference_Clamps()
        {
            var clamped = new FloatReferenceClamped();

            var firstVariable = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            var secondVariable = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;

            var first = new FloatReference();
            var second = new FloatReference();

            first.Variable = firstVariable;
            second.Variable = secondVariable;

            first.Type = ReferenceType.SharedReference;
            second.Type = ReferenceType.SharedReference;

            Clamped_Reference_Clamps(first, second, clamped);
        }

        [Test]
        public void Instanced_Reference_Clamps()
        {
            var clamped = new FloatReferenceClamped();

            var firstVariable = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            var secondVariable = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;

            var first = new FloatReference();
            var second = new FloatReference();

            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstancedVariableOwner>();
            first.Connection = instancedVariableOwner;
            second.Connection = instancedVariableOwner;

            first.Variable = firstVariable;
            second.Variable = secondVariable;

            first.Type = ReferenceType.InstancedReference;
            second.Type = ReferenceType.InstancedReference;

            Clamped_Reference_Clamps(first, second, clamped);
        }

        private void Assert_Instanced_Reference_Expression(float a, float b, string expression, float expected)
        {
            var floatReferenceExpression = new FloatReferenceExpression();

            var firstVariable = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            var secondVariable = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;

            var first = new FloatReference();
            var second = new FloatReference();

            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstancedVariableOwner>();
            
            first.Connection = instancedVariableOwner;
            second.Connection = instancedVariableOwner;

            first.Variable = firstVariable;
            second.Variable = secondVariable;

            first.Type = ReferenceType.InstancedReference;
            second.Type = ReferenceType.InstancedReference;

            first.Value = a;
            second.Value = b;

            floatReferenceExpression.Variables = new[]
            {
                new FloatReferenceExpression.ExpressionVariables()
                {
                    Name = "a",
                    Reference = first
                },
                new FloatReferenceExpression.ExpressionVariables()
                {
                    Name = "b",
                    Reference = second
                }
            };

            floatReferenceExpression.Expression = expression;
            Assert.AreEqual(expected, floatReferenceExpression.Value);
        }

        [Test]
        public void Instanced_Reference_Expression_Add()
        {
            Assert_Instanced_Reference_Expression(10, 2, "a + b", 12);
        }
        
        [Test]
        public void Instanced_Reference_Expression_Subtract()
        {
            Assert_Instanced_Reference_Expression(10, 2, "a - b", 8);
        }
        
        [Test]
        public void Instanced_Reference_Expression_Multiply()
        {
            Assert_Instanced_Reference_Expression(10, 2, "a * b", 20);
        }
        
        [Test]
        public void Instanced_Reference_Expression_Divide()
        {
            Assert_Instanced_Reference_Expression(10, 2, "a / b", 5);
        }
        
        [Test]
        public void Instanced_Reference_Expression_Pow()
        {
            Assert_Instanced_Reference_Expression(10, 2, "a ^ b", 100);
        }
        
        [Test]
        public void Instanced_Reference_Expression_Complex()
        {
            Assert_Instanced_Reference_Expression(10, 2, "((a + b) / 2) - b", 4);
        }
    }
}
