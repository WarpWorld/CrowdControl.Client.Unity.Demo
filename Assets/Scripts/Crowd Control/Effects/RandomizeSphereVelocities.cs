using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;
using EffectResponse = CrowdControl.Client.WebSocket.EffectResponse;

public class RandomizeSphereVelocities : UnityEffectBase
{
    public override EffectResponse StartEffect(EffectRequest request)
    {
        foreach (SphereBehavior sphereBehavior in FindObjectsByType<SphereBehavior>()) SetRandomVelocity(sphereBehavior);

        return EffectStatus.Success;
    }
    
    private void SetRandomVelocity(SphereBehavior sphereBehavior)
    {
        if (sphereBehavior.TryGetComponent(out Rigidbody rb))
        {
            //linear velocity
            Vector2 linearDir2D = Random.insideUnitCircle.normalized;
            Vector3 randomDirection = new Vector3(linearDir2D.x, 0f, linearDir2D.y);
            float randomSpeed = Random.Range(5f, 10f);
            rb.linearVelocity = randomDirection * randomSpeed;
            
            /*//angular velocity
            Vector2 angularDir2D = Random.insideUnitCircle.normalized;
            Vector3 randomAngularDirection = new Vector3(angularDir2D.x, 0f, angularDir2D.y);
            float randomAngularSpeed = Random.Range(5f, 10f);
            rb.angularVelocity = randomAngularDirection * randomAngularSpeed;*/
        }
    }
}