using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinManagerBehavior : MonoBehaviour
{
    public TextMeshProUGUI CoinCounter;

    public int CollectedCoinCount { get; private set; }

    private List<GameObject> m_allCoins = new();

    private const string COIN_TAG = "Coin";

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
        CollectedCoinCount++;
        UpdateCounterText();
        return true;
    }

    public void AddCoins(int amount)
    {
        CollectedCoinCount += amount;
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        if (!CoinCounter) return;
        CoinCounter.text = $"Coins: {CollectedCoinCount}";
    }
}
