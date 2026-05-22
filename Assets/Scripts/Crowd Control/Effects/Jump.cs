using CrowdControl.Client.Unity;
using CrowdControl.Common;

public class Jump : UnityEffectBase
{
    public override EffectStatus StartEffect(EffectRequest request)
    {
        bool success = false;
        foreach (SphereBehavior sphereBehavior in FindObjectsByType<SphereBehavior>())
        {
            if (sphereBehavior.TryJump())
                success = true;
        }
        return success ? EffectStatus.Success : EffectStatus.FailTemporary;
    }
}