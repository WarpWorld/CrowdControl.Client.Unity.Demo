using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;

public class ChangeColor : UnityEffectBase
{
    private Material m_sphereMaterial;

    protected override void Awake()
    {
        m_sphereMaterial = FindAnyObjectByType<SphereBehavior>().GetComponent<MeshRenderer>().material; //get the material from the first active ball
        base.Awake();
    }

    public override EffectStatus StartEffect(EffectRequest request)
    {
        if (SphereBehavior.InstanceCount == 0)
            return EffectStatus.FailTemporary;

        m_sphereMaterial.color = request.Parameters["changecolor_options"].Value switch
        {
            "changecolor_red" => Color.red,
            "changecolor_green" => Color.green,
            "changecolor_blue" => Color.blue,
            _ => Color.white
        };

        return EffectStatus.Success;
    }
}