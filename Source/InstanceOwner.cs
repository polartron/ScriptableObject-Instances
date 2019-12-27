using System.Collections.Generic;
using Fasteraune.SO.Instances.Events;
using Fasteraune.SO.Instances.Tags;
using Fasteraune.SO.Instances.Variables;
using UnityEngine;

namespace Fasteraune.SO.Instances
{
    public class InstanceOwner : MonoBehaviour
    {
        public InstanceOwner Parent;
        public ScriptableObjectBase[] InitialObjects;

        private List<ScriptableObjectBase> connectedScriptableObjects = new List<ScriptableObjectBase>();

        internal void Register(ScriptableObjectBase scriptableObject)
        {
            connectedScriptableObjects.Add(scriptableObject);
        }

        private void Awake()
        {
            foreach (var initialObject in InitialObjects)
            {
                initialObject.GetOrCreateInstance(this);
            }
        }

        private void OnDestroy()
        {
            foreach (var scriptableObject in connectedScriptableObjects)
            {
                if (scriptableObject != null)
                {
                    scriptableObject.RemoveInstance(this);
                }
            }
        }

        public VariableReference<TVariableType, TVariable> GetReferenceToVariable<TVariableType, TVariable>(TVariable variable) 
            where TVariable : Variable<TVariableType>
        {
            if (variable.GetInstance(this) == null)
            {
                return null;
            }
            
            return new VariableReference<TVariableType, TVariable>()
            {
                Connection = this,
                Variable = variable,
                Type = ReferenceType.Instanced
            };
        }
        
        public EventReference<TEventType, TEvent> GetReferenceToEvent<TEventType, TEvent>(TEvent @event) 
            where TEvent : Event<TEventType>
        {
            if (@event.GetInstance(this) == null)
            {
                return null;
            }
            
            return new EventReference<TEventType, TEvent>()
            {
                Connection = this,
                Type = ReferenceType.Instanced
            };
        }

        public bool HasInstance(ScriptableObjectBase scriptableObjectBase)
        {
            return scriptableObjectBase.GetInstance(this) != null;
        }
        
        public void AddInstance(ScriptableObjectBase scriptableObjectBase)
        {
            scriptableObjectBase.GetOrCreateInstance(this);
        }
        
        public void RemoveInstance(ScriptableObjectBase scriptableObjectBase)
        {
            connectedScriptableObjects.Remove(scriptableObjectBase);
            scriptableObjectBase.RemoveInstance(this);
        }
    }
}