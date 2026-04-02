using UnityEngine;
using UnityEngine.InputSystem;

// Attach to any UI panel. Requires a CanvasGroup on the same GameObject.
// Fades the panel when the mover walks behind it or the mouse hovers over it.
// Also disables raycasts while faded so the Maker can click through to place platforms.
[RequireComponent(typeof(CanvasGroup))]
public class PanelFadeOnOverlap : MonoBehaviour
{
    [SerializeField] private Collider2D moverCollider;
    [SerializeField] private Camera cam;
    [SerializeField] private float fadedAlpha = 0.15f;
    [SerializeField] private float fadeSpeed = 8f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        bool shouldFade = IsMoverBehind() || IsMouseOver();
        float targetAlpha = shouldFade ? fadedAlpha : 1f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        canvasGroup.blocksRaycasts = !shouldFade;
    }

    private bool IsMoverBehind()
    {
        if (moverCollider == null || cam == null) return false;

        // Check all 4 corners of the collider's world-space bounding box
        Bounds b = moverCollider.bounds;
        Vector3[] worldCorners = {
            new Vector3(b.min.x, b.min.y, b.center.z),
            new Vector3(b.min.x, b.max.y, b.center.z),
            new Vector3(b.max.x, b.min.y, b.center.z),
            new Vector3(b.max.x, b.max.y, b.center.z)
        };

        foreach (Vector3 corner in worldCorners)
        {
            Vector2 screenPos = cam.WorldToScreenPoint(corner);
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPos, null))
                return true;
        }
        return false;
    }

    private bool IsMouseOver()
    {
        if (Mouse.current == null) return false;
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Mouse.current.position.ReadValue(), null);
    }
}
