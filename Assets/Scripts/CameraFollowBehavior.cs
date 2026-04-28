using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Smoothly follows a target while clamping the camera within the ground collider bounds.
/// The ground defines an infinite-height bounding box: X/Z clamped to bounds, Y clamped above the ground top.
/// </summary>
public class CameraFollowBehavior : MonoBehaviour
{
    private Vector3 m_baseOffset;
    private float m_orbitAngleDegrees;

    /// <summary>
    /// The transform to follow.
    /// </summary>
    public Transform target; // the object to follow

    /// <summary>
    /// Ground object defining the bounding box: sides from width/breadth, bottom from top surface.
    /// Preferred source for precise bounds.
    /// </summary>
    public Collider groundCollider; // preferred source for precise bounds

    /// <summary>
    /// Offset added to the target position to place the camera.
    /// </summary>
    public Vector3 offset = new(0, 1, -5); // default camera offset

    /// <summary>
    /// Follow speed used for smoothing.
    /// </summary>
    [Range(0f, 20f)]
    public float smoothSpeed = 5f; // follow speed

    /// <summary>
    /// Rotation sensitivity when dragging with the middle mouse button.
    /// </summary>
    [Range(0f, 50f)]
    public float middleMouseDragSensitivity = 10f;

    /// <summary>
    /// Rotation speed in degrees per second from the gamepad right stick horizontal input.
    /// </summary>
    [Range(0f, 360f)]
    public float rightStickRotationSpeed = 180f;

    /// <summary>
    /// Optional legacy input axis name for the gamepad right stick horizontal input.
    /// Leave empty when using the Input System package.
    /// </summary>
    public string legacyRightStickHorizontalAxis = string.Empty;

    /// <summary>
    /// Preferred viewing angle (pitch/tilt in degrees).
    /// </summary>
    [Range(0f, 89f)]
    public float preferredPitchDegrees = 10f;

    private void Awake()
    {
        m_baseOffset = offset;
    }

    /// <summary>
    /// Physics update used to clamp and smoothly follow the target.
    /// </summary>
    void FixedUpdate()
    {
        if (!target) return;

        float orbitDeltaDegrees = 0f;

        if (Input.GetMouseButton(2))
            orbitDeltaDegrees += Input.GetAxis("Mouse X") * middleMouseDragSensitivity;

        if (Gamepad.current != null)
            orbitDeltaDegrees += Gamepad.current.rightStick.ReadValue().x * rightStickRotationSpeed * Time.deltaTime;

        if (!string.IsNullOrWhiteSpace(legacyRightStickHorizontalAxis))
            orbitDeltaDegrees += Input.GetAxis(legacyRightStickHorizontalAxis) * rightStickRotationSpeed * Time.deltaTime;

        m_orbitAngleDegrees += orbitDeltaDegrees;

        Vector3 orbitOffset = Quaternion.Euler(0f, m_orbitAngleDegrees, 0f) * m_baseOffset;
        Vector3 desiredPosition = target.position + orbitOffset;

        // Clamp desired position within ground-defined bounds using collider
        if (groundCollider)
        {
            Bounds bounds = groundCollider.bounds;

            // Sides: clamp X and Z within ground width/breadth
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, bounds.min.x, bounds.max.x);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, bounds.min.z, bounds.max.z);

            // Bottom: keep Y at or above the ground's top (infinitely tall upwards)
            desiredPosition.y = Mathf.Max(desiredPosition.y, bounds.max.y);
        }

        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Apply preferred pitch while yawing toward the target
        Vector3 toTarget = target.position - transform.position;
        Vector3 planar = new Vector3(toTarget.x, 0f, toTarget.z);
        float yawDeg = planar.sqrMagnitude > 1e-6f ? Mathf.Atan2(planar.x, planar.z) * Mathf.Rad2Deg : transform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(preferredPitchDegrees, yawDeg, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}