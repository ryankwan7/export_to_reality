using UnityEngine;

public class UnlockTrigger : MonoBehaviour
{
    [Header("Unlock on trigger")]
    public GameObject squareBlockButton; // drag the button directly here

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            squareBlockButton.SetActive(true);
            gameObject.SetActive(false); // disable the trigger zone
        }
    }
}