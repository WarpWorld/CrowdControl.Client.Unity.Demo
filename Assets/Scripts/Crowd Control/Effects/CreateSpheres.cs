using CrowdControl.Client.Unity;
using CrowdControl.Client.WebSocket;
using CrowdControl.Common;
using JetBrains.Annotations;
using UnityEngine;

public class CreateSpheres : UnityEffectBase
{
    public GameObject BallPrefab;

    public Transform SpawnLocation;

    public CreateSpheres([NotNull] CrowdControl.Client.WebSocket.CrowdControl crowdControl, [NotNull] ClientSocket client) : base(crowdControl, client) { }

    public override EffectStatus StartEffect(EffectRequest request)
    {
        if (!BallPrefab || !SpawnLocation) return EffectStatus.FailPermanent;
        Instantiate(BallPrefab, SpawnLocation.position, SpawnLocation.rotation);
        return EffectStatus.Success;
    }
}