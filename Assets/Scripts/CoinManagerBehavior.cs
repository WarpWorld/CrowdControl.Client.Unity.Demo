using CrowdControl.Client.Unity;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinManagerBehavior : MonoBehaviour
{
    private CrowdControlBehavior m_ccBehavior;

    private static readonly WaitForSeconds WAIT_30 = new(30f);
    public TextMeshProUGUI CoinCounter;

    public int CollectedCoinCount { get; private set; }

    private List<GameObject> m_allCoins = new();
    private readonly ConcurrentDictionary<string, object?> m_coinCollectedEventArgs = new()
    {
        ["coins"] = 0
    };

    private const string COIN_TAG = "Coin";


    private void Awake()
    {
        m_ccBehavior = FindAnyObjectByType<CrowdControlBehavior>();
    }

    void Start()
    {
        m_allCoins.AddRange(GameObject.FindGameObjectsWithTag(COIN_TAG));
        UpdateCounterText();
    }

    public bool TryCollectCoin(GameObject coin)
    {
        if ((!coin) || (!coin.activeSelf)) return false;
        if (!coin.CompareTag(COIN_TAG)) return false;

        coin.SetActive(false);
        IEnumerator respawn()
        {
            yield return WAIT_30;
            coin.SetActive(true);
        }
        StartCoroutine(respawn());

        m_coinCollectedEventArgs["coins"] = CollectedCoinCount;
        if (m_ccBehavior) m_ccBehavior.TriggerEvent("CoinCollected", m_coinCollectedEventArgs);
        CollectedCoinCount++;
        UpdateCounterText();
        return true;
    }

    public void AddCoins(int amount)
    {
        CollectedCoinCount += amount;
        UpdateCounterText();
    }

    public void ResetCoins()
    {
        CollectedCoinCount = 0;
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        if (!CoinCounter) return;
        CoinCounter.text = $"Coins: {CollectedCoinCount}";
    }
}
