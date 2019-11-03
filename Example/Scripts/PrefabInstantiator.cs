using Fasteraune.SO;
using Generated.Variables;
using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    public GameObject StatusPrefab;
    public TransformVariableReference Parent;
    private GameObject statusPrefabInstance;
    public bool destroyInstanceOnDestroy = true;
    
    void Start()
    {
        statusPrefabInstance = Instantiate(StatusPrefab);
        statusPrefabInstance.GetComponent<RectTransform>().SetParent(Parent.Value);
        statusPrefabInstance.GetComponent<InstanceOwner>().Parent = GetComponent<InstanceOwner>();
    }

    void OnDestroy()
    {
        if (destroyInstanceOnDestroy && statusPrefabInstance != null)
        {
            Destroy(statusPrefabInstance);
        }
    }
}
