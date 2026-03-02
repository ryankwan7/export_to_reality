using UnityEngine;

public class MaxUpgradeTrigger : MonoBehaviour
{
    [Header("References")]
    public PlacementManager2D placementManager;

    [Header("Upgrade Settings")]
    [Tooltip("Which platform type index to upgrade (matches platformPrefabs array in PlacementManager2D)")]
    public int platformTypeIndex = 0;

    [Tooltip("The new max value to set for that platform type")]
    public int newMaxValue = 3;

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            placementManager.SetMaxForType(platformTypeIndex, newMaxValue);
            gameObject.SetActive(false);
        }
    }
}