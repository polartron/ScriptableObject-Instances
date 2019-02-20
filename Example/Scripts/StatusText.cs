using UnityEngine;
using UnityEngine.UI;

public class StatusText : MonoBehaviour
{
    [SerializeField] private PlayerDataReference playerData;
    [SerializeField] private FloatReference Health;
    [SerializeField] private FloatReference MaxHealth;

    public Image HealthImage;
    public Image Portrait;

    private void Start()
    {
        Health.AddListener(OnHealthChanged);

        SetBar(Health, MaxHealth);

        Portrait.color = playerData.Value.PlayerColor;
    }

    private void OnDestroy()
    {
        Health.RemoveListener(OnHealthChanged);
    }

    private void OnHealthChanged(float value)
    {
        SetBar(value, MaxHealth);
    }

    private void SetBar(float health, float maxHealth)
    {
        HealthImage.fillAmount = Mathf.InverseLerp(0f, maxHealth, health);
    }
}