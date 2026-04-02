using UnityEngine;

public class PulleySystem2D : MonoBehaviour
{
    [Header("Platforms")]
    [SerializeField] private Transform platformA;
    [SerializeField] private Transform platformB;

    [Header("Sensors (triggers on top)")]
    [SerializeField] private PulleyWeightSensor2D sensorA;
    [SerializeField] private PulleyWeightSensor2D sensorB;

    [Header("Motion")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float deadzoneMassDifference = 0.05f;

    [Header("Limits")]
    [SerializeField] private float platformAMinY;
    [SerializeField] private float platformAMaxY;
    [SerializeField] private float platformBMinY;
    [SerializeField] private float platformBMaxY;

    [Header("Optional feel")]
    [SerializeField] private float maxMassDifferenceForFullSpeed = 5f;

    [SerializeField] private LayerMask obstacleLayer;

    [Header("Auto-Reset")]
    [SerializeField] private bool useAutoReset = true;
    [SerializeField] private float resetSpeed = 1.0f;
    private float originalPlatformAY; // We store where it started

    private void Reset()
    {
        speed = 2.5f;
        deadzoneMassDifference = 0.05f;
        maxMassDifferenceForFullSpeed = 5f;
    }

    private void Start()
    {
        // Record the starting position so it knows where "home" is
        if (platformA != null) originalPlatformAY = platformA.position.y;
    }

    private void FixedUpdate()
    {
        if (platformA == null || platformB == null || sensorA == null || sensorB == null) return;

        float massA = sensorA.TotalMass;
        float massB = sensorB.TotalMass;

        float diff = massA - massB; // positive means A is heavier
    // Balance in middle if not touched
    if (useAutoReset && Mathf.Abs(massA) < 0.01f && Mathf.Abs(massB) < 0.01f)
    {
        float currentAY = platformA.position.y;
        
        // If we aren't at the original spot, move toward it
        if (Mathf.Abs(currentAY - originalPlatformAY) > 0.01f)
        {
            // Determine if we need to go up or down to get home
            float direction = (originalPlatformAY > currentAY) ? 1f : -1f;
            float resetDelta = direction * resetSpeed * Time.fixedDeltaTime;
            
            // Move back toward home (A moves resetDelta, B moves opposite)
            MovePlatformPair(resetDelta, -resetDelta);
            return; // Exit so the mass logic doesn't override this
        }
    }

        if (Mathf.Abs(diff) < deadzoneMassDifference) return;

        float t = Mathf.Clamp(diff / Mathf.Max(0.001f, maxMassDifferenceForFullSpeed), -1f, 1f);

        float delta = t * speed * Time.fixedDeltaTime;

        // If A is heavier, A should move down, B should move up
        MovePlatformPair(-delta, +delta);
    }

    // private void MovePlatformPair(float deltaAY, float deltaBY)
    // {
    //     Vector3 a = platformA.position;
    //     Vector3 b = platformB.position;

    //     float targetAY = a.y + deltaAY;
    //     float targetBY = b.y + deltaBY;

    //     // Clamp to limits
    //     float clampedAY = Mathf.Clamp(targetAY, platformAMinY, platformAMaxY);
    //     float clampedBY = Mathf.Clamp(targetBY, platformBMinY, platformBMaxY);

    //     // Rope constraint feel
    //     // If one side hits a limit, the other side should also stop for that step.
    //     // We do that by applying only the “allowed” movement.
    //     float appliedAY = clampedAY - a.y;
    //     float appliedBY = clampedBY - b.y;

    //     float applied = 0f;

    //     // We want equal magnitude and opposite direction.
    //     // Pick the smaller magnitude movement that both sides can do this frame.
    //     float magA = Mathf.Abs(appliedAY);
    //     float magB = Mathf.Abs(appliedBY);
    //     float mag = Mathf.Min(magA, magB);

    //     if (mag <= 0f) return;

    //     applied = Mathf.Sign(appliedAY) * mag; // sign of A determines direction

    //     a.y += applied;
    //     b.y -= applied;

    //     platformA.position = a;
    //     platformB.position = b;
    // }

    private void MovePlatformPair(float deltaAY, float deltaBY)
    {
        Rigidbody2D rbA = platformA.GetComponent<Rigidbody2D>();
        Rigidbody2D rbB = platformB.GetComponent<Rigidbody2D>();

        float targetAY = platformA.position.y + deltaAY;
        float targetBY = platformB.position.y + deltaBY;

        float clampedAY = Mathf.Clamp(targetAY, platformAMinY, platformAMaxY);
        float clampedBY = Mathf.Clamp(targetBY, platformBMinY, platformBMaxY);

        float finalDeltaY = (clampedAY - platformA.position.y);

        // Check if running through ground layer
        if (IsPathBlocked(rbA, finalDeltaY) || IsPathBlocked(rbB, -finalDeltaY))
        {
            return; // Stop moving if either side is blocked!
        }

        rbA.MovePosition(new Vector2(platformA.position.x, platformA.position.y + finalDeltaY));
        rbB.MovePosition(new Vector2(platformB.position.x, platformB.position.y - finalDeltaY));
    }

    private bool IsPathBlocked(Rigidbody2D rb, float deltaY)
    {
        if (Mathf.Abs(deltaY) < 0.001f) return false;

        Vector2 direction = deltaY > 0 ? Vector2.up : Vector2.down;
        float distance = Mathf.Abs(deltaY);

        // Cast the platform's own shape into the world to see if it hits anything
        // We use a ContactFilter to ignore the weights sitting ON the platform
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(obstacleLayer); 
        filter.useLayerMask = true;


        RaycastHit2D[] results = new RaycastHit2D[1];
        int count = rb.Cast(direction, filter, results, distance+ 0.01f);

        return count > 0;
    }

}
