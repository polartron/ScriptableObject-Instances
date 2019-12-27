using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fasteraune.SO.Instances
{
    [Serializable]
    public class ScriptableObjectBase : ScriptableObject
    {
        internal Dictionary<InstanceOwner, ScriptableObjectBase> instances = new Dictionary<InstanceOwner, ScriptableObjectBase>();
        private List<InstanceOwner> owners = new List<InstanceOwner>();
        
        public IEnumerator<InstanceOwner> Owners => owners.GetEnumerator();

        internal virtual ScriptableObjectBase GetOrCreateInstance(InstanceOwner connection)
        {
            if (instances.ContainsKey(connection))
            {
                return instances[connection];
            }

            var instance = CreateInstance(GetType().Name) as ScriptableObjectBase;

            if (instance == null)
            {
                Debug.LogError("Could not create instance of type " + GetType().Name);
                return null;
            }

            instances.Add(connection, instance);
            owners.Add(connection);
            connection.Register(instance);
            
            return instances[connection];
        }

        internal virtual ScriptableObjectBase GetInstance(InstanceOwner connection)
        {
            if (instances.ContainsKey(connection))
            {
                return instances[connection];
            }

            return null;
        }
        
        internal void RemoveInstance(InstanceOwner connection)
        {
            if (instances.ContainsKey(connection))
            {
                instances.Remove(connection);
                owners.Remove(connection);
            }
        }


    }
}
