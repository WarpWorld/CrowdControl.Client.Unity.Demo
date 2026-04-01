using CrowdControl.Client.Unity;
using CrowdControl.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(CanvasRenderer))]
public class ExampleOverlayController : MonoBehaviour
{
    public bool showOverlay = true;

    public int maxMessages = 5;

    public float messageLifetime = 6f;

    [NonSerialized]
    private readonly ConcurrentDictionary<Guid, EffectRequest> m_queuedEffects = new();

    [NonSerialized]
    private readonly ConcurrentDictionary<Guid, ActiveEffectEntry> m_activeEffects = new();

    [NonSerialized]
    private readonly List<OverlayMessage> m_messages = new();

    [Header("Active Effects Header")]
    public TextMeshProUGUI textActiveEffectsHeader;

    [Header("Active Effects List")]
    public TextMeshProUGUI textActiveEffectsList;

    [Header("Recent Activity Header")]
    public TextMeshProUGUI textRecentActivityHeader;

    [Header("Recent Activity List")]
    public TextMeshProUGUI textRecentActivityList;

    [Header("No Effects Header")]
    public TextMeshProUGUI textNoEffects;

    private struct OverlayMessage
    {
        public string Text;
        public float ExpiresAt;
    }

    private struct ActiveEffectEntry
    {
        public EffectState State;
        public float ExpiresAt;
    }

    public void OnEffectRequest(EffectRequest request)
    {
        Debug.Log($"Effect Requested with ID: {request.EffectID}");
        m_queuedEffects[request.ID] = request;
        AddMessage($"Requested: {request.EffectID}");
    }

    public void OnEffectUpdate(EffectState state)
    {
        Debug.Log($"Effect Updated with ID: {state.Request.EffectID}, State: {state.Response.Status}");
        m_queuedEffects.TryRemove(state.Request.ID, out _);

        string effectName = GetEffectName(state);
        EffectStatus status = state.Response.Status;
        UnityEffectBase unityEffect = state.Effect as UnityEffectBase;
        bool isTimedEffect = unityEffect && unityEffect.IsTimed;

        if(isTimedEffect)
        {
            switch(status)
            {
                case EffectStatus.Success:
                case EffectStatus.TimedBegin:
                case EffectStatus.TimedResume:
                    Log.Debug($"Adding/updating active effect with ID: {state.Request.ID}, expires in: {state.Response.TimeRemaining} seconds");
                    m_activeEffects[state.Request.ID] = new()
                    {
                        State = state,
                        ExpiresAt = Time.unscaledTime + Mathf.Max(0.1f, (float)state.Response.TimeRemaining)
                    };
                    break;
                case EffectStatus.TimedPause:
                    m_activeEffects[state.Request.ID] = new()
                    {
                        State = state,
                        ExpiresAt = float.MaxValue
                    };
                    break;
                case EffectStatus.FailTemporary:
                case EffectStatus.FailPermanent:
                case EffectStatus.TimedEnd:
                case EffectStatus.TimedCanceled:
                case EffectStatus.TimedAborted:
                    m_activeEffects.TryRemove(state.Request.ID, out _);
                    break;
            }
        }
        else
            m_activeEffects.TryRemove(state.Request.ID, out _);

        AddMessage($"{effectName}: {DescribeStatus(status, isTimedEffect)}");
    }

    private void Update()
    {
        float now = Time.unscaledTime;
        m_messages.RemoveAll(message => message.ExpiresAt <= now);

        foreach (Guid effectId in m_activeEffects.Where(entry => entry.Value.ExpiresAt <= now).Select(pair => pair.Key))
        {
            AddMessage($"{GetEffectName(m_activeEffects[effectId].State)}: ended");
            m_activeEffects.TryRemove(effectId, out _);
        }
    }

    private void OnGUI()
    {
        if (!showOverlay)
            return;

        bool anythingVisible = false;
        if (m_activeEffects.Count == 0)
        {
            textActiveEffectsHeader.gameObject.SetActive(false);
            textActiveEffectsList.gameObject.SetActive(false);
        }
        else
        {
            anythingVisible = true;
            textActiveEffectsHeader.gameObject.SetActive(true);
            textActiveEffectsList.gameObject.SetActive(true);
            textActiveEffectsList.text = string.Join(Environment.NewLine, m_activeEffects.Values.Select(entry => GetEffectName(entry.State)).Distinct()); 
        }

        if (m_messages.Count == 0)
        {
            textRecentActivityHeader.gameObject.SetActive(false);
            textRecentActivityList.gameObject.SetActive(false);
        }
        else
        {
            anythingVisible = true;
            textRecentActivityHeader.gameObject.SetActive(true);
            textRecentActivityList.gameObject.SetActive(true);
            textRecentActivityList.text = string.Join(Environment.NewLine, m_messages.Select(message => message.Text));
        }

        textNoEffects.gameObject.SetActive(!anythingVisible);
    }

    private void AddMessage(string text)
    {
        m_messages.Add(new OverlayMessage
        {
            Text = text,
            ExpiresAt = Time.unscaledTime + messageLifetime
        });

        if (m_messages.Count > maxMessages)
            m_messages.RemoveRange(0, m_messages.Count - maxMessages);
    }

    private static string GetEffectName(EffectState state)
    {
        if (state.Effect is UnityEffectBase unityEffect && !string.IsNullOrWhiteSpace(unityEffect.Name))
            return unityEffect.Name;

        return state.Request.EffectID;
    }

    private static string DescribeStatus(EffectStatus status, bool isTimedEffect)
    {
        if (status.IsSuccess())
            return isTimedEffect ? "started" : "applied";
        if (status == EffectStatus.TimedResume)
            return "resumed";
        if (status == EffectStatus.TimedPause)
            return "paused";
        if (status == EffectStatus.TimedEnd)
            return "stopped";
        if (status.IsFailure() || status == EffectStatus.TimedCanceled || status == EffectStatus.TimedAborted)
            return "failed";

        return status.ToCamelCase();
    }
}