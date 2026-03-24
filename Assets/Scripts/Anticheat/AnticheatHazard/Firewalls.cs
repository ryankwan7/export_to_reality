using UnityEngine;

public class Firewalls : MonoBehaviour
{
    void Update()
    {
        StunAnticheat anticheat = Object.FindFirstObjectByType<StunAnticheat>();
        bool shouldBeVisible = anticheat != null && anticheat.enabled;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf != shouldBeVisible)
            {
                child.gameObject.SetActive(shouldBeVisible);
            }
        }
    }
}
