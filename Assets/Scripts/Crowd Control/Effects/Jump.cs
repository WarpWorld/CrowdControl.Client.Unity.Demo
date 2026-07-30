using CrowdControl.Client.Unity;
using CrowdControl.Common;
using EffectResponse = CrowdControl.Client.WebSocket.EffectResponse;

public class Jump : UnityEffectBase
{
    public override EffectResponse StartEffect(EffectRequest request)
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