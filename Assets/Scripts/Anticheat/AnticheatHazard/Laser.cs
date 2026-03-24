using UnityEngine;

public class Laser : MonoBehaviour
{
    private enum ExecutionState { Idle, Shaking }
    [SerializeField] private ExecutionState currentState = ExecutionState.Idle;

    [Header("Laser Setup")]
    public Transform firePoint;
    public LineRenderer lineRenderer;
    public float maxDistance = 15f;
    public LayerMask hitLayers;

    [Header("Effect Settings")]
    public float shakeDuration = 2.0f;
    public Color bsodColor = new Color(0, 0, 1, 1);

    private GameObject currentVictim;
    private Vector3 originalPos;
    private SpriteRenderer victimSR;
    private Transform victimIcon;
    private float timer;

    void Update()
    {
        if (currentState == ExecutionState.Idle)
        {
            HandleLaserRaycast();
        }
        else
        {
            HandleExecutionLogic();
        }
    }

    private void HandleLaserRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, maxDistance, hitLayers);

        if (hit.collider != null)
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hit.point);

            StunAnticheat stunScript = hit.collider.GetComponent<StunAnticheat>();
            PursuitPlayer pursuit = hit.collider.GetComponent<PursuitPlayer>();

            if (stunScript != null && pursuit != null && !pursuit.enabled)
            {
                StartExecution(hit.collider.gameObject);
            }
            else if (hit.collider.TryGetComponent<MoverController2D>(out MoverController2D player))
            {
                if (!player.IsBlueScreened && !player.isRebooting)
                {
                    player.StartBlueScreen(2.0f);                
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.right * maxDistance);
        }
    }

    private void StartExecution(GameObject victim)
    {
        currentVictim = victim;
        currentState = ExecutionState.Shaking;
        timer = 0;

        if (currentVictim.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (currentVictim.TryGetComponent<StunAnticheat>(out var stun)) { stun.ForceCancelStun(); }
        if (currentVictim.TryGetComponent<PursuitPlayer>(out var move)) { move.enabled = false; }

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
            ResetLaser();
            return;
        }

        timer += Time.deltaTime;

        float shakeAmount = 0.15f;
        currentVictim.transform.position = originalPos + new Vector3(
            Random.Range(-shakeAmount, shakeAmount),
            Random.Range(-shakeAmount, shakeAmount), 0);

        if (timer >= shakeDuration)
        {
            Destroy(currentVictim);
            ResetLaser();
        }
    }

    private void ResetLaser()
    {
        currentState = ExecutionState.Idle;
        currentVictim = null;
        timer = 0;
    }
}