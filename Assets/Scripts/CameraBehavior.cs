using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(10, 5, 0);
    void LateUpdate()
    {
        transform.position = target.position + offset;
    }

}
