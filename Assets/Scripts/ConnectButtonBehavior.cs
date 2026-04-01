using CrowdControl.Client.Unity;
using CrowdControl.Client.WebSocket.Data;
using TMPro;
using UnityEngine;

public class ConnectButtonBehavior : MonoBehaviour
{
    private TextMeshProUGUI m_buttonText;
    private CrowdControlBehavior m_ccBehavior;
    private bool m_isConnecting;
    private bool m_sessionReady;

    private void Awake()
    {
        m_ccBehavior = FindFirstObjectByType<CrowdControlBehavior>();
        m_buttonText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateButtonText();
    }

    private void OnEnable()
    {
        if (!m_ccBehavior) return;

        m_ccBehavior.AuthCodeReceived += OnAuthCodeReceived;
        m_ccBehavior.AuthCodeErrorReceived += OnAuthCodeErrorReceived;
        m_ccBehavior.SessionReady += OnSessionReady;
        m_ccBehavior.SessionEnded += OnSessionEnded;
        UpdateButtonText();
    }

    private void OnDisable()
    {
        if (!m_ccBehavior) return;

        m_ccBehavior.AuthCodeReceived -= OnAuthCodeReceived;
        m_ccBehavior.AuthCodeErrorReceived -= OnAuthCodeErrorReceived;
        m_ccBehavior.SessionReady -= OnSessionReady;
        m_ccBehavior.SessionEnded -= OnSessionEnded;
    }

    public void ButtonClick()
    {
        if (!m_ccBehavior)
            return;

        if (m_sessionReady || m_isConnecting)
        {
            m_ccBehavior.Disconnect();
            m_isConnecting = false;
            m_sessionReady = false;
            UpdateButtonText();
        }
        else
        {
            m_isConnecting = true;
            m_ccBehavior.Connect();
            UpdateButtonText();
        }
    }

    private void OnAuthCodeReceived(ApplicationAuthCode authCode)
    {
        m_isConnecting = true;
        UpdateButtonText();

        if (!string.IsNullOrWhiteSpace(authCode.Url))
            Application.OpenURL(authCode.Url);
    }

    private void OnAuthCodeErrorReceived(ApplicationAuthCodeError _)
    {
        m_isConnecting = false;
        m_sessionReady = false;
        UpdateButtonText();
    }

    private void OnSessionReady()
    {
        m_isConnecting = false;
        m_sessionReady = true;
        UpdateButtonText();
    }

    private void OnSessionEnded()
    {
        m_isConnecting = false;
        m_sessionReady = false;
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (!m_buttonText)
            return;

        if (m_sessionReady)
            m_buttonText.text = "Disconnect";
        else if (m_isConnecting || (m_ccBehavior && m_ccBehavior.Connected))
            m_buttonText.text = "Cancel";
        else
            m_buttonText.text = "Connect";
    }
}
