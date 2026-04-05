using UnityEngine;

public class WireHorizontal : MonoBehaviour
{
    public Transform leftCapacitor;  // Pivot A
    public Transform rightCapacitor; // Pivot B
    public float wireThickness = 0.5f; // Match your vertical wire width

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (leftCapacitor == null || rightCapacitor == null) return;

        float distance = Mathf.Abs(rightCapacitor.position.x - leftCapacitor.position.x);

        sr.size = new Vector2(wireThickness, distance);
        
        transform.position = new Vector3(leftCapacitor.position.x, leftCapacitor.position.y, transform.position.z);
    }
}