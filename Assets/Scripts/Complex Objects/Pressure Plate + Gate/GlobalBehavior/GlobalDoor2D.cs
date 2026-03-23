using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GlobalDoor2D : MonoBehaviour
{
    [Header("Channel Settings")]
    [Tooltip("Match this ID with the GlobalPressurePlate2D ID")]
    [SerializeField] private int channelID = 0;

    [Header("Movement Settings")]
    [Tooltip("How far and in what direction to move (e.g., 0,3 for Up; 3,0 for Right)")]
    [SerializeField] private Vector2 moveOffset = new Vector2(0, 3f);
    [SerializeField] private float speed = 2f;

    public Vector2 Velocity { get; private set; }

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 targetPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = rb.position;
        targetPos = startPos + moveOffset;

        // Standard moving platform settings
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void FixedUpdate()
    {
        bool isActive = GlobalPressurePlate2D.GetChannelState(channelID);
        Vector2 destination = isActive ? targetPos : startPos;
        
        Vector2 prevPos = rb.position; // Store old position
        Vector2 newPos = Vector2.MoveTowards(prevPos, destination, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // CALCULATE VELOCITY: (New - Old) / Time
        Velocity = (newPos - prevPos) / Time.fixedDeltaTime;
    }
}