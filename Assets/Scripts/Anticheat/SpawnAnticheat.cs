using UnityEngine;

public class SpawnAnticheat : MonoBehaviour
{
    public GameObject anticheatPrefab;
    public Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameObject.FindWithTag("Anticheat")==null) 
            {
                PathRecord.recordedSnapshots.Clear();
                Instantiate(anticheatPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}
