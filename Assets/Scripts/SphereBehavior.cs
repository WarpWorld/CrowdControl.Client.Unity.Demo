using System;
using UnityEngine;

/// <summary>
/// Keeps a physics-driven sphere inside the bounding box defined by a ground collider.
/// Collisions against the X/Z sides reflect velocity like a billiard ball; Y is unconstrained above ground top.
/// </summary>
public class SphereBehavior : MonoBehaviour
{
    /// <summary>
    /// Rigidbody driving the sphere's motion.
    /// </summary>
    public Rigidbody RigidBody;

    /// <summary>
    /// Collider whose world-space bounds define the allowable X/Z area and the ground top for Y.
    /// </summary>
    public Collider GroundCollider;

    /// <summary>
    /// Cached world-space sphere radius for fast boundary tests.
    /// </summary>
    [NonSerialized]
    private float m_radius;

    /// <summary>
    /// Cache the world-space sphere radius once on awake.
    /// </summary>
    void Awake() => m_radius = GetWorldSphereRadius();

    /// <summary>
    /// Physics update: clamp position inside bounds and reflect velocity on wall hits.
    /// </summary>
    void FixedUpdate()
    {
        if (!GroundCollider) return; // nothing to constrain against
        if (!RigidBody) return; // nothing to constrain against

        Bounds bounds = GroundCollider.bounds;

        Vector3 pos = transform.position;
        Vector3 vel = RigidBody.linearVelocity;

        bool hitXMin = pos.x - m_radius < bounds.min.x;
        bool hitXMax = pos.x + m_radius > bounds.max.x;
        bool hitZMin = pos.z - m_radius < bounds.min.z;
        bool hitZMax = pos.z + m_radius > bounds.max.z;

        // Reflect velocity components on boundary hits and clamp position to stay inside
        if (hitXMin) {
            pos.x = bounds.min.x + m_radius;
            if (vel.x < 0f) vel.x = -vel.x; // perfectly elastic
        } else if (hitXMax) {
            pos.x = bounds.max.x - m_radius;
            if (vel.x > 0f) vel.x = -vel.x;
        }

        if (hitZMin) {
            pos.z = bounds.min.z + m_radius;
            if (vel.z < 0f) vel.z = -vel.z;
        } else if (hitZMax) {
            pos.z = bounds.max.z - m_radius;
            if (vel.z > 0f) vel.z = -vel.z;
        }

        // Apply results
        transform.position = pos;
        if (RigidBody) RigidBody.linearVelocity = vel;
    }

    /// <summary>
    /// Compute the sphere's world-space radius from its SphereCollider and transform scale.
    /// </summary>
    private float GetWorldSphereRadius()
    {
        SphereCollider sc = GetComponent<SphereCollider>();
        if (!sc) return 0.5f * Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z));
        Vector3 s = transform.lossyScale;
        float maxScale = Mathf.Max(s.x, Mathf.Max(s.y, s.z));
        return sc.radius * maxScale;
    }
}
