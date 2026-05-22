using UnityEngine;

public class NameplateAlignmentBehavior : MonoBehaviour
{
    public Vector3 Offset = new(0f, 1f, 0f);

    public Camera Camera;

    private void LateUpdate()
    {
        transform.position = transform.parent.position + Offset;

        if (!Camera) return;

        Vector3 directionToCamera = -(Camera.transform.position - transform.position);
        transform.rotation = Quaternion.LookRotation(directionToCamera, Camera.transform.up);
    }
}
