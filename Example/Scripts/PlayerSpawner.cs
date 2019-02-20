using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;

    public void SpawnPlayer()
    {
        Instantiate(PlayerPrefab);
    }
}