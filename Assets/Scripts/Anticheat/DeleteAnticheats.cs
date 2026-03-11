using UnityEngine;

public class DeleteAnticheats : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PathRecord.recordedSnapshots.Clear();
                GameObject enemy = GameObject.FindWithTag("Anticheat");
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
        }
}
