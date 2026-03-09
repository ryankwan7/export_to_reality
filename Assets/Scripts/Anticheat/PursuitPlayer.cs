using UnityEngine;

public class PursuitPlayer : MonoBehaviour
{
    public int frameDelay = 60; // Delay between mover and anticheat movement
    private int currentFrame = 0;

    void FixedUpdate()
    {
        currentFrame++;
        if (currentFrame > frameDelay && PathRecord.recordedPositions.Count > 0)
        {
            transform.position = PathRecord.recordedPositions.Dequeue();
        }
    }
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     MoverController2D playerScript = collision.gameObject.GetComponent<MoverController2D>();
    //     if (playerScript != null)
    //     {
    //         Debug.Log("Caught by Anticheat");
    //         playerScript.ResetPlayer();
    //     }
    // }
}
