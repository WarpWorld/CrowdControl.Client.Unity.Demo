using UnityEngine;

/// <summary>
/// Smoothly follows a target while clamping the camera within the ground collider bounds.
/// The ground defines an infinite-height bounding box: X/Z clamped to bounds, Y clamped above the ground top.
/// </summary>
public class CameraFollowBehavior : MonoBehaviour
{
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
        /// Preferred viewing angle (pitch/tilt in degrees).
        /// </summary>
        [Range(0f, 89f)]
        public float preferredPitchDegrees = 10f;

        /// <summary>
        /// Physics update used to clamp and smoothly follow the target.
        /// </summary>
        void FixedUpdate()
        {
            if (!target) return;
            Vector3 desiredPosition = target.position + offset;

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