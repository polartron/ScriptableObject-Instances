using System;
using UnityEngine;

namespace Fasteraune.SO.Instances.Events
{
    [Serializable]
    public abstract class Event : ScriptableObjectBase
    {
    }

    [Serializable]
    public class Event<T> : Event
    {
        public event Action<T> OnEvent;

        public Event<T> GetOrCreateInstancedVariable(InstanceOwner connection)
        {
            if (instances.ContainsKey(connection))
            {
                return instances[connection] as Event<T>;
            }

            var instance = CreateInstance(GetType().Name) as Event<T>;

            if (instance == null)
            {
                Debug.LogError("Could not create instance of type " + GetType().Name);
                return null;
            }

            instances.Add(connection, instance);
            connection.Register(instance);
            return instances[connection] as Event<T>;
        }

        public void Invoke(T value)
        {
            OnEvent?.Invoke(value);
        }
    }
}
