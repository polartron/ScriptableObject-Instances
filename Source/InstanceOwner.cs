using System.Collections.Generic;
using Fasteraune.SO.Instances.Variables;
using UnityEngine;

namespace Fasteraune.SO.Instances
{
    public class InstanceOwner : MonoBehaviour
    {
        public InstanceOwner Parent;
        public Variable[] LocalVariableOverrides;

        private List<ScriptableObjectBase> connectedScriptableObjects = new List<ScriptableObjectBase>();

        internal void Register(ScriptableObjectBase scriptableObject)
        {
            connectedScriptableObjects.Add(scriptableObject);
        }

        private void Awake()
        {
            foreach (var localScriptableObject in LocalVariableOverrides)
            {
                localScriptableObject.GetOrCreateInstancedVariable(this);
            }
        }

        private void OnDestroy()
        {
            foreach (var scriptableObjects in connectedScriptableObjects)
            {
                if (scriptableObjects != null)
                {
                    scriptableObjects.ClearConnection(this);
                }
            }
        }

        public bool HasInstanceOf(Variable variable)
        {
            if (variable.GetInstancedVariable(this) == null)
            {
                return false;
            }

            return true;
        }

        public VariableReference<TVariableType, TVariable> GetReferenceTo<TVariableType, TVariable>(TVariable variable) where TVariable : Variable<TVariableType>
        {
            if (variable.GetInstancedVariable(this) == null)
            {
                return null;
            }
            
            return new VariableReference<TVariableType, TVariable>()
            {
                Connection = this,
                Variable = variable,
                Type = ReferenceType.InstancedReference
            };
        }
    }
}