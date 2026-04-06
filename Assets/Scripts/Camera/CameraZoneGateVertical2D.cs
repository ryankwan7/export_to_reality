using System.Collections.Generic;
using UnityEngine;

public class CameraZoneGateVertical2D : MonoBehaviour
{
    [SerializeField] private FixedZoneCameraController2D cameraController;
    [SerializeField] private int topZoneIndex    = 0;
    [SerializeField] private int bottomZoneIndex = 1;
    [SerializeField] private string playerTag    = "Player";
    [SerializeField] public bool reverseDirection = false;

    private readonly Dictionary<Collider2D, bool> enteredFromTop = new Dictionary<Collider2D, bool>();

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

        float gateY = transform.position.y;
        float y = other.bounds.center.y;

        enteredFromTop[other] = (y > gateY);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (cameraController == null) return;

        float gateY = transform.position.y;
        float y = other.bounds.center.y;

        bool wasFromTop = false;
        if (enteredFromTop.TryGetValue(other, out bool val))
            wasFromTop = val;

        int zoneTop    = reverseDirection ? bottomZoneIndex : topZoneIndex;
        int zoneBottom = reverseDirection ? topZoneIndex    : bottomZoneIndex;

        if (wasFromTop && y < gateY)          // passed top → bottom
            cameraController.SetZone(zoneBottom);
        else if (!wasFromTop && y > gateY)    // passed bottom → top
            cameraController.SetZone(zoneTop);

        enteredFromTop.Remove(other);
    }
}
