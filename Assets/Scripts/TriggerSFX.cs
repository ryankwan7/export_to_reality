using UnityEngine;

public class TriggerSFX : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool playEveryTime = false;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private bool hasPlayed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!playEveryTime && hasPlayed) return;

        hasPlayed = true;
        AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }
}