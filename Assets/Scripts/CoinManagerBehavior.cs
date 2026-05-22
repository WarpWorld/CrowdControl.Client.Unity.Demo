using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinManagerBehavior : MonoBehaviour
{
    private static readonly WaitForSeconds WAIT_30 = new(30f);
    public TextMeshProUGUI CoinCounter;

    public int CollectedCoinCount { get; private set; }

    private List<GameObject> m_allCoins = new();

    private const string COIN_TAG = "Coin";

    private Guid m_instanceID;

    void Start()
    {
        m_instanceID = Guid.NewGuid();
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
        m_instanceID = Guid.NewGuid();
        CollectedCoinCount = 0;
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        if (!CoinCounter) return;
        CoinCounter.text = $"Coins: {CollectedCoinCount}";
    }
}
