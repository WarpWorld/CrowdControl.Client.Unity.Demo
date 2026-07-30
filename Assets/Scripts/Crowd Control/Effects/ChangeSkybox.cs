using CrowdControl.Client.Unity;
using CrowdControl.Common;
using System;
using Unity.VisualScripting;
using UnityEngine;
using EffectResponse = CrowdControl.Client.WebSocket.EffectResponse;

public class ChangeSkybox : UnityEffectBase
{
    private const string NEXT_SKYBOX_METADATA = "nextSkyboxName";

    [InspectorName("Skybox Options")]
    public ChangeSkyboxOptions[] Options;

    private int m_currentOptionIndex = 0;

    [Serializable]
    public class ChangeSkyboxOptions
    {
        public string Name;
        public Material Material;
    }

    public override void Initialize()
    {
        base.Initialize();
        RenderSettings.skybox = GetNextMaterial();
    }

    public Material GetNextMaterial()
    {
        if (Options.Length == 0) return null;

        Material currentMaterial = Options[m_currentOptionIndex].Material;
        m_currentOptionIndex = (m_currentOptionIndex + 1) % Options.Length;
        string nextName = Options[m_currentOptionIndex].Name;

        if (CrowdControlBehavior.TryGetMetadataObject(NEXT_SKYBOX_METADATA, out NextSkyboxName nextSkyboxName))
            nextSkyboxName.UpdateValue(nextName);

        return currentMaterial;
    }

    public override EffectResponse StartEffect(EffectRequest request)
    {
        RenderSettings.skybox = GetNextMaterial();
        return EffectStatus.Success;
    }
}