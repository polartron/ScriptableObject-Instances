using System;
using Fasteraune.Variables;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Serializable]
    public class PlayerData : IComparable
    {
        public Color PlayerColor = Color.white;
        public float RegenerationRate = 1f;
        public int Experience = 0;
        public int Level = 0;
        
        public int CompareTo(object obj)
        {
            PlayerData data = obj as PlayerData;
            return Level - data.Level;
        }
    }

    [SerializeField] private PlayerDataReference playerData;
    [SerializeField] private ClampedFloatReference Health;
    [SerializeField] private ExpressionFloatReference Expression;

    public InstancedVariableOwner Connection;
    public GameObject StatusPrefab;

    private GameObject statusPrefabInstance;

    private void Start()
    {
        statusPrefabInstance = Instantiate(StatusPrefab);

        //Hack
        var statusPanelRectTransform = GameObject.Find("Status Panel").GetComponent<RectTransform>();
        statusPrefabInstance.GetComponent<RectTransform>().SetParent(statusPanelRectTransform);

        //Set the parent
        statusPrefabInstance.GetComponent<InstancedVariableOwner>().Parent = Connection;
    }

    private void OnDestroy()
    {
        if (statusPrefabInstance != null)
        {
            Destroy(statusPrefabInstance);
        }
    }

    private void Update()
    {
        float regenRate = playerData.Value.RegenerationRate;
        Health.Value = Health.Value + regenRate;
        float a = Expression.Value;
    }
}