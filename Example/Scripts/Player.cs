using System;
using Fasteraune.SO.Variables;
using Generated.Events;
using Generated.Variables;
using UnityEngine;

namespace Fasteraune.SO.Example
{
    public class Player : MonoBehaviour
    {
        public FloatVariableReferenceClamped Health;
        public FloatEventReference OnDamagedEvent;
        public FloatEventReference OnHealedEvent;

        private void Start()
        {
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
            OnDamagedEvent.RemoveListener(OnDamaged);
            OnHealedEvent.RemoveListener(OnHealed);
            Health.RemoveListener(OnHealthChanged);
        }
    }
}
