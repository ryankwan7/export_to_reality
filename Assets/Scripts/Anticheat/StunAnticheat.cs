using UnityEngine;
using System.Collections.Generic;

public class StunAnticheat : MonoBehaviour
{
    private enum StunState { Normal, Bouncing, Rewinding }
    [SerializeField] private StunState currentState = StunState.Normal;

    private PursuitPlayer pursuitPlayer;
    
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
    
    private float stunTimer;
    private int rewindIndex;

    void Awake()
    {
        pursuitPlayer = GetComponent<PursuitPlayer>();
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        if (currentState == StunState.Bouncing)
        {
            HandleBouncingLogic();
        }
        else if (currentState == StunState.Rewinding)
        {
            HandleRewindingLogic();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Placeable"))
        {
            PlacementManager2D manager = Object.FindFirstObjectByType<PlacementManager2D>();
            if (manager != null) manager.SendMessage("RemovePlatform", collision.gameObject);
            
            if (currentState == StunState.Normal)
            {
                StartStun(collision);
            }
        }
    }

    private void StartStun(Collider2D collision)
    {
        currentState = StunState.Bouncing;
        stunTimer = 0;
        bouncePath.Clear();

        pursuitPlayer.enabled = false;
        cloneAnimator.SetBool("isStunned", true);
        if (dizzyParticles != null) dizzyParticles.Play();

        rb.gravityScale = 10;
        rb.linearVelocity = Vector2.zero;
        Vector2 hitDirection = (transform.position - collision.transform.position).normalized;
        hitDirection.y = 1f;
        rb.AddForce(hitDirection.normalized * bounceForce, ForceMode2D.Impulse);
        float faceDir = Mathf.Sign(hitDirection.x);
        transform.localScale = new Vector3(faceDir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void HandleBouncingLogic()
    {
        stunTimer += Time.deltaTime;
        bouncePath.Add(transform.position);

        if (stunTimer >= stunDuration)
        {
            currentState = StunState.Rewinding;
            rewindIndex = bouncePath.Count - 1;

            if (dizzyParticles != null) dizzyParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (rewindIcon != null) rewindIcon.SetActive(true);
            
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleRewindingLogic()
    {
        if (rewindIndex >= 0)
        {
            transform.position = bouncePath[rewindIndex];
            
            if (rewindIcon != null)
            {
                float parentDirection = Mathf.Sign(transform.localScale.x);
                rewindIcon.transform.localScale = new Vector3(parentDirection * 0.25f, 0.25f, 1);
            }
            rewindIndex -= 2;
        }
        else
        {
            EndStun();
        }
    }

    private void EndStun()
    {
        currentState = StunState.Normal;
        
        if (rewindIcon != null) rewindIcon.SetActive(false);
        rb.gravityScale = originalGravity;
        pursuitPlayer.enabled = true;
        cloneAnimator.SetBool("isStunned", false);
    }

    public void ForceCancelStun()
    {
        if (rewindIcon != null) rewindIcon.SetActive(false);
        if (dizzyParticles != null) dizzyParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        currentState = StunState.Normal;
        rb.gravityScale = originalGravity;
        this.enabled = false;
    }
}