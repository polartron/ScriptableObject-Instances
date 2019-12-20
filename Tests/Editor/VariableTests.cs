using Generated.Variables;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Fasteraune.SO.Instances.Variables.Tests
{
    public class VariableTests
    {
        [Test]
        public void Constant_Value_Changed_Callback()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            
            var floatReference = new FloatVariableReference();
            floatReference.Type = VariableReference.ReferenceType.ConstantValue;
            floatReference.Variable = floatVariableObject;

            var changedValue = 0f;

            floatReference.AddListener(delegate(float value) { changedValue = value; });

            floatReference.Value = 10f;

            Assert.AreEqual(10f, changedValue);
            Assert.AreEqual(10f, floatReference.Value);
        }

        [Test]
        public void Shared_Variable_Changed_Callback()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;

            var floatReference = new FloatVariableReference();
            floatReference.Type = VariableReference.ReferenceType.SharedReference;
            floatReference.Variable = floatVariableObject;

            var changedValue = 0f;

            floatReference.AddListener(delegate(float value) { changedValue = value; });

            floatReference.Value = 10f;

            Assert.AreEqual(10f, changedValue);
            Assert.AreEqual(10f, floatReference.Value);
        }

        [Test]
        public void Instanced_Variable_Changed_Callback()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            
            var floatReference = new FloatVariableReference();
            floatReference.Type = VariableReference.ReferenceType.InstancedReference;
            floatReference.Variable = floatVariableObject;

            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstanceOwner>();
            floatReference.Connection = instancedVariableOwner;
            
            var changedValue = 0f;

            floatReference.AddListener(delegate(float value) { changedValue = value; });

            floatReference.Value = 10f;

            Assert.AreEqual(10f, changedValue);
            Assert.AreEqual(10f, floatReference.Value);
        }

        [Test]
        public void Shared_Variable_Reference_Same_Value()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            
            var floatReference = new FloatVariableReference();
            floatReference.Type = VariableReference.ReferenceType.SharedReference;
            floatReference.Variable = floatVariableObject;

            var secondFloatReference = new FloatVariableReference();
            secondFloatReference.Type = VariableReference.ReferenceType.SharedReference;
            secondFloatReference.Variable = floatVariableObject;

            floatReference.Value = 10f;

            Assert.AreEqual(10f, secondFloatReference.Value);
            Assert.AreEqual(10f, floatReference.Value);
        }

        [Test]
        public void Instanced_Variable_Error_On_Missing_InstancedVariableOwner()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            
            var floatReference = new FloatVariableReference();
            floatReference.Type = VariableReference.ReferenceType.InstancedReference;
            floatReference.Variable = floatVariableObject;

            var secondFloatReference = new FloatVariableReference();
            secondFloatReference.Type = VariableReference.ReferenceType.InstancedReference;
            secondFloatReference.Variable = floatVariableObject;

            Debug.Log("---IGNORE ERROR MESSAGES BELOW---");
            floatReference.Value = 10f;
            LogAssert.Expect(LogType.Error, "Missing reference to InstancedVariableOwner script");
            secondFloatReference.Value = 10f;
            LogAssert.Expect(LogType.Error, "Missing reference to InstancedVariableOwner script");
            Debug.Log("---IGNORE ERROR MESSAGES ABOVE---");
        }

        [Test]
        public void Instanced_Variable_Error_On_Missing_Variable()
        {
            var floatReference = new FloatVariableReference();
            floatReference.Type = VariableReference.ReferenceType.InstancedReference;

            var secondFloatReference = new FloatVariableReference();
            secondFloatReference.Type = VariableReference.ReferenceType.InstancedReference;

            Debug.Log("---IGNORE ERROR MESSAGES BELOW---");
            floatReference.Value = 10f;
            LogAssert.Expect(LogType.Error, "Missing reference to variable asset");
            secondFloatReference.Value = 10f;
            LogAssert.Expect(LogType.Error, "Missing reference to variable asset");
            Debug.Log("---IGNORE ERROR MESSAGES ABOVE---");
        }

        [Test]
        public void Instanced_Variable_Change_Instanced_Value_Not_Shared()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            
            var floatReference = new FloatVariableReference();
            floatReference.Type = VariableReference.ReferenceType.InstancedReference;
            floatReference.Variable = floatVariableObject;

            var secondFloatReference = new FloatVariableReference();
            secondFloatReference.Type = VariableReference.ReferenceType.SharedReference;
            secondFloatReference.Variable = floatVariableObject;

            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstanceOwner>();
            floatReference.Connection = instancedVariableOwner;

            floatReference.Value = 10f;
            Assert.AreEqual(10f, floatReference.Value);
            secondFloatReference.Value = 20f;
            Assert.AreEqual(10f, floatReference.Value);
            Assert.AreEqual(20f, secondFloatReference.Value);
            floatReference.Value = 30f;
            Assert.AreEqual(20f, secondFloatReference.Value);
        }

        [Test]
        public void Instanced_Variable_With_Parent_Connection_Change_Same_Value()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            
            var floatReference = new FloatVariableReference();
            floatReference.Type = VariableReference.ReferenceType.InstancedReference;
            floatReference.Variable = floatVariableObject;

            var secondFloatReference = new FloatVariableReference();
            secondFloatReference.Type = VariableReference.ReferenceType.InstancedReference;
            secondFloatReference.Variable = floatVariableObject;

            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstanceOwner>();
            floatReference.Connection = instancedVariableOwner;
            secondFloatReference.Connection = instancedVariableOwner;

            floatReference.Value = 10f;
            Assert.AreEqual(10f, floatReference.Value);
            Assert.AreEqual(10f, secondFloatReference.Value);

            secondFloatReference.Value = 20f;
            Assert.AreEqual(20f, floatReference.Value);
            Assert.AreEqual(20f, secondFloatReference.Value);
        }
    }
}