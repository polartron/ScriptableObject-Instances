using System;
using Fasteraune.Variables;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class VariableTests
    {
        [Serializable]
        private class FloatVariable : Variable<float>
        {
        }

        [Serializable]
        private class FloatReference : BaseReference<float, FloatVariable>
        {
            public FloatReference(float Value) : base(Value)
            {
            }

            public FloatReference()
            {
            }
        }

        // A Test behaves as an ordinary method
        [Test]
        public void Constant_Value_Changed_Callback()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            Assert.IsNotNull(floatVariableObject);

            var floatReference = new FloatReference();
            floatReference.Type = ReferenceType.ConstantValue;
            floatReference.Variable = floatVariableObject;

            var changedValue = 0f;

            floatReference.AddListener(delegate(float value) { changedValue = value; });

            floatReference.Value = 10f;

            Assert.AreEqual(changedValue, 10f);
            Assert.AreEqual(floatReference.Value, 10f);
        }

        [Test]
        public void Shared_Variable_Changed_Callback()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            Assert.IsNotNull(floatVariableObject);

            var floatReference = new FloatReference();
            floatReference.Type = ReferenceType.SharedReference;
            floatReference.Variable = floatVariableObject;

            var changedValue = 0f;

            floatReference.AddListener(delegate(float value) { changedValue = value; });

            floatReference.Value = 10f;

            Assert.AreEqual(changedValue, 10f);
            Assert.AreEqual(floatReference.Value, 10f);
        }

        [Test]
        public void Instanced_Variable_Changed_Callback()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            Assert.IsNotNull(floatVariableObject);

            var floatReference = new FloatReference();
            floatReference.Type = ReferenceType.SharedReference;
            floatReference.Variable = floatVariableObject;

            var changedValue = 0f;

            floatReference.AddListener(delegate(float value) { changedValue = value; });

            floatReference.Value = 10f;

            Assert.AreEqual(changedValue, 10f);
            Assert.AreEqual(floatReference.Value, 10f);
        }

        [Test]
        public void Shared_Variable_Reference_Same_Value()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            Assert.IsNotNull(floatVariableObject);

            var floatReference = new FloatReference();
            floatReference.Type = ReferenceType.SharedReference;
            floatReference.Variable = floatVariableObject;

            var secondFloatReference = new FloatReference();
            secondFloatReference.Type = ReferenceType.SharedReference;
            secondFloatReference.Variable = floatVariableObject;

            floatReference.Value = 10f;

            Assert.AreEqual(secondFloatReference.Value, 10f);
            Assert.AreEqual(floatReference.Value, 10f);
        }

        [Test]
        public void Instanced_Variable_Error_On_Missing_InstancedVariableOwner()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            Assert.IsNotNull(floatVariableObject);

            var floatReference = new FloatReference();
            floatReference.Type = ReferenceType.InstancedReference;
            floatReference.Variable = floatVariableObject;

            var secondFloatReference = new FloatReference();
            secondFloatReference.Type = ReferenceType.InstancedReference;
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
            var floatReference = new FloatReference();
            floatReference.Type = ReferenceType.InstancedReference;

            var secondFloatReference = new FloatReference();
            secondFloatReference.Type = ReferenceType.InstancedReference;

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
            Assert.IsNotNull(floatVariableObject);

            var floatReference = new FloatReference();
            floatReference.Type = ReferenceType.InstancedReference;
            floatReference.Variable = floatVariableObject;

            var secondFloatReference = new FloatReference();
            secondFloatReference.Type = ReferenceType.SharedReference;
            secondFloatReference.Variable = floatVariableObject;

            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstancedVariableOwner>();
            floatReference.Connection = instancedVariableOwner;

            floatReference.Value = 10f;
            Assert.AreEqual(floatReference.Value, 10f);
            secondFloatReference.Value = 20f;
            Assert.AreEqual(floatReference.Value, 10f);
            Assert.AreEqual(secondFloatReference.Value, 20f);
            floatReference.Value = 30f;
            Assert.AreEqual(secondFloatReference.Value, 20f);
        }

        [Test]
        public void Instanced_Variable_With_Parent_Connection_Change_Same_Value()
        {
            var floatVariableObject = ScriptableObject.CreateInstance(typeof(FloatVariable)) as FloatVariable;
            Assert.IsNotNull(floatVariableObject);

            var floatReference = new FloatReference();
            floatReference.Type = ReferenceType.InstancedReference;
            floatReference.Variable = floatVariableObject;

            var secondFloatReference = new FloatReference();
            secondFloatReference.Type = ReferenceType.InstancedReference;
            secondFloatReference.Variable = floatVariableObject;

            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstancedVariableOwner>();
            floatReference.Connection = instancedVariableOwner;
            secondFloatReference.Connection = instancedVariableOwner;

            floatReference.Value = 10f;
            Assert.AreEqual(floatReference.Value, 10f);
            Assert.AreEqual(secondFloatReference.Value, 10f);

            secondFloatReference.Value = 20f;
            Assert.AreEqual(floatReference.Value, 20f);
            Assert.AreEqual(secondFloatReference.Value, 20f);
        }
    }
}