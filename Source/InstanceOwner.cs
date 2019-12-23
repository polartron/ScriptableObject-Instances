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
        public Variable[] LocalVariableOverrides;
        public Tag[] Tags;

        private List<ScriptableObjectBase> connectedScriptableObjects = new List<ScriptableObjectBase>();

        internal void Register(ScriptableObjectBase scriptableObject)
        {
            connectedScriptableObjects.Add(scriptableObject);
        }
        
        internal void UnRegister(ScriptableObjectBase scriptableObject)
        {
            connectedScriptableObjects.Remove(scriptableObject);
        }

        private void Awake()
        {
            foreach (var localVariableOverride in LocalVariableOverrides)
            {
                localVariableOverride.GetOrCreateInstance(this);
            }

            foreach (var tag in Tags)
            {
                tag.GetOrCreateInstance(this);
            }
        }

        private void OnDestroy()
        {
            foreach (var scriptableObject in connectedScriptableObjects)
            {
                if (scriptableObject != null)
                {
                    scriptableObject.ClearConnection(this);
                }
            }
        }

        public VariableReference<TVariableType, TVariable> GetReferenceToVariable<TVariableType, TVariable>(TVariable variable) where TVariable : Variable<TVariableType>
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
        
        public EventReference<TEventType, TEvent> GetReferenceToEvent<TEventType, TEvent>(TEvent eventObject) where TEvent : Event<TEventType>
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
            scriptableObjectBase.RemoveInstance(this);
        }
    }
}