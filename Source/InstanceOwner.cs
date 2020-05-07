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

        public VariableReference<TVariableType, TVariable> GetReferenceToVariable<TVariableType, TVariable>(TVariable variableObject) 
            where TVariable : Variable<TVariableType>
        {
            if (variableObject.GetInstance(this) == null)
            {
                return null;
            }
            
            return new VariableReference<TVariableType, TVariable>()
            {
                Connection = this,
                Variable = variableObject,
                Type = ReferenceType.Instanced
            };
        }
        
        public EventReference<TEventType, TEvent> GetReferenceToEvent<TEventType, TEvent>(TEvent eventObject) 
            where TEvent : Event<TEventType>
        {
            if (eventObject.GetInstance(this) == null)
            {
                return null;
            }
            
            return new EventReference<TEventType, TEvent>()
            {
                Connection = this,
                Type = ReferenceType.Instanced
            };
        }

        public bool HasInstance(ScriptableObjectBase baseObject)
        {
            return baseObject.GetInstance(this) != null;
        }
        
        public void AddInstance(ScriptableObjectBase baseObject)
        {
            baseObject.GetOrCreateInstance(this);
        }
        
        public void RemoveInstance(ScriptableObjectBase baseObject)
        {
            connectedScriptableObjects.Remove(baseObject);
            baseObject.RemoveInstance(this);
        }
    }
}