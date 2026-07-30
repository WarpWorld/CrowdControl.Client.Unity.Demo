using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;
using EffectResponse = CrowdControl.Client.WebSocket.EffectResponse;

public class InvertControls : UnityEffectBase
{
    public override EffectResponse StartEffect(EffectRequest request)
    {
        SphereBehavior.InputMultiplier = -Vector3.one;
        return EffectStatus.Success;
    }

    public override EffectResponse? PauseEffect(EffectRequest request)
    {
        SphereBehavior.InputMultiplier = Vector3.one;
        return EffectStatus.Success;
    }

    public override EffectResponse? ResumeEffect(EffectRequest request)
    {
        SphereBehavior.InputMultiplier = -Vector3.one;
        return EffectStatus.Success;
    }

    public override EffectResponse? StopEffect(EffectRequest request)
    {
        SphereBehavior.InputMultiplier = Vector3.one;
        return EffectStatus.Success;
    }
}
