using Generated.Variables;
using UnityEngine;

public class Regeneration : MonoBehaviour
{
    [SerializeField] private FloatReferenceClamped Health;
    [SerializeField] private FloatReference PassiveRegeneration;

    // Start is called before the first frame update
    void Start()
    {
        Health.EnableCaching();
    }

    void OnDestroy()
    {
        Health.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        Health.Value = Health.Value + PassiveRegeneration.Value * Time.deltaTime;
    }
}
