using CrowdControl.Client.Unity;
using CrowdControl.Common;
using System.Linq;
using UnityEngine;

public class HalveBallEffect : UnityEffectBase
{
    private Transform m_cameraFollow;

    protected override void Awake()
    {
        m_cameraFollow = Camera.main?.GetComponent<CameraFollowBehavior>()?.target;
        base.Awake();
    }

    public override EffectStatus StartEffect(EffectRequest request)
    {
        if (SphereBehavior.InstanceCount == 0)
            return EffectStatus.FailTemporary;

        int i = 0;
        foreach (GameObject ball in SphereBehavior.ActiveBalls.ToArray())
        {
            if (ball.transform == m_cameraFollow) continue; //don't destoy the ball the camera is following
            if (i++ % 2 == 0) Destroy(ball);
        }

        return EffectStatus.Success;
    }
}