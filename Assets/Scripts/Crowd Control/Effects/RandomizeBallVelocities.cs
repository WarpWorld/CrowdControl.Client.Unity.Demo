using CrowdControl.Client.Unity;
using CrowdControl.Client.WebSocket;
using CrowdControl.Common;
using JetBrains.Annotations;
using UnityEngine;

public class RandomizeBallVelocities : UnityEffectBase
{
    public RandomizeBallVelocities([NotNull] CrowdControl.Client.WebSocket.CrowdControl crowdControl, [NotNull] ClientSocket client) : base(crowdControl, client) { }

    public override EffectStatus StartEffect(EffectRequest request)
    {
        foreach (SphereBehavior sphereBehavior in FindObjectsByType<SphereBehavior>(FindObjectsSortMode.None)) SetRandomVelocity(sphereBehavior);

        return EffectStatus.Success;
    }
    
    private void SetRandomVelocity(SphereBehavior sphereBehavior)
    {
        if (sphereBehavior.TryGetComponent(out Rigidbody rb))
        {
            //linear velocity
            Vector3 randomDirection = Random.onUnitSphere;
            float randomSpeed = Random.Range(5f, 15f);
            rb.linearVelocity = randomDirection * randomSpeed;
            
            //angular velocity
            Vector3 randomAngularDirection = Random.onUnitSphere;
            float randomAngularSpeed = Random.Range(5f, 15f);
            rb.angularVelocity = randomAngularDirection * randomAngularSpeed;
        }
    }
}