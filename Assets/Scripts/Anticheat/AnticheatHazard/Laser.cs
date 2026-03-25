using UnityEngine;

public class Laser : MonoBehaviour
{
private enum ExecutionState { Idle, Shaking }
    [SerializeField] private ExecutionState currentState = ExecutionState.Idle;

    [Header("Effect Settings")]
    public float shakeDuration = 2.0f;
    public Color bsodColor = new Color(0, 0, 1, 1);

    private GameObject currentVictim;
    private Vector3 originalPos;
    private SpriteRenderer victimSR;
    private Transform victimIcon;
    private float timer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState != ExecutionState.Idle) return;
        StunAnticheat stunScript = other.GetComponent<StunAnticheat>();
        PursuitPlayer pursuit = other.GetComponent<PursuitPlayer>();

        if (stunScript != null && pursuit != null && !pursuit.enabled)
        {
            StartExecution(other.gameObject);
        }
        else if (other.TryGetComponent<MoverController2D>(out MoverController2D player))
        {
            if (!player.IsBlueScreened && !player.isRebooting)
            {
                player.StartBlueScreen(2.0f);
            }
        }
    }

    void Update()
    {
        if (currentState == ExecutionState.Shaking)
        {
            HandleExecutionLogic();
        }
    }

    private void StartExecution(GameObject victim)
    {
        currentVictim = victim;
        currentState = ExecutionState.Shaking;
        timer = 0;

        // Stop the victim's movement
        if (currentVictim.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; // Freeze them in place
        }

        if (currentVictim.TryGetComponent<StunAnticheat>(out var stun)) { stun.ForceCancelStun(); }
        
        // Find visuals
        victimIcon = currentVictim.transform.Find("WarningIcon");
        if (victimIcon != null) victimIcon.gameObject.SetActive(true);

        victimSR = currentVictim.GetComponentInChildren<SpriteRenderer>();
        if (victimSR != null) victimSR.color = bsodColor;

        originalPos = currentVictim.transform.position;
    }

    private void HandleExecutionLogic()
    {
        if (currentVictim == null)
        {
            ResetTrap();
            return;
        }

        timer += Time.deltaTime;

        float shakeAmount = 0.1f;
        currentVictim.transform.position = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;

        if (timer >= shakeDuration)
        {
            Destroy(currentVictim);
            ResetTrap();
        }
    }

    private void ResetTrap()
    {
        currentState = ExecutionState.Idle;
        currentVictim = null;
        timer = 0;
    }
}