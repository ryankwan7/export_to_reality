using UnityEngine;

// Attach to a trigger collider tagged "DeathZone".
// Resets the player only when this death zone is outside the camera's view.
// If the death zone is visible on screen, the reset is suppressed.
public class OffscreenDeathZone : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (IsVisibleToCamera()) return;

        MoverController2D mover = other.GetComponent<MoverController2D>();
        if (mover != null) mover.ResetPlayer();
    }

    private bool IsVisibleToCamera()
    {
        if (cam == null) return false;

        Bounds b = GetComponent<Collider2D>().bounds;
        Vector3[] corners = {
            new Vector3(b.min.x, b.min.y, b.center.z),
            new Vector3(b.min.x, b.max.y, b.center.z),
            new Vector3(b.max.x, b.min.y, b.center.z),
            new Vector3(b.max.x, b.max.y, b.center.z)
        };

        foreach (Vector3 corner in corners)
        {
            Vector3 vp = cam.WorldToViewportPoint(corner);
            if (vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f && vp.z > 0f)
                return true;
        }
        return false;
    }
}
