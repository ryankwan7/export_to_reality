using UnityEngine;

public class PlaceablePlatform : MonoBehaviour
{
    private PlacementManager2D manager;

    private void Awake()
    {
        manager = FindFirstObjectByType<PlacementManager2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("DeathZone")) return;

        if (manager != null)
            manager.RemovePlatform(gameObject);
        else
            Destroy(gameObject);
    }
}
