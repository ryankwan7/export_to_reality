using UnityEngine;

public class SpawnAnticheat : MonoBehaviour
{
    public GameObject anticheatPrefab;
    public Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if it's the player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameObject.FindWithTag("Anticheat")==null) 
            {
                Instantiate(anticheatPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}
