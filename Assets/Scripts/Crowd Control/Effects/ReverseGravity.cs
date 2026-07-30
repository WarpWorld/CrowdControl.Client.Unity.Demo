using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;
using EffectResponse = CrowdControl.Client.WebSocket.EffectResponse;

public class ReverseGravity : UnityEffectBase
{
    private Vector3 m_originalGravity;

    protected override void Awake()
    {
        base.Awake();
        m_originalGravity = Physics.gravity;
    }

    public override EffectResponse StartEffect(EffectRequest request)
    {
        Physics.gravity = -m_originalGravity;
        return EffectStatus.Success;
    }

    public override EffectResponse? PauseEffect(EffectRequest request)
    {
        Physics.gravity = m_originalGravity;
        return EffectStatus.Success;
    }

    public override EffectResponse? ResumeEffect(EffectRequest request)
    {
        Physics.gravity = -m_originalGravity;
        return EffectStatus.Success;
    }

    public override EffectResponse? StopEffect(EffectRequest request)
    {
        Physics.gravity = m_originalGravity;
        return EffectStatus.Success;
    }
}