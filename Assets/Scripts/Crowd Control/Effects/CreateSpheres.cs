using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;

public class CreateSpheres : UnityEffectBase
{
    public GameObject BallPrefab;

    private Camera m_camera;

    private CameraFollowBehavior m_cameraFollow;

    private static readonly Vector3 SPAWN_OFFSET = new(0f, 5f, 0f);

    protected Transform SpawnLocation
    {
        get
        {
            if (!m_cameraFollow) return gameObject.transform;
            return m_cameraFollow.target.transform;
        }
    }

    protected override void Awake()
    {
        m_camera = Camera.main;
        m_cameraFollow = m_camera.GetComponent<CameraFollowBehavior>();
        base.Awake();
    }

    public override EffectStatus StartEffect(EffectRequest request)
    {
        if ((!m_camera) || (!BallPrefab) || (!SpawnLocation)) return EffectStatus.FailPermanent;

        //add a meter in the camera look direction and spawn the ball there
        Vector3 position = SpawnLocation.position + SPAWN_OFFSET;
        Instantiate(BallPrefab, position, Quaternion.identity);
        return EffectStatus.Success;
    }
}