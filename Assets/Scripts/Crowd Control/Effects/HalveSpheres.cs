using CrowdControl.Client.Unity;
using CrowdControl.Client.WebSocket;
using CrowdControl.Common;
using JetBrains.Annotations;
using UnityEngine;

public class HalveBallEffect : UnityEffectBase
{
    public HalveBallEffect([NotNull] CrowdControl.Client.WebSocket.CrowdControl crowdControl, [NotNull] ClientSocket client) : base(crowdControl, client) { }

    public override EffectStatus StartEffect(EffectRequest request)
    {
        if (SphereBehavior.InstanceCount == 0)
            return EffectStatus.FailTemporary;

        int i = 0;
        foreach (GameObject ball in SphereBehavior.ActiveBalls)
        {
            if (i++ % 2 == 0) Destroy(ball);
        }

        return EffectStatus.Success;
    }
}