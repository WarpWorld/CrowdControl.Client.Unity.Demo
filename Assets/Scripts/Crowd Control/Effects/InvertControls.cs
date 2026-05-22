using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;

public class InvertControls : UnityEffectBase
{
    public override EffectStatus StartEffect(EffectRequest request)
    {
        SphereBehavior.InputMultiplier = -Vector3.one;
        return EffectStatus.Success;
    }

    public override EffectStatus? PauseEffect(EffectRequest request)
    {
        SphereBehavior.InputMultiplier = Vector3.one;
        return EffectStatus.Success;
    }

    public override EffectStatus? ResumeEffect(EffectRequest request)
    {
        SphereBehavior.InputMultiplier = -Vector3.one;
        return EffectStatus.Success;
    }

    public override EffectStatus? StopEffect(EffectRequest request)
    {
        SphereBehavior.InputMultiplier = Vector3.one;
        return EffectStatus.Success;
    }
}
