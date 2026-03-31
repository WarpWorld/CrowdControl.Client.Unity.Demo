using CrowdControl.Client.Unity;
using TMPro;
using UnityEngine;

public class ConnectButtonBehavior : MonoBehaviour
{
    private TextMeshProUGUI m_buttonText;
    private CrowdControlBehavior m_ccBehavior;

    private bool m_isConnected = false;

    private void Awake()
    {
        m_ccBehavior = FindFirstObjectByType<CrowdControlBehavior>();
        m_buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ButtonClick()
    {
        if (m_isConnected)
        {
            m_ccBehavior.Disconnect();
            m_isConnected = false;
            m_buttonText.text = "Connect";
        }
        else
        {
            m_ccBehavior.Connect();
            m_isConnected = true;
            m_buttonText.text = "Disconnect";
        }
    }
}
