using UnityEngine;

public class HardwareWireVertical : MonoBehaviour
{
    public Transform platformAnchor; // Point on the platform
    public Transform capacitorPivot; // Center of the capacitor
    public float wireWidth = 0.5f;   // Visual thickness of the wire

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // Ensure the pivot is at the TOP center of the sprite 
        // so it "grows" downwards.
    }

    void LateUpdate()
    {
        if (platformAnchor == null || capacitorPivot == null) return;

        transform.position = capacitorPivot.position;

        float distance = capacitorPivot.position.y - platformAnchor.position.y;

        sr.size = new Vector2(wireWidth, Mathf.Abs(distance));
    }
}