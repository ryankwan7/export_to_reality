using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class PlacementManager2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Placeable Prefabs (buttons select an index)")]
    [SerializeField] private GameObject[] platformPrefabs;

    [Header("Limits")]
    [SerializeField] private int maxPlatformsInScene = 5;

    [Tooltip("Max allowed per prefab index. Must match platformPrefabs length. Example: [3,2,1].")]
    [SerializeField] private int[] maxPerType;

    [Header("Placement Settings")]
    [SerializeField] private bool snapToGrid = true;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float rotateStepDegrees = 90f;
    [SerializeField] private float ghostRotateSpeedDegPerSec = 540f;
    [SerializeField] private float ghostAlpha = 0.5f;

    [Header("Removal Settings")]
    [Tooltip("What layers count as removable platforms. Put your placed platforms on this layer.")]
    [SerializeField] private LayerMask removableMask;
    [SerializeField] private float clickRadius = 0.1f;
    [Header("UI Counters (one per prefab index)")]
    [SerializeField] private TMP_Text[] typeCounterTexts;

    [SerializeField] private Color counterNormalColor = Color.white;
    [SerializeField] private Color counterMaxColor = Color.red;


    private readonly List<GameObject> placedPlatforms = new();

    // Track placed count per type and map instance -> type index
    private int[] currentPerType;
    private readonly Dictionary<GameObject, int> instanceToTypeIndex = new();

    private bool isPlacing;
    private int selectedIndex = 0;

    private GameObject ghost;
    private float currentRotationZ;
    private float targetRotationZ;

    void Awake()
    {
        if (cam == null) cam = Camera.main;

        int typeCount = platformPrefabs != null ? platformPrefabs.Length : 0;
        currentPerType = new int[typeCount];

        if (typeCount > 0)
        {
            if (maxPerType == null || maxPerType.Length != typeCount)
            {
                maxPerType = new int[typeCount];
                for (int i = 0; i < typeCount; i++)
                    maxPerType[i] = maxPlatformsInScene;
            }
        }

        RefreshCountersUI();

    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (isPlacing && ghost != null)
        {
            UpdateGhostFollowAndRotation();

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (!CanPlaceSelectedType())
                    return;

                Vector3 pos = ghost.transform.position;
                PlaceRealPlatform(pos, targetRotationZ);
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                StopPlacing();
            }
        }
        else
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                TryRemovePlatformUnderMouse();
            }
        }
    }

    // UI Buttons call this with a different index
    public void StartPlacing(int prefabIndex)
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0) return;
        if (prefabIndex < 0 || prefabIndex >= platformPrefabs.Length) return;

        selectedIndex = prefabIndex;
        isPlacing = true;

        currentRotationZ = 0f;
        targetRotationZ = 0f;

        if (ghost != null) Destroy(ghost);

        ghost = Instantiate(platformPrefabs[selectedIndex]);
        ghost.name = "GhostPlatform";

        var col = ghost.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        var rb = ghost.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        var sr = ghost.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = ghostAlpha;
            sr.color = c;
        }
    }

    public void StopPlacing()
    {
        isPlacing = false;
        if (ghost != null) Destroy(ghost);
        ghost = null;
    }

    private void UpdateGhostFollowAndRotation()
    {
        Vector2 mouseScreen2D = Mouse.current.position.ReadValue();
        Vector3 mouseScreen = new(mouseScreen2D.x, mouseScreen2D.y, -cam.transform.position.z);
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        if (snapToGrid)
        {
            mouseWorld.x = Mathf.Round(mouseWorld.x / gridSize) * gridSize;
            mouseWorld.y = Mathf.Round(mouseWorld.y / gridSize) * gridSize;
        }

        ghost.transform.position = mouseWorld;

        float scrollY = Mouse.current.scroll.ReadValue().y;
        if (scrollY > 0f) targetRotationZ += rotateStepDegrees;
        else if (scrollY < 0f) targetRotationZ -= rotateStepDegrees;

        currentRotationZ = Mathf.MoveTowardsAngle(
            currentRotationZ,
            targetRotationZ,
            ghostRotateSpeedDegPerSec * Time.deltaTime
        );

        ghost.transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);

        // Optional visual: tint ghost if you cannot place due to limits
        var sr = ghost.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = ghostAlpha;
            sr.color = c;
        }
    }

    private bool CanPlaceSelectedType()
    {
        if (placedPlatforms.Count >= maxPlatformsInScene)
            return false;

        if (currentPerType == null || selectedIndex < 0 || selectedIndex >= currentPerType.Length)
            return false;

        if (maxPerType != null && maxPerType.Length == currentPerType.Length)
        {
            if (currentPerType[selectedIndex] >= maxPerType[selectedIndex])
                return false;
        }

        return true;
    }

    private void PlaceRealPlatform(Vector3 pos, float rotZ)
    {
        GameObject prefab = platformPrefabs[selectedIndex];
        GameObject placed = Instantiate(prefab, pos, Quaternion.Euler(0f, 0f, rotZ));
        placed.name = $"Platform_{selectedIndex}";

        if (placed.GetComponent<PlaceablePlatform>() == null)
            placed.AddComponent<PlaceablePlatform>();

        var rb = placed.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            // rb.gravityScale = 1f;
        }

        var col = placed.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        placedPlatforms.Add(placed);

        if (currentPerType != null && selectedIndex >= 0 && selectedIndex < currentPerType.Length)
            currentPerType[selectedIndex]++;

        instanceToTypeIndex[placed] = selectedIndex;

        RefreshCountersUI();
    }

    private void TryRemovePlatformUnderMouse()
    {
        Vector2 mouseScreen2D = Mouse.current.position.ReadValue();
        Vector3 mouseScreen = new(mouseScreen2D.x, mouseScreen2D.y, -cam.transform.position.z);
        Vector2 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        Collider2D hit = Physics2D.OverlapCircle(mouseWorld, clickRadius, removableMask);
        if (hit == null) return;

        PlaceablePlatform marker = hit.GetComponentInParent<PlaceablePlatform>();
        if (marker == null) return;

        RemovePlatform(marker.gameObject);
    }

    private void RemovePlatform(GameObject platform)
    {
        if (platform == null) return;

        if (instanceToTypeIndex.TryGetValue(platform, out int typeIndex))
        {
            if (currentPerType != null && typeIndex >= 0 && typeIndex < currentPerType.Length)
                currentPerType[typeIndex] = Mathf.Max(0, currentPerType[typeIndex] - 1);

            instanceToTypeIndex.Remove(platform);
        }

        placedPlatforms.Remove(platform);
        Destroy(platform);

        RefreshCountersUI();
    }

    public void RemoveMode()
    {
        StopPlacing();
    }

    public void ClearAllPlaced()
    {
        for (int i = placedPlatforms.Count - 1; i >= 0; i--)
        {
            GameObject p = placedPlatforms[i];
            if (p == null) continue;

            if (instanceToTypeIndex.TryGetValue(p, out int typeIndex))
            {
                if (currentPerType != null && typeIndex >= 0 && typeIndex < currentPerType.Length)
                    currentPerType[typeIndex] = Mathf.Max(0, currentPerType[typeIndex] - 1);

                instanceToTypeIndex.Remove(p);
            }

            Destroy(p);
        }

        placedPlatforms.Clear();

        RefreshCountersUI();
    }

    // Optional helpers for UI
    public int GetTotalPlacedCount()
    {
        return placedPlatforms.Count;
    }

    public int GetPlacedCountForType(int typeIndex)
    {
        if (currentPerType == null) return 0;
        if (typeIndex < 0 || typeIndex >= currentPerType.Length) return 0;
        return currentPerType[typeIndex];
    }

    public int GetRemainingForType(int typeIndex)
    {
        if (maxPerType == null || currentPerType == null) return 0;
        if (typeIndex < 0 || typeIndex >= currentPerType.Length) return 0;
        return Mathf.Max(0, maxPerType[typeIndex] - currentPerType[typeIndex]);
    }

    void OnDrawGizmosSelected()
    {
        if (cam == null) return;
        Gizmos.DrawWireSphere(Vector3.zero, 0.01f);
    }

    private void RefreshCountersUI()
    {
        if (typeCounterTexts == null || currentPerType == null || maxPerType == null) return;

        int count = Mathf.Min(typeCounterTexts.Length, currentPerType.Length);

        for (int i = 0; i < count; i++)
        {
            TMP_Text t = typeCounterTexts[i];
            if (t == null) continue;

            int placed = currentPerType[i];
            int max = (i < maxPerType.Length) ? maxPerType[i] : maxPlatformsInScene;

            t.text = $"{placed}/{max}";
            t.color = placed >= max ? counterMaxColor : counterNormalColor;
        }
    }

    public void SetMaxForType(int typeIndex, int newMax)
    {
        if (maxPerType == null || typeIndex < 0 || typeIndex >= maxPerType.Length) return;
        maxPerType[typeIndex] = newMax;
        RefreshCountersUI();
    }

}