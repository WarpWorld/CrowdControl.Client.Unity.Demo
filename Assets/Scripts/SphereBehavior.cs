using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Keeps a physics-driven sphere inside the bounding box defined by a ground collider.
/// Collisions against the X/Z sides reflect velocity like a billiard ball; Y is unconstrained above ground top.
/// </summary>
public class SphereBehavior : MonoBehaviour
{
    /// <summary>The default movement speed.</summary>
    private static readonly Vector2 MOVEMENT_SPEED = new(5f, 5f);

    /// <summary>The default movement speed.</summary>
    private static readonly Vector3 JUMP_FORCE = new(0f, 5f, 0f);

    /// Maximum velocity magnitude to prevent excessive speeds from accumulating.
    private const float MAX_VELOCITY = 10f;

    /// <summary>Represents the multiplier applied to input values, used to reverse input vectors.</summary>
    public static Vector3 InputMultiplier = Vector3.one;

    /// <summary>Main camera in the scene, used for camera-relative controls.</summary>
    private Camera m_camera;

    /// <summary>Gets the total number of active instances of the class.</summary>
    public static int InstanceCount => s_activeBalls.Count;

    /// <summary>Gets a read-only collection of all active sphere GameObjects.</summary>
    public static IReadOnlyCollection<GameObject> ActiveBalls => s_activeBalls;

    /// Internal list of active sphere GameObjects for tracking instance count and providing access to active spheres.
    private static HashSet<GameObject> s_activeBalls { get; } = new();

    /// <summary>Rigidbody driving the sphere's motion.</summary>
    public Rigidbody RigidBody;

    /// <summary>Collider whose world-space bounds define the allowable X/Z area and the ground top for Y.</summary>
    public Collider GroundCollider;

    /// <summary>The behavior that manages coin count and associated GUI.</summary>
    public CoinManagerBehavior CoinManager;

    /// <summary>Optional nameplate object to display as a child of the sphere for player identification.</summary>
    public TextMeshPro Nameplate;

    /// <summary>
    /// Cached world-space sphere radius for fast boundary tests.
    /// </summary>
    [NonSerialized]
    private float m_radius;

    /// <summary>
    /// Input system actions for this sphere.
    /// </summary>
    private InputSystem_Actions m_input;

    /// <summary>
    /// Latest movement input value (updated by input callbacks) for continuous application.
    /// </summary>
    private Vector2 m_moveInput;

    [SerializeField]
    private float m_groundProbeDistance = 0.2f;

    [SerializeField]
    private LayerMask m_groundMask = ~0;

    /// <summary>
    /// Cache the world-space sphere radius once on awake.
    /// </summary>
    void Awake()
    {
        m_camera = Camera.main;
        if (!RigidBody) RigidBody = GetComponent<Rigidbody>(); //try to find a Rigidbody if not assigned, required for movement controls and boundary reflection on fresh spheres
        m_input = new InputSystem_Actions();
        m_input.Player.Move.performed += OnMove;
        m_input.Player.Move.canceled += OnMove;
        m_input.Player.Jump.performed += OnJump;
        m_radius = GetWorldSphereRadius();
    }

    /// <summary>
    /// On enable, add this sphere to the active set and enable its input actions.
    /// </summary>
    void OnEnable()
    {
        s_activeBalls.Add(gameObject);
        m_input?.Player.Enable();
    }

    /// <summary>
    /// On disable, remove this sphere from the active set and disable its input actions.
    /// </summary>
    void OnDisable()
    {
        m_input?.Player.Disable();
        s_activeBalls.Remove(gameObject);
    }

    /// <summary>
    /// Apply input as a force to the sphere's Rigidbody in the X/Z plane, allowing player control of the sphere's motion.
    /// </summary>
    /// <param name="input">The input context containing the movement vector.</param>
    private void OnMove(InputAction.CallbackContext input) => m_moveInput = input.ReadValue<Vector2>() * InputMultiplier;

    /// <summary>
    /// Handles the jump input action when triggered by the user.
    /// </summary>
    /// <param name="input">The context for the input action, containing information about the input event and its state.</param>
    private void OnJump(InputAction.CallbackContext input) => TryJump();

    public bool TryJump()
    {
        if (!RigidBody || !IsGrounded()) return false;
        RigidBody.AddForce(JUMP_FORCE, ForceMode.Impulse);
        return true;
    }

    /// <summary>
    /// Displays the nameplate with the specified name above the sphere.
    /// </summary>
    /// <param name="name">The name to display on the nameplate.</param>
    /// <remarks>If the nameplate is already active, this method will update the displayed name.</remarks>
    public void ShowNameplate(string name)
    {
        if (!Nameplate) return;
        Nameplate.text = name;
        Nameplate.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the nameplate by deactivating its associated GameObject.
    /// </summary>
    /// <remarks>If the nameplate is already hidden or not assigned, this method has no effect.</remarks>
    public void HideNameplate()
    {
        if (!Nameplate) return;
        Nameplate.gameObject.SetActive(false);
    }

    /// <summary>
    /// Physics update: clamp position inside bounds and reflect velocity on wall hits.
    /// </summary>
    void FixedUpdate()
    {
        if (!RigidBody) return; // nothing to move or constrain

        ApplyMovement();

        // Clamp velocity to prevent excessive speeds
        if (RigidBody.linearVelocity.magnitude > MAX_VELOCITY)
            RigidBody.linearVelocity = RigidBody.linearVelocity.normalized * MAX_VELOCITY;

        if (!GroundCollider) return; // nothing to constrain against

        Bounds bounds = GroundCollider.bounds;

        Vector3 pos = transform.position;
        Vector3 vel = RigidBody.linearVelocity;

        bool hitXMin = pos.x - m_radius < bounds.min.x;
        bool hitXMax = pos.x + m_radius > bounds.max.x;
        bool hitZMin = pos.z - m_radius < bounds.min.z;
        bool hitZMax = pos.z + m_radius > bounds.max.z;

        // Reflect velocity components on boundary hits and clamp position to stay inside
        if (hitXMin)
        {
            pos.x = bounds.min.x + m_radius;
            if (vel.x < 0f) vel.x = -vel.x; // perfectly elastic
        } else if (hitXMax)
        {
            pos.x = bounds.max.x - m_radius;
            if (vel.x > 0f) vel.x = -vel.x;
        }

        if (hitZMin)
        {
            pos.z = bounds.min.z + m_radius;
            if (vel.z < 0f) vel.z = -vel.z;
        } else if (hitZMax)
        {
            pos.z = bounds.max.z - m_radius;
            if (vel.z > 0f) vel.z = -vel.z;
        }

        // Apply results
        transform.position = pos;
        if (RigidBody) RigidBody.linearVelocity = vel;
    }

    private void ApplyMovement()
    {
        if (!RigidBody) return;
        if (!m_camera) return;

        Vector2 movement = m_moveInput * MOVEMENT_SPEED;
        if (movement.sqrMagnitude <= 0f) return;

        Vector3 forward = m_camera.transform.forward;
        Vector3 right = m_camera.transform.right;

        // flatten to XZ plane
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * movement.y + right * movement.x;
        //move = ProjectMoveOntoGround(move);
        RigidBody.AddForce(move);
    }

    private Vector3 ProjectMoveOntoGround(Vector3 move)
    {
        if (move.sqrMagnitude <= 0f) return move;

        Vector3 origin = transform.position + Vector3.up * 0.05f;
        float rayLength = m_radius + Mathf.Max(0.01f, m_groundProbeDistance);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, m_groundMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 normal = hit.normal;
            if (normal.sqrMagnitude > 0.0001f)
                return Vector3.ProjectOnPlane(move, normal);
        }

        return move;
    }

    private bool IsGrounded()
    {
        float rayLength = m_radius + Mathf.Max(0.01f, m_groundProbeDistance);
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            out RaycastHit hit,
            rayLength,
            m_groundMask,
            QueryTriggerInteraction.Ignore) && hit.normal.y > 0.1f;
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other) return;
        if (!other.CompareTag("Coin")) return;
        if (!CoinManager) return;

        CoinManager.TryCollectCoin(other.gameObject);
    }
}
