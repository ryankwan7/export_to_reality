using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


[RequireComponent(typeof(Rigidbody2D))]
public class MoverController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpImpulse = 10f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);

    [SerializeField] private Animator animator;

    [Header("Respawn")]
    [SerializeField] private Vector2 respawnOffset = new Vector2(0.75f, 1.0f); // up and right

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpQueued;
    private Collider2D col;

    private Vector3 respawnPosition;
    public System.Action OnRespawnCallback;
    
    [Header("Blue Screen Visuals")]
    [SerializeField] private GameObject warningIcon;
    private bool isBlueScreened = false;
    public bool IsBlueScreened => isBlueScreened;

    private float blueScreenTimer = 0f;
    private float blueScreenDuration = 0f;
    private Vector3 originalPosBeforeBSOD;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        respawnPosition = transform.position;
    }

    public void SetRespawnFromGate(Transform gateTransform)
    {
        respawnPosition = gateTransform.position + (Vector3)respawnOffset;
    }

    public void ResetPlayer()
    {
        transform.position = respawnPosition;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        OnRespawnCallback?.Invoke();

        Debug.Log($"Player respawned at {respawnPosition}");
    }

    private bool IsGrounded()
    {
        Bounds b = col.bounds;

        Vector2 origin = new Vector2(b.center.x, b.min.y + 0.02f);
        Vector2 size = new Vector2(b.size.x * 0.9f, 2.0f);

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            size,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return hit.collider != null && hit.collider != col;
    }

    public void OnMove(InputValue value){
        if (isBlueScreened) return;
        moveInput = value.Get<Vector2>();
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            animator.SetBool("isRunning",true);
        }
        else
        {
            animator.SetBool("isRunning",false);
        }
    }

    public void OnRespawn(InputValue value)
    {
        if (isBlueScreened) return;
        if (value.isPressed)
            ResetPlayer();
    }


    public void OnJump(InputValue value)
    {
        if (isBlueScreened) return;
        if (value.isPressed) jumpQueued = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            ResetPlayer();
        }

        // if(other.CompareTag("Door"))
        // {
        //     UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelector");   
        // }

        /*
        if(other.CompareTag("DialogueTrigger"))
        {
            DialogueTrigger trigger = other.GetComponent<DialogueTrigger>();
            if (trigger != null)
                trigger.PrintDialogueToConsole();
        }
        */
    }

    private void Update()
    {
        if (isBlueScreened)
        {
            HandleBlueScreenLogic();
        }
    }

    public void StartBlueScreen(float duration)
    {
        if (isBlueScreened) return;

        isBlueScreened = true;
        blueScreenDuration = duration;
        blueScreenTimer = 0f;
        originalPosBeforeBSOD = transform.position;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        moveInput = Vector2.zero;

        if (warningIcon != null) warningIcon.SetActive(true);
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = Color.blue;
    }

    private void HandleBlueScreenLogic()
    {
        blueScreenTimer += Time.deltaTime;

        if (blueScreenTimer < blueScreenDuration)
        {
            float shakeIntensity = 0.15f;
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0
            );
            transform.position = originalPosBeforeBSOD + randomOffset;
        }
        else
        {
            EndBlueScreen();
        }
    }

    private void EndBlueScreen()
    {
        if (warningIcon != null) warningIcon.SetActive(false);
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = Color.white;

        rb.simulated = true;
        ResetPlayer();
        animator.SetBool("isRunning",false);
        isBlueScreened = false;
    }

    private void FixedUpdate()
    {
        if (isBlueScreened) return; // Stop all movement logic if blue screened

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        float direction = moveInput.x < 0 ? -1 : 1;
        if(!animator.GetBool("isJumping"))
        {
            transform.localScale = new Vector3(direction*5, 5, 5);
        }
        else // For jump sprites that face the wrong way
        {
            transform.localScale = new Vector3(-direction*5, 5, 5);
        }

        if (animator.GetBool("isJumping") && IsGrounded())
        {
            animator.SetBool("isJumping",false);
        }

        if (jumpQueued)
        {
            jumpQueued = false;
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
                animator.SetBool("isJumping",true);
            }
        }
    }
}