using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Fasteraune.SO.Instances.Tags.Tests
{
    public class TagsTests : MonoBehaviour
    {
        [Test]
        public void Instance_Owner_Has_Tag()
        {
            var tag = ScriptableObject.CreateInstance(typeof(Tag)) as Tag;
            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstanceOwner>();
            
            instancedVariableOwner.AddInstance(tag);
            
            Assert.IsTrue(instancedVariableOwner.HasInstance(tag));
        }
        
        [Test]
        public void Instance_Owner_Can_Remove_Tag()
        {
            var tag = ScriptableObject.CreateInstance(typeof(Tag)) as Tag;
            var gameObject = new GameObject();
            var instancedVariableOwner = gameObject.AddComponent<InstanceOwner>();
            
            instancedVariableOwner.AddInstance(tag);
            instancedVariableOwner.RemoveInstance(tag);

            Assert.IsTrue(!instancedVariableOwner.HasInstance(tag));
        }
    }
}