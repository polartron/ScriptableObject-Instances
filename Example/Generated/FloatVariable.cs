using UnityEngine;
using Fasteraune.SO.Instances.Variables;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Generated.Variables
{
    [CreateAssetMenu(menuName = "Variables/Float")]
    public class FloatVariable : Variable<float>
    {
#if UNITY_EDITOR
        private FloatVariableProxy cachedProxy;

        public override SerializedObject GetRuntimeValueWrapper()
        {
            if (cachedProxy == null)
            {
                cachedProxy = CreateInstance(typeof(FloatVariableProxy)) as FloatVariableProxy;
            }
            
            cachedProxy.ProxyValue = Value;
            return new SerializedObject(cachedProxy);
        }
        
        public override SerializedObject GetInitialValueWrapper()
        {
            if (cachedProxy == null)
            {
                cachedProxy = CreateInstance(typeof(FloatVariableProxy)) as FloatVariableProxy;
            }
            
            cachedProxy.ProxyValue = InitialValue;
            return new SerializedObject(cachedProxy);
        }

        public override void ApplyModifiedValue(SerializedObject serializedObject)
        {
            FloatVariableProxy proxy = serializedObject.targetObject as FloatVariableProxy;
            Value = proxy.ProxyValue;
        }
#endif
    }
}