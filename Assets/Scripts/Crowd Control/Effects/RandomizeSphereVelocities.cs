using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;

public class RandomizeSphereVelocities : UnityEffectBase
{
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