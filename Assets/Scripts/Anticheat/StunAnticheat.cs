using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StunAnticheat : MonoBehaviour
{
    private PursuitPlayer pursuitPlayer;
    private bool isStunned = false;
    
    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private float bounceForce = 25f;
    [SerializeField] private Animator cloneAnimator;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem dizzyParticles;
    [SerializeField] private GameObject rewindIcon;

    private Rigidbody2D rb;
    private float originalGravity;
    private List<Vector3> bouncePath = new List<Vector3>();

    void Start()
    {
        pursuitPlayer = GetComponent<PursuitPlayer>();
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Placeable"))
        {
            PlacementManager2D manager = Object.FindFirstObjectByType<PlacementManager2D>();
            if (manager != null)
            {
                manager.SendMessage("RemovePlatform", collision.gameObject);
            }
            if (!isStunned)
            {
                StartCoroutine(HandleStun(collision));
            }
        }
    }

    IEnumerator HandleStun(Collider2D collision)
    {
        cloneAnimator.SetBool("isStunned", true);
        if (dizzyParticles != null) dizzyParticles.Play();
        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        isStunned = true;
        pursuitPlayer.enabled = false; 
        bouncePath.Clear(); // Reset the path for the new bounce

        rb.gravityScale = 10;
        rb.linearVelocity = Vector2.zero; 
        Vector2 hitDirection = (transform.position - collision.transform.position).normalized;
        hitDirection.y = 1f; 

        rb.AddForce(hitDirection.normalized * bounceForce, ForceMode2D.Impulse);

        float timer = 0;
        while (timer < stunDuration)
        {
            bouncePath.Add(transform.position);
            timer += Time.deltaTime;
            yield return null;
        }

        if (dizzyParticles != null) dizzyParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        if (rewindIcon != null) rewindIcon.SetActive(true);

        for (int i = bouncePath.Count - 1; i >= 0; i-=2)
        {
            transform.position = bouncePath[i];
            if (rewindIcon != null)
            {
                float parentDirection = Mathf.Sign(transform.localScale.x);
                rewindIcon.transform.localScale = new Vector3(parentDirection*0.25f, 0.25f, 1);
            }

            yield return null; 
        }
        if (rewindIcon != null) rewindIcon.SetActive(false);

        
        rb.gravityScale = originalGravity;
        pursuitPlayer.enabled = true;
        isStunned = false;
        cloneAnimator.SetBool("isStunned", false);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
