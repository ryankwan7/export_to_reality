using UnityEngine;
using System.Collections.Generic;

public class PathRecord : MonoBehaviour
{
    public static Queue<Vector3> recordedPositions = new Queue<Vector3>();
    public int maxHistory = 5000;
    
    void FixedUpdate()
    {
        recordedPositions.Enqueue(transform.position);
        if (recordedPositions.Count > maxHistory)
        {
            recordedPositions.Dequeue();
        }
    }
}
