using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Respawn")]
    [SerializeField] private Vector2 respawnOffset = new Vector2(0.75f, 1.0f); // up and right

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpQueued;
    private Collider2D col;

    private Vector3 respawnPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        respawnPosition = transform.position; // default to start
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

        Debug.Log($"Player respawned at {respawnPosition}");
    }

    private bool IsGrounded()
    {
        Bounds b = col.bounds;

        Vector2 origin = new Vector2(b.center.x, b.min.y + 0.02f);
        Vector2 size = new Vector2(b.size.x * 0.9f, 0.02f);

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

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    public void OnRespawn(InputValue value)
    {
        if (value.isPressed)
            ResetPlayer();
    }


    public void OnJump(InputValue value)
    {
        if (value.isPressed) jumpQueued = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            ResetPlayer();
        }

        if(other.CompareTag("Door"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelector");   
        }

        if(other.CompareTag("DialogueTrigger"))
        {
            DialogueTrigger trigger = other.GetComponent<DialogueTrigger>();
            if (trigger != null)
                trigger.PrintDialogueToConsole();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (jumpQueued)
        {
            jumpQueued = false;

            if (IsGrounded())
                rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        }
    }
}