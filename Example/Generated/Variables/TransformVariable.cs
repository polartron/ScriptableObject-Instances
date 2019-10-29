using UnityEngine;
using Fasteraune.Variables;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Generated.Variables
{
    [CreateAssetMenu(menuName = "Variables/Transform")]
    public class TransformVariable : Variable<UnityEngine.Transform>
    {
#if UNITY_EDITOR
        private TransformVariableProxy cachedProxy;

        public override SerializedObject GetRuntimeValueWrapper()
        {
            if (cachedProxy == null)
            {
                cachedProxy = CreateInstance(typeof(TransformVariableProxy)) as TransformVariableProxy;
            }
            
            cachedProxy.ProxyValue = Value;
            return new SerializedObject(cachedProxy);
        }
        
        public override SerializedObject GetInitialValueWrapper()
        {
            if (cachedProxy == null)
            {
                cachedProxy = CreateInstance(typeof(TransformVariableProxy)) as TransformVariableProxy;
            }
            
            cachedProxy.ProxyValue = InitialValue;
            return new SerializedObject(cachedProxy);
        }

        public override void ApplyModifiedValue(SerializedObject serializedObject)
        {
            TransformVariableProxy proxy = serializedObject.targetObject as TransformVariableProxy;
            Value = proxy.ProxyValue;
        }
#endif
    }
}