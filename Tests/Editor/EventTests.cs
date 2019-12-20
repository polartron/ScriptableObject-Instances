using System.Collections;
using System.Collections.Generic;
using Generated.Events;
using NUnit.Framework;
using UnityEngine;

namespace Fasteraune.SO.Instances.Events.Tests
{
    public class EventTests
    {
        [Test]
        public void Shared_Event_Invoked()
        {
            var floatEvent = ScriptableObject.CreateInstance(typeof(FloatEvent)) as FloatEvent;
            
            var floatEventReference = new FloatEventReference();
            floatEventReference.Type = EventReference.ReferenceType.SharedReference;
            floatEventReference.Event = floatEvent;

            bool pass = false;
            float passedValue = 0f;
            
            floatEventReference.AddListener(delegate(float f) 
            { 
                pass = true;
                passedValue = f;
            });
            
            floatEventReference.Invoke(5f);
            
            Assert.IsTrue(pass);
            Assert.AreEqual(5f, passedValue);
        }
        
        [Test]
        public void Instanced_Event_Invoked()
        {
            var floatEvent = ScriptableObject.CreateInstance(typeof(FloatEvent)) as FloatEvent;
            
            var floatEventReference = new FloatEventReference();
            floatEventReference.Type = EventReference.ReferenceType.InstancedReference;
            floatEventReference.Event = floatEvent;

            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstanceOwner>();
            floatEventReference.Connection = instancedVariableOwner;
            
            bool pass = false;
            float passedValue = 0f;
            
            floatEventReference.AddListener(delegate(float f) 
            { 
                pass = true;
                passedValue = f;
            });
            
            floatEventReference.Invoke(5f);
            
            Assert.IsTrue(pass);
            Assert.AreEqual(5f, passedValue);
        }
        
        [Test]
        public void Instanced_Event_Invoked_With_Parent()
        {
            var floatEvent = ScriptableObject.CreateInstance(typeof(FloatEvent)) as FloatEvent;
            
            var floatEventReference = new FloatEventReference();
            floatEventReference.Type = EventReference.ReferenceType.InstancedReference;
            floatEventReference.Event = floatEvent;

            var firstGameObject = new GameObject();
            var firstInstancedVariableOwner = firstGameObject.AddComponent<InstanceOwner>();
            floatEventReference.Connection = firstInstancedVariableOwner;

            var secondEventReference = new FloatEventReference();
            secondEventReference.Type = EventReference.ReferenceType.InstancedReference;
            secondEventReference.Event = floatEvent;
            
            var secondGameObject = new GameObject();
            var secondInstancedVariableOwner = firstGameObject.AddComponent<InstanceOwner>();
            secondInstancedVariableOwner.Parent = firstInstancedVariableOwner;
            secondEventReference.Connection = secondInstancedVariableOwner;

            bool pass = false;
            float passedValue = 0f;
            
            secondEventReference.AddListener(delegate(float f) 
            { 
                pass = true;
                passedValue = f;
            });
            
            floatEventReference.Invoke(5f);
            
            Assert.IsTrue(pass);
            Assert.AreEqual(5f, passedValue);
        }
    }
}

