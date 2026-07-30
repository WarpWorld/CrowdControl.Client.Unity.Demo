using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;
using EffectResponse = CrowdControl.Client.WebSocket.EffectResponse;

public class CreateSpheres : UnityEffectBase
{
    public GameObject BallPrefab;

    public bool ShowNameplateOnSpawns = true;

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

    public override EffectResponse StartEffect(EffectRequest request)
    {
        if ((!m_camera) || (!BallPrefab) || (!SpawnLocation)) return EffectStatus.FailPermanent;

        //add a meter in the camera look direction and spawn the ball there
        Vector3 position = SpawnLocation.position + SPAWN_OFFSET;
        GameObject newSphere = Instantiate(BallPrefab, position, Quaternion.identity);
        if (ShowNameplateOnSpawns)
        {
            string displayViewer = request.DisplayViewer;
            if ((!string.IsNullOrEmpty(displayViewer)))
            {
                SphereBehavior sphereBehavior = newSphere.GetComponent<SphereBehavior>();
                if (sphereBehavior)
                    sphereBehavior.ShowNameplate(displayViewer);
            }
        }
        //newSphere.SetActive(true);

        return EffectStatus.Success;
    }
}