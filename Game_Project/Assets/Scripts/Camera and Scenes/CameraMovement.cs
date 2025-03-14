using UnityEngine;

public class CameraMovement: MonoBehaviour
{
    public Transform target;
    public float smoothing = 0.125f;
    public Vector2 maxPosition;
    public Vector2 minPosition;

    void Start()
    {

    }
    void LateUpdate()
    {
        if(transform.position != target.position)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
