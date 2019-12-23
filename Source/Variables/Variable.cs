using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Fasteraune.SO.Instances.Variables
{
    [Serializable]
    public abstract class Variable : ScriptableObjectBase
    {
        
#if UNITY_EDITOR
        public virtual SerializedObject GetRuntimeValueWrapper()
        {
            return null;
        }

        public virtual void ApplyModifiedValue(SerializedObject serializedObject)
        {
        }

        internal abstract void SaveRuntimeValue(Variable target);
#endif
    }

    [Serializable]
    public class Variable<T> : Variable, ISerializationCallbackReceiver
    {
        public Variable Base;
        public T InitialValue;
        [NonSerialized] public T RuntimeValue;

        public event Action<T> OnValueChanged;

        internal override ScriptableObjectBase GetOrCreateInstance(InstanceOwner connection)
        {
            if (instances.ContainsKey(connection))
            {
                return instances[connection] as Variable<T>;
            }

            Variable<T> instance = (Base != null)
                ? Base.GetOrCreateInstance(connection) as Variable<T>
                : CreateInstance(GetType().Name) as Variable<T>;

            if (instance == null)
            {
                Debug.LogError("Could not create instance of type " + GetType().Name);
                return null;
            }

            instance.InitialValue = InitialValue;
            instance.RuntimeValue = InitialValue;
            instances.Add(connection, instance);
            connection.Register(instance);
            
            return instances[connection];
        }

        internal override ScriptableObjectBase GetInstance(InstanceOwner connection)
        {
            if (instances.ContainsKey(connection))
            {
                if (Base != null)
                {
                    return Base.GetInstance(connection);
                }
                
                return instances[connection] as Variable<T>;
            }

            return null;
        }

        public T Value
        {
            get { return RuntimeValue; }
            set
            {
                RuntimeValue = value;

                if (OnValueChanged != null)
                {
                    OnValueChanged.Invoke(value);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            RuntimeValue = InitialValue;
        }

        public void OnBeforeSerialize()
        {
        }

#if UNITY_EDITOR
        internal override void SaveRuntimeValue(Variable target)
        {
            var targetConverted = target as Variable<T>;

            if (targetConverted == null)
            {
                return;
            }

            InitialValue = targetConverted.RuntimeValue;
            EditorUtility.SetDirty(this);

            Debug.Log("Saved " + InitialValue + " to " + name);
        }
#endif
    }
}