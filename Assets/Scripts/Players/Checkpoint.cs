using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Camera Zone")]
    [SerializeField] private FixedZoneCameraController2D cameraController;
    [SerializeField] private int zoneIndex = 0;

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        MoverController2D mover = other.GetComponent<MoverController2D>();
        if (mover != null)
        {
            hasTriggered = true;
            mover.SetRespawnFromGate(transform);
            mover.OnRespawnCallback = () => cameraController?.SetZone(zoneIndex);
        }
    }
}