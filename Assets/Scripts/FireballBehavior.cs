using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FireballBehavior : MonoBehaviour
{
    private const float WAIT_BEFORE_MOVE_SECONDS = 5f;
    private const float DESPAWN_SECONDS = 10f;
    private const float SEARCH_RADIUS = 20f;
    private const float MAX_MOVE_DISTANCE = 10f;
    private const float MOVE_SPEED = 2f;
    private const float HOVER_AMPLITUDE = 0.1f;
    private const float SPAWN_DURATION = 1f;
    private const float DESPAWN_DURATION = 0.1f;

    private static readonly Vector3 RESPAWN_MIN = new(-50f, 1f, -50f);
    private static readonly Vector3 RESPAWN_MAX = new(50f, 1f, 50f);

    public HealthManagerBehavior HealthManager;

    private Renderer m_renderer;
    private SphereCollider m_collider;
    private Rigidbody m_rigidbody;
    private Vector3 m_basePosition;
    private Vector3 m_originalScale;
    private bool m_isDespawned;
    private bool m_hitHandled;
    private Coroutine m_moveCoroutine;
    private Coroutine m_scaleCoroutine;
    private Coroutine m_behaviorCoroutine;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<SphereCollider>();
        m_renderer = GetComponent<MeshRenderer>();
        m_basePosition = transform.position;
        m_originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void OnEnable()
    {
        m_basePosition = transform.position;
        m_isDespawned = false;
        m_hitHandled = false;
        transform.localScale = Vector3.zero;
        m_renderer.enabled = true;
        m_collider.enabled = true;

        if (m_behaviorCoroutine != null)
            StopCoroutine(m_behaviorCoroutine);

        m_behaviorCoroutine = StartCoroutine(BeginBehaviorLoop());
    }

    void OnDisable()
    {
        if (m_behaviorCoroutine != null)
        {
            StopCoroutine(m_behaviorCoroutine);
            m_behaviorCoroutine = null;
        }

        if (m_moveCoroutine != null)
        {
            StopCoroutine(m_moveCoroutine);
            m_moveCoroutine = null;
        }

        if (m_scaleCoroutine != null)
        {
            StopCoroutine(m_scaleCoroutine);
            m_scaleCoroutine = null;
        }
    }

    void FixedUpdate()
    {
        if (m_isDespawned)
            return;

        SyncPosition();
    }

    private IEnumerator BeginBehaviorLoop()
    {
        yield return AnimateScale(Vector3.zero, m_originalScale, SPAWN_DURATION);
        m_behaviorCoroutine = StartCoroutine(MoveTowardsNearestSphereLoop());
    }

    private IEnumerator MoveTowardsNearestSphereLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(WAIT_BEFORE_MOVE_SECONDS);

            if (m_isDespawned)
                continue;

            TryMoveTowardsNearestSphere();
        }
    }

    private void TryMoveTowardsNearestSphere()
    {
        GameObject nearestSphere = null;
        float nearestDistance = float.MaxValue;

        foreach (GameObject sphere in SphereBehavior.ActiveBalls)
        {
            if (!sphere)
                continue;

            float distance = Vector3.Distance(m_basePosition, sphere.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestSphere = sphere;
            }
        }

        if (!nearestSphere || nearestDistance > SEARCH_RADIUS)
            return;

        Vector3 direction = nearestSphere.transform.position - m_basePosition;
        if (direction.sqrMagnitude <= 0f)
            return;

        if (m_moveCoroutine != null)
            StopCoroutine(m_moveCoroutine);

        m_moveCoroutine = StartCoroutine(MoveTowardsSphere(nearestSphere.transform));
    }

    private IEnumerator MoveTowardsSphere(Transform sphereTransform)
    {
        float remainingDistance = MAX_MOVE_DISTANCE;

        while (!m_isDespawned && remainingDistance > 0f && sphereTransform)
        {
            Vector3 direction = sphereTransform.position - m_basePosition;
            float distanceToSphere = direction.magnitude;
            if (distanceToSphere <= 0f)
                break;

            float stepDistance = Mathf.Min(MOVE_SPEED * Time.deltaTime, remainingDistance, distanceToSphere);
            m_basePosition += direction.normalized * stepDistance;
            remainingDistance -= stepDistance;
            SyncPosition();

            yield return null;
        }

        m_moveCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<SphereBehavior>())
            return;

        HandleSphereHit();
    }

    private void HandleSphereHit()
    {
        if (m_isDespawned || m_hitHandled)
            return;

        m_hitHandled = true;
        HealthManager.AddHealth(-10);
        StartCoroutine(DespawnAndRespawn());
    }

    private IEnumerator DespawnAndRespawn()
    {
        m_isDespawned = true;

        if (m_moveCoroutine != null)
        {
            StopCoroutine(m_moveCoroutine);
            m_moveCoroutine = null;
        }

        if (m_scaleCoroutine != null)
            StopCoroutine(m_scaleCoroutine);

        yield return AnimateScale(m_originalScale, Vector3.zero, DESPAWN_DURATION);

        m_renderer.enabled = false;
        m_collider.enabled = false;

        yield return new WaitForSeconds(DESPAWN_SECONDS);

        m_basePosition = new Vector3(
            Random.Range(RESPAWN_MIN.x, RESPAWN_MAX.x),
            RESPAWN_MIN.y,
            Random.Range(RESPAWN_MIN.z, RESPAWN_MAX.z));

        SyncPosition();
        transform.localScale = Vector3.zero;
        m_renderer.enabled = true;
        m_collider.enabled = true;
        m_isDespawned = false;
        m_hitHandled = false;

        m_scaleCoroutine = StartCoroutine(AnimateScale(Vector3.zero, m_originalScale, SPAWN_DURATION));
    }

    private IEnumerator AnimateScale(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        transform.localScale = endScale;
        m_scaleCoroutine = null;
    }

    private void SyncPosition()
    {
        Vector3 position = m_basePosition;
        position.y += Mathf.Sin(Time.time) * HOVER_AMPLITUDE;
        transform.position = position;
    }
}
