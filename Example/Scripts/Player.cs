using System;
using Fasteraune.SO.Variables;
using Generated.Events;
using Generated.Variables;
using UnityEngine;

namespace Fasteraune.SO.Example
{
    public class Player : MonoBehaviour
    {
        public InstanceOwner instanceOwner;
        public GameObject StatusPrefab;
        public TransformReference StatusPrefabTransform;
        private GameObject statusPrefabInstance;

        public FloatReferenceClamped Health;
        public FloatEventReference OnDamagedEvent;
        public FloatEventReference OnHealedEvent;

        private void Start()
        {
            statusPrefabInstance = Instantiate(StatusPrefab);
            statusPrefabInstance.GetComponent<RectTransform>().SetParent(StatusPrefabTransform.Value);
            statusPrefabInstance.GetComponent<InstanceOwner>().Parent = instanceOwner;
            
            OnDamagedEvent.AddListener(OnDamaged);
            OnHealedEvent.AddListener(OnHealed);
            Health.AddListener(OnHealthChanged);
        }

        private void OnDamaged(float value)
        {
            Health.Value = Health.Value - value;
        }

        private void OnHealed(float value)
        {
            Health.Value = Health.Value + value;
        }

        private void OnHealthChanged(float value)
        {
            if (value <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (statusPrefabInstance != null)
            {
                Destroy(statusPrefabInstance);
            }
            
            OnDamagedEvent.RemoveListener(OnDamaged);
            OnHealedEvent.RemoveListener(OnHealed);
            Health.RemoveListener(OnHealthChanged);
        }
    }
}
