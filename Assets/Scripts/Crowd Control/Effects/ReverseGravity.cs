using CrowdControl.Client.Unity;
using CrowdControl.Client.WebSocket;
using CrowdControl.Common;
using JetBrains.Annotations;
using UnityEngine;

public class ReverseGravity : UnityEffectBase
{
    private Vector3 originalGravity;

    public ReverseGravity([NotNull] CrowdControl.Client.WebSocket.CrowdControl crowdControl, [NotNull] ClientSocket client) : base(crowdControl, client) { }

    protected override void Awake()
    {
        base.Awake();
        originalGravity = Physics.gravity;
    }

    public override EffectStatus StartEffect(EffectRequest request)
    {
        Physics.gravity = -originalGravity;
        return EffectStatus.Success;
    }

    public override EffectStatus? PauseEffect(EffectRequest request)
    {
        Physics.gravity = originalGravity;
        return EffectStatus.Success;
    }

    public override EffectStatus? ResumeEffect(EffectRequest request)
    {
        Physics.gravity = -originalGravity;
        return EffectStatus.Success;
    }

    public override EffectStatus? StopEffect(EffectRequest request)
    {
        Physics.gravity = originalGravity;
        return EffectStatus.Success;
    }
}