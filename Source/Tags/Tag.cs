using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Fasteraune.SO.Instances.Tags
{
    public abstract class Tag : ScriptableObjectBase
    {
        internal Tag GetOrCreateInstancedTag(InstanceOwner connection)
        {
            if (instances.ContainsKey(connection))
            {
                return instances[connection] as Tag;
            }

            Tag instance = CreateInstance(GetType().Name) as Tag;

            if (instance == null)
            {
                Debug.LogError("Could not create instance of type " + GetType().Name);
                return null;
            }
            
            instances.Add(connection, instance);
            connection.Register(instance);
            return instances[connection] as Tag;
        }

        internal Tag GetInstancedTag(InstanceOwner connection)
        {
            if (instances.ContainsKey(connection))
            {

                return instances[connection] as Tag;
            }

            return null;
        }
    }

}
