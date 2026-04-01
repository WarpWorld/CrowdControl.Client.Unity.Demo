using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;

public class ReverseGravity : UnityEffectBase
{
    private Vector3 m_originalGravity;

    protected override void Awake()
    {
        base.Awake();
        m_originalGravity = Physics.gravity;
    }

    public override EffectStatus StartEffect(EffectRequest request)
    {
        Physics.gravity = -m_originalGravity;
        return EffectStatus.Success;
    }

    public override EffectStatus? PauseEffect(EffectRequest request)
    {
        Physics.gravity = m_originalGravity;
        return EffectStatus.Success;
    }

    public override EffectStatus? ResumeEffect(EffectRequest request)
    {
        Physics.gravity = -m_originalGravity;
        return EffectStatus.Success;
    }

    public override EffectStatus? StopEffect(EffectRequest request)
    {
        Physics.gravity = m_originalGravity;
        return EffectStatus.Success;
    }
}