using System.Collections.Generic;
using UnityEngine;

public class CameraZoneGate2D : MonoBehaviour
{
    [SerializeField] private FixedZoneCameraController2D cameraController;
    [SerializeField] private int leftZoneIndex = 0;
    [SerializeField] private int rightZoneIndex = 1;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] public bool reverseDirection = false;

    private readonly Dictionary<Collider2D, bool> enteredFromLeft = new Dictionary<Collider2D, bool>();

    private void Reset()
    {
        var bc = GetComponent<BoxCollider2D>();
        if (bc != null) bc.isTrigger = true;
    }

    private void Awake()
    {
        if (cameraController == null)
            cameraController = FindFirstObjectByType<FixedZoneCameraController2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        float gateX = transform.position.x;
        float x = other.bounds.center.x;

        enteredFromLeft[other] = (x < gateX);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (cameraController == null) return;

        float gateX = transform.position.x;
        float x = other.bounds.center.x;

        bool wasFromLeft = false;
        if (enteredFromLeft.TryGetValue(other, out bool val))
            wasFromLeft = val;

        int zoneLeft  = reverseDirection ? rightZoneIndex : leftZoneIndex;
        int zoneRight = reverseDirection ? leftZoneIndex  : rightZoneIndex;

        if (wasFromLeft && x > gateX)
            cameraController.SetZone(zoneRight);
        else if (!wasFromLeft && x < gateX)
            cameraController.SetZone(zoneLeft);

        enteredFromLeft.Remove(other);
    }
}