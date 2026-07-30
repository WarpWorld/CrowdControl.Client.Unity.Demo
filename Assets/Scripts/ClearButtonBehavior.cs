using CrowdControl.Client.Unity;
using CrowdControl.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearButtonBehavior : MonoBehaviour
{
    private Button m_button;
    private TextMeshProUGUI m_buttonText;
    private CrowdControlBehavior m_ccBehavior;

    void Awake()
    {
        m_ccBehavior = FindAnyObjectByType<CrowdControlBehavior>();
        m_button = GetComponentInChildren<Button>();
        m_buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnEnable() => Initialize();

    public void Initialize()
    {
        if (CrowdControlBehavior.IsStoredTokenValid)
            OnValidToken();
        else
        {
            Log.Debug("ClearButtonBehavior: No valid token detected on enable, disabling clear button.");
            m_button.interactable = false;
            m_buttonText.text = "No Stored Token";
        }
    }

    public void OnValidToken()
    {
        Log.Debug("ClearButtonBehavior: Valid token detected, enabling clear button.");
        m_button.interactable = true;
        m_buttonText.text = "Clear Login Token";
    }

    public void ButtonClick()
    {
        Log.Debug("ClearButtonBehavior: Clear button clicked, clearing token and disabling button.");
        m_ccBehavior.ClearToken();
        m_button.interactable = false;
        m_buttonText.text = "No Stored Token";
    }
}
