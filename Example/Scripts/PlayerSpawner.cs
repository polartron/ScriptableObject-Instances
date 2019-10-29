using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;

    public void SpawnPlayer()
    {
        for (int i = 0; i < 1; i++)
        {
            Instantiate(PlayerPrefab);
        }
    }
}