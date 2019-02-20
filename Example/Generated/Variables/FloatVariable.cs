using Fasteraune.Variables;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

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
        var proxy = serializedObject.targetObject as FloatVariableProxy;
        RuntimeValue = proxy.ProxyValue;
    }

#endif
}