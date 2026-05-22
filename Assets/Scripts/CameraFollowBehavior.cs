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
    private float m_zoomMultiplier = 1f;

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
    public float smoothSpeed = 10f; // follow speed

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
    /// Zoom sensitivity applied to mouse scroll wheel input.
    /// </summary>
    [Range(0f, 50f)]
    public float mouseScrollZoomSensitivity = 10f;

    /// <summary>
    /// Preferred viewing angle (pitch/tilt in degrees).
    /// </summary>
    [Range(0f, 89f)]
    public float preferredPitchDegrees = 10f;

    void Awake() => m_baseOffset = offset;

    /// <summary>
    /// Handles user input from the mouse and gamepad to update the zoom level and orbit angle of the camera each frame.
    /// </summary>
    void Update()
    {
        if (Mouse.current != null)
        {
            float inputSystemScrollY = -Mouse.current.scroll.ReadValue().y;
            if (inputSystemScrollY != 0f)
                m_zoomMultiplier = Mathf.Clamp(m_zoomMultiplier + (Mathf.Sign(inputSystemScrollY) * mouseScrollZoomSensitivity * Time.deltaTime), 0.5f, 2f);

            if (Mouse.current.middleButton.isPressed)
                m_orbitAngleDegrees += Mouse.current.delta.ReadValue().x * middleMouseDragSensitivity * Time.deltaTime;
        }

        if (Gamepad.current != null)
            m_orbitAngleDegrees += Gamepad.current.rightStick.ReadValue().x * rightStickRotationSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Physics update used to clamp and smoothly follow the target.
    /// </summary>
    void FixedUpdate()
    {
        if (!target) return;

        Vector3 orbitOffset = Quaternion.Euler(0f, m_orbitAngleDegrees, 0f) * (m_baseOffset * m_zoomMultiplier);
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