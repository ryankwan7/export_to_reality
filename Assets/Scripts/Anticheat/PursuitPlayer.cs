using UnityEngine;

public class PursuitPlayer : MonoBehaviour
{
    public int frameDelay = 200; // Delay between mover and anticheat movement
    private int currentFrame = 0;
    public int rubberBand = 250;
    [SerializeField] private Animator cloneAnimator;

    [Header("After-Image Settings")]
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float distanceBetweenImages = 1.0F;
    private Vector3 lastImagePos;

    void FixedUpdate()
    {
        currentFrame++;
        int snapshotsInQueue = PathRecord.recordedSnapshots.Count;
        int processCount = snapshotsInQueue switch
        {
            var s when s >=rubberBand*3=>4,
            var s when s >=rubberBand*2=>3,
            var s when s >=rubberBand*1=>2,
            _=>1
        };
        for (int i = 0; i < processCount; i++)
        {
            if (currentFrame > frameDelay && PathRecord.recordedSnapshots.Count > 0)
            {
                PlayerSnapshot snapshot = PathRecord.recordedSnapshots.Dequeue();
                if (i==0){
                    transform.position = snapshot.position;
                    cloneAnimator.SetBool("isRunning", snapshot.isRunning);
                    cloneAnimator.SetBool("isJumping", snapshot.isJumping);
                    transform.localScale = new Vector3(snapshot.scaleX, 5, 5);
                }

                if (Vector3.Distance(transform.position, lastImagePos) > distanceBetweenImages)
                {
                    SpawnGhost();
                    lastImagePos = transform.position;
                }
            }
        }
    }

    void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
        SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
        SpriteRenderer cloneSR = GetComponent<SpriteRenderer>();

        // Copy the current look of the clone
        ghostSR.sprite = cloneSR.sprite;
        ghost.transform.localScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MoverController2D playerScript = collision.gameObject.GetComponent<MoverController2D>();
        if (playerScript != null)
        {
            Debug.Log("Caught by Anticheat");
            playerScript.ResetPlayer();
            PathRecord.recordedSnapshots.Clear();
            Destroy(gameObject);
        }
    }
}
