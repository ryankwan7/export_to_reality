using UnityEngine;

public class UIActive : MonoBehaviour
{
    [Header("Settings")]
    public GameObject targetUI; // The UI element to watch
    public bool matchTargetState = false; // True = same as target, False = opposite of target

    private bool _lastState;

    void Start()
    {
        if (targetUI != null)
        {
            _lastState = targetUI.activeInHierarchy;
            ToggleChildren(_lastState);
        }
    }

    void Update()
    {
        if (targetUI == null) return;

        bool currentState = targetUI.activeInHierarchy;

        if (currentState != _lastState)
        {
            _lastState = currentState;
            ToggleChildren(matchTargetState ? currentState : !currentState);
        }
    }

    void ToggleChildren(bool state)
    {
        // Loops through every immediate child of this GameObject
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(state);
        }
    }
}