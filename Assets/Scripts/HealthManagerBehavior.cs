using System.Linq;
using TMPro;
using UnityEngine;

public class HealthManagerBehavior : MonoBehaviour
{
    public TextMeshProUGUI HealthCounter;

    public CoinManagerBehavior CoinManager;

    public GameObject SpherePrefab;

    public CameraFollowBehavior CameraFollow;

    public int Health { get; private set; }

    private Vector3 m_originalSpherePosition;

    private Quaternion m_originalSphereRotation;

    private const int MAX_HEALTH = 100;


    /// <summary>
    /// 
    /// </summary>
    public static bool Invincible = false;

    void Start()
    {
        CacheOriginalSphereState();
        ResetHealth();
    }

    public void AddHealth(int amount, bool ignoreInvincibility = false)
    {
        if (Invincible && (!ignoreInvincibility) && (amount < 0)) return;
        SetHealth(Health + amount);
        Debug.Log($"Added {amount} health. Current health: {Health}");
    }

    public void SetHealth(int amount)
    {
        Health = Mathf.Clamp(amount, 0, MAX_HEALTH);

        if (Health == 0)
        {
            HealthDepleted();
            return;
        }

        UpdateCounterText();
    }

    public void ResetHealth()
    {
        Health = MAX_HEALTH;
        UpdateCounterText();
    }

    private void CacheOriginalSphereState()
    {
        m_originalSpherePosition = SpherePrefab.transform.position;
        m_originalSphereRotation = SpherePrefab.transform.rotation;
    }

    private void HealthDepleted()
    {
        ResetHealth();
        GameObject freshSphere = SpawnFreshSphere();
        DestroyAllSpheres(freshSphere);
        CoinManager.ResetCoins();
    }

    private void DestroyAllSpheres(GameObject sphereToKeep = null)
    {
        foreach (GameObject sphere in SphereBehavior.ActiveBalls.ToArray())
            if (sphere && sphere != sphereToKeep)
                Destroy(sphere);
    }

    private GameObject SpawnFreshSphere()
    {
        GameObject newSphere = Instantiate(SpherePrefab, m_originalSpherePosition, m_originalSphereRotation);
        CameraFollow.target = newSphere.transform;
        return newSphere;
    }

    private void UpdateCounterText() => HealthCounter.text = $"Health {Health}%";
}
