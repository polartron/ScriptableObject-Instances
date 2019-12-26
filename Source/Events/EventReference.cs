using System;
using UnityEngine;

namespace Fasteraune.SO.Instances.Events
{
    [Serializable]
    public abstract class EventReference
    {

        public ReferenceType Type = ReferenceType.Shared;
        public InstanceOwner Connection;
    }

    [Serializable]
    public class EventReference<TEventType, TEvent> : EventReference 
        where TEvent : Event<TEventType>
    {
        public TEvent Event;

        private Event<TEventType> instancedEvent;
        private Event<TEventType> InstancedEvent
        {
            get
            {
                if (instancedEvent == null)
                {
                    var connection = Connection.Parent ? Connection.Parent : Connection;
                    instancedEvent = Event.GetOrCreateInstance(connection) as Event<TEventType>;
                }

                return instancedEvent;
            }
        }

        public virtual void Invoke(TEventType value)
        {
            switch (Type)
            {
                case ReferenceType.Shared:
                {
                    if (VariableReferenceMissing())
                    {
                        return;
                    }
                    
                    Event.Invoke(value);
                    return;
                }

                case ReferenceType.Instanced:
                {
                    if (VariableReferenceMissing() || ConnectionReferenceMissing())
                    {
                        return;
                    }

                    InstancedEvent.Invoke(value);
                    
                    return;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual void AddListener(Action<TEventType> listener)
        {
            switch (Type)
            {
                case ReferenceType.Shared:
                {
                    if (VariableReferenceMissing())
                    {
                        return;
                    }

                    Event.OnEvent += listener;
                    break;
                }

                case ReferenceType.Instanced:
                {
                    if (VariableReferenceMissing() || ConnectionReferenceMissing())
                    {
                        return;
                    }

                    InstancedEvent.OnEvent += listener;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual void RemoveListener(Action<TEventType> listener)
        {
            switch (Type)
            {
                case ReferenceType.Shared:
                {
                    if (VariableReferenceMissing())
                    {
                        return;
                    }

                    Event.OnEvent -= listener;
                    break;
                }

                case ReferenceType.Instanced:
                {
                    if (VariableReferenceMissing() || ConnectionReferenceMissing())
                    {
                        return;
                    }

                    InstancedEvent.OnEvent -= listener;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool VariableReferenceMissing()
        {
            if (Event == null)
            {
                Debug.LogError("Missing reference to variable asset");
                return true;
            }

            return false;
        }

        private bool ConnectionReferenceMissing()
        {
            if (Connection == null)
            {
                Debug.LogError("Missing reference to InstancedVariableOwner script");
                return true;
            }

            return false;
        }
    }
}