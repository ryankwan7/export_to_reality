using UnityEngine;

public class PortraitCamera : MonoBehaviour
{
    public Transform target;        // drag your Mover here
    public Vector3 offset;          // set in Inspector, e.g. (0, 0.5f, -2f)

    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
