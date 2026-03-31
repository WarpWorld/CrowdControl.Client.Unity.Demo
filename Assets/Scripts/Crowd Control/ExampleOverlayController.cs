using CrowdControl.Client.Unity;
using CrowdControl.Common;
using System.Collections.Concurrent;
using UnityEngine;

public class ExampleOverlayController : MonoBehaviour
{
    private ConcurrentDictionary<string, EffectRequest> m_queuedEffects = new();

    private ConcurrentDictionary<string, EffectRequest> m_activeEffects = new();

    private void Update()
    {
        //manage your overlay state here based on m_queuedEffects and m_activeEffects
    }

    public void OnEffectRequest(EffectRequest request)
    {
        Debug.Log($"Effect Requested with ID: {request.EffectID}");
        //m_queuedEffects[request.ID] = request;
    }

    public void OnEffectUpdate(EffectState state)
    {
        Debug.Log($"Effect Updated with ID: {state.Request.EffectID}, State: {state.Response.Status}");
        //m_activeEffects[state.Request.ID] = state;
    }
}