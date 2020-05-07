using System;
using UnityEngine;

namespace Fasteraune.SO.Instances.Events
{
    [Serializable]
    public class Event : ScriptableObjectBase
    {
    }

    [Serializable]
    public class Event<T> : Event
    {
        public event Action<T> OnEvent;

        public void Invoke(T value)
        {
            OnEvent?.Invoke(value);
        }
    }
}
