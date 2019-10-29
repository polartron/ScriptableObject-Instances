using System;
using Fasteraune.Variables;
using Generated.Variables;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InstancedVariableOwner instancedVariableOwner;
    public GameObject StatusPrefab;
    public TransformReference StatusPrefabTransform;
    private GameObject statusPrefabInstance;

    private void Start()
    {
        statusPrefabInstance = Instantiate(StatusPrefab);
        statusPrefabInstance.GetComponent<RectTransform>().SetParent(StatusPrefabTransform.Value);
        statusPrefabInstance.GetComponent<InstancedVariableOwner>().Parent = instancedVariableOwner;
    }

    private void OnDestroy()
    {
        if (statusPrefabInstance != null)
        {
            Destroy(statusPrefabInstance);
        }
    }
}