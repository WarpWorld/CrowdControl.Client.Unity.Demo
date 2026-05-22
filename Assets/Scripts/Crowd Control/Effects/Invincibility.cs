using CrowdControl.Client.Unity;
using CrowdControl.Common;

public class Invincibility : UnityEffectBase
{
    public override EffectStatus StartEffect(EffectRequest request)
    {
        HealthManagerBehavior.Invincible = true;
        return EffectStatus.Success;
    }

    public override EffectStatus? PauseEffect(EffectRequest request)
    {
        HealthManagerBehavior.Invincible = false;
        return EffectStatus.Success;
    }

    public override EffectStatus? ResumeEffect(EffectRequest request)
    {
        HealthManagerBehavior.Invincible = true;
        return EffectStatus.Success;
    }

    public override EffectStatus? StopEffect(EffectRequest request)
    {
        HealthManagerBehavior.Invincible = false;
        return EffectStatus.Success;
    }
}