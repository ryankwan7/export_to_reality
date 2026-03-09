using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        MoverController2D mover = other.GetComponent<MoverController2D>();
        if (mover != null)
        {
            hasTriggered = true;
            mover.SetRespawnFromGate(transform);
        }
    }
}