using CrowdControl.Client.Unity;
using CrowdControl.Common;
using EffectResponse = CrowdControl.Client.WebSocket.EffectResponse;

public class GiveCoins : UnityEffectBase
{
    private CoinManagerBehavior m_coinManager;

    protected override void Awake()
    {
        base.Awake();
        m_coinManager = FindAnyObjectByType<CoinManagerBehavior>();
    }

    public override EffectResponse StartEffect(EffectRequest request)
    {
        if (!m_coinManager) return EffectStatus.FailTemporary;
        m_coinManager.AddCoins((int)request.Quantity);
        return EffectStatus.Success;
    }
}
