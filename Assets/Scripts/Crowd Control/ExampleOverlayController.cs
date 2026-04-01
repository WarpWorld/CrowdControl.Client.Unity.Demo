using CrowdControl.Client.Unity;
using CrowdControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExampleOverlayController : MonoBehaviour
{
    [SerializeField]
    private bool m_showOverlay = true;

    [SerializeField]
    private int m_maxMessages = 5;

    [SerializeField]
    private float m_messageLifetime = 6f;

    private readonly Dictionary<Guid, EffectRequest> m_queuedEffects = new();

    private readonly Dictionary<Guid, ActiveEffectEntry> m_activeEffects = new();

    private readonly List<OverlayMessage> m_messages = new();

    private GUIStyle m_titleStyle;
    private GUIStyle m_textStyle;
    private GUIStyle m_boxStyle;

    private sealed class OverlayMessage
    {
        public string Text;
        public float ExpireAt;
    }

    private sealed class ActiveEffectEntry
    {
        public EffectState State;
        public float ExpiresAt;
    }

    public void OnEffectRequest(EffectRequest request)
    {
        m_queuedEffects[request.ID] = request;
        AddMessage($"Requested: {request.EffectID}");
        Debug.Log($"Effect Requested with ID: {request.EffectID}");
    }

    public void OnEffectUpdate(EffectState state)
    {
        m_queuedEffects.Remove(state.Request.ID);

        string effectName = GetEffectName(state);
        string statusName = state.Response.Status.ToString();
        UnityEffectBase unityEffect = state.Effect as UnityEffectBase;
        bool isTimedEffect = unityEffect != null && unityEffect.IsTimed;

        if (IsActiveStatus(statusName) && isTimedEffect)
        {
            m_activeEffects[state.Request.ID] = new ActiveEffectEntry
            {
                State = state,
                ExpiresAt = Time.unscaledTime + GetExpectedDuration(unityEffect)
            };
        }
        else if (IsTerminalStatus(statusName) || !isTimedEffect)
            m_activeEffects.Remove(state.Request.ID);

        AddMessage($"{effectName}: {DescribeStatus(statusName, isTimedEffect)}");
        Debug.Log($"Effect Updated with ID: {state.Request.EffectID}, State: {state.Response.Status}");
    }

    private void Update()
    {
        float now = Time.unscaledTime;
        m_messages.RemoveAll(message => message.ExpireAt <= now);

        foreach (Guid effectId in m_activeEffects.Where(pair => pair.Value.ExpiresAt <= now).Select(pair => pair.Key).ToArray())
        {
            AddMessage($"{GetEffectName(m_activeEffects[effectId].State)}: ended");
            m_activeEffects.Remove(effectId);
        }
    }

    private void OnGUI()
    {
        if (!m_showOverlay)
            return;

        EnsureStyles();

        Rect panelRect = new Rect(20f, 20f, 420f, 220f);
        GUILayout.BeginArea(panelRect, GUIContent.none, m_boxStyle);
        GUILayout.Label("Crowd Control", m_titleStyle);

        if (m_activeEffects.Count > 0)
        {
            GUILayout.Label("Active effects", m_titleStyle);
            foreach (string activeEffect in m_activeEffects.Values.Select(entry => GetEffectName(entry.State)).Distinct())
                GUILayout.Label(activeEffect, m_textStyle);
        }

        if (m_messages.Count > 0)
        {
            GUILayout.Space(8f);
            GUILayout.Label("Recent activity", m_titleStyle);
            foreach (OverlayMessage message in m_messages)
                GUILayout.Label(message.Text, m_textStyle);
        }

        if (m_activeEffects.Count == 0 && m_messages.Count == 0)
            GUILayout.Label("No effects yet.", m_textStyle);

        GUILayout.EndArea();
    }

    private void AddMessage(string text)
    {
        m_messages.Add(new OverlayMessage
        {
            Text = text,
            ExpireAt = Time.unscaledTime + m_messageLifetime
        });

        if (m_messages.Count > m_maxMessages)
            m_messages.RemoveRange(0, m_messages.Count - m_maxMessages);
    }

    private static string GetEffectName(EffectState state)
    {
        if (state.Effect is UnityEffectBase unityEffect && !string.IsNullOrWhiteSpace(unityEffect.Name))
            return unityEffect.Name;

        return state.Request.EffectID;
    }

    private static bool IsActiveStatus(string statusName)
    {
        return statusName.Contains("Success", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Resume", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Start", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Active", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Running", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsTerminalStatus(string statusName)
    {
        return statusName.Contains("Stop", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Finish", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Fail", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Reject", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Cancel", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Pause", StringComparison.OrdinalIgnoreCase)
            || statusName.Contains("Error", StringComparison.OrdinalIgnoreCase);
    }

    private static string DescribeStatus(string statusName, bool isTimedEffect)
    {
        if (statusName.Contains("Success", StringComparison.OrdinalIgnoreCase))
            return isTimedEffect ? "started" : "applied";
        if (statusName.Contains("Resume", StringComparison.OrdinalIgnoreCase))
            return "resumed";
        if (statusName.Contains("Pause", StringComparison.OrdinalIgnoreCase))
            return "paused";
        if (statusName.Contains("Stop", StringComparison.OrdinalIgnoreCase) || statusName.Contains("Finish", StringComparison.OrdinalIgnoreCase))
            return "stopped";
        if (statusName.Contains("Fail", StringComparison.OrdinalIgnoreCase) || statusName.Contains("Error", StringComparison.OrdinalIgnoreCase))
            return "failed";

        return statusName;
    }

    private static float GetExpectedDuration(UnityEffectBase unityEffect)
    {
        return Mathf.Max(0.1f, unityEffect.DefaultDuration);
    }

    private void EnsureStyles()
    {
        if (m_boxStyle != null)
            return;

        m_boxStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(12, 12, 12, 12),
            alignment = TextAnchor.UpperLeft,
            fontSize = 16
        };

        m_titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        m_textStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            wordWrap = true,
            normal = { textColor = new Color(0.9f, 0.95f, 1f) }
        };
    }
}