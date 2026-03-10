using UnityEngine;
using System.Collections.Generic;

public struct PlayerSnapshot {
    public Vector3 position;
    public bool isRunning;
    public bool isJumping;
    public float scaleX;
}

public class PathRecord : MonoBehaviour
{
    public static Queue<PlayerSnapshot> recordedSnapshots = new Queue<PlayerSnapshot>();
    public int maxHistory = 5000;
    [SerializeField] private Animator playerAnimator;

    void FixedUpdate()
    {
        PlayerSnapshot currentFrame = new PlayerSnapshot {
            position = transform.position,
            isRunning = playerAnimator.GetBool("isRunning"),
            isJumping = playerAnimator.GetBool("isJumping"),
            scaleX = transform.localScale.x
        };
        recordedSnapshots.Enqueue(currentFrame);
        
        if (recordedSnapshots.Count > maxHistory)
        {
            recordedSnapshots.Dequeue();
        }
    }
}
