using CrowdControl.Client.Unity;
using CrowdControl.Client.WebSocket.Data;
using TMPro;
using UnityEngine;

public class ConnectButtonBehavior : MonoBehaviour
{
    private TextMeshProUGUI m_buttonText;
    private CrowdControlBehavior m_ccBehavior;

    private bool m_isConnected = false;
    private bool m_isConnecting = false;

    private void Awake()
    {
        m_ccBehavior = FindFirstObjectByType<CrowdControlBehavior>();
        m_buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnSessionReady()
    {
        m_isConnecting = false;
        m_isConnected = true;
        m_buttonText.text = "Disconnect";
    }

    public void OnSessionEnded()
    {
        m_isConnected = false;
        m_buttonText.text = "Connect";
    }

    public void OnAuthCodeReceived(ApplicationAuthCode authCode)
    {
        m_isConnecting = true;
        m_buttonText.text = "Connecting...";
    }

    public void OnAuthCodeErrorReceived(ApplicationAuthCodeError authCode)
    {
        m_isConnected = false;
        m_buttonText.text = "Connect";
    }

    public void ButtonClick()
    {
        if (m_isConnecting) return;
        if (m_isConnected) m_ccBehavior.Disconnect();
        else m_ccBehavior.Connect();
    }
}
