using Fasteraune.Variables;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class PlayerDataVariable : Variable<Player.PlayerData>
{
#if UNITY_EDITOR

    private PlayerDataVariableProxy cachedProxy;

    public override SerializedObject GetRuntimeValueWrapper()
    {
        if (cachedProxy == null)
        {
            cachedProxy = CreateInstance(typeof(PlayerDataVariableProxy)) as PlayerDataVariableProxy;
        }

        cachedProxy.ProxyValue = Value;
        return new SerializedObject(cachedProxy);
    }

    public override SerializedObject GetInitialValueWrapper()
    {
        if (cachedProxy == null)
        {
            cachedProxy = CreateInstance(typeof(PlayerDataVariableProxy)) as PlayerDataVariableProxy;
        }

        cachedProxy.ProxyValue = InitialValue;
        return new SerializedObject(cachedProxy);
    }

    public override void ApplyModifiedValue(SerializedObject serializedObject)
    {
        var proxy = serializedObject.targetObject as PlayerDataVariableProxy;
        RuntimeValue = proxy.ProxyValue;
    }

#endif
}