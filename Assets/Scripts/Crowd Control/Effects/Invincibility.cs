using CrowdControl.Client.Unity;
using CrowdControl.Common;
using EffectResponse = CrowdControl.Client.WebSocket.EffectResponse;

public class Invincibility : UnityEffectBase
{
    public override EffectResponse StartEffect(EffectRequest request)
    {
        HealthManagerBehavior.Invincible = true;
        return EffectStatus.Success;
    }

    public override EffectResponse? PauseEffect(EffectRequest request)
    {
        HealthManagerBehavior.Invincible = false;
        return EffectStatus.Success;
    }

    public override EffectResponse? ResumeEffect(EffectRequest request)
    {
        HealthManagerBehavior.Invincible = true;
        return EffectStatus.Success;
    }

    public override EffectResponse? StopEffect(EffectRequest request)
    {
        HealthManagerBehavior.Invincible = false;
        return EffectStatus.Success;
    }
}