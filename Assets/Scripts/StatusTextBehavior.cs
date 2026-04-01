using CrowdControl.Client.WebSocket.Data;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StatusTextBehavior : MonoBehaviour
{
    public bool TimeoutEnabled = false;

    private TextMeshProUGUI m_text;

    private const float TIMER_DURATION = 5f;
    private float m_timer = 0f;

    private void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    public void OnAuthCodeReceived(ApplicationAuthCode authCode)
    {
        m_text.text = "Waiting for user to authenticate...";
        m_timer = TIMER_DURATION;
    }

    public void OnAuthCodeRedeemedReceived(ApplicationAuthCodeRedeemed authCode)
    {
        m_text.text = "Crowd Control is connecting...";
        m_timer = TIMER_DURATION;
    }

    public void OnAuthCodeErrorReceived(ApplicationAuthCodeError authCode)
    {
        m_text.text = "An error occurred during authentication.";
        m_timer = TIMER_DURATION;
    }

    public void OnSessionReady()
    {
        m_text.text = "Crowd Control connected...";
        m_timer = TIMER_DURATION;
    }

    public void OnSessionEnded()
    {
        m_text.text = "Crowd Control disconnected...";
        m_timer = TIMER_DURATION;
    }

    void Update()
    {
        if (!TimeoutEnabled) return;
        if (m_timer <= 0f) return;
        m_timer -= Time.deltaTime;
        if (m_timer <= 0f)
            m_text.text = string.Empty;
    }
}
