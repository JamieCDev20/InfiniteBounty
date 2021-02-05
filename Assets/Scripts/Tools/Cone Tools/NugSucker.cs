using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugSucker : ConeTool
{
    [SerializeField] private float f_suckForce;
    [SerializeField] private GameObject go_cone;
    private ParticleSystem ps_cone;

    public void Start()
    {
        ps_cone = go_cone.GetComponentInChildren<ParticleSystem>();
    }
    public override void Use(Vector3 _v_forwards)
    {
        base.Use(_v_forwards);
        if (go_cone != null)
        {
            ParticleSystem.ShapeModule sm = ps_cone.shape;
            sm.angle = f_angle * 0.5f;
            PlayParticles(true);
        }
        PlayAudio(ac_activationSound);
        foreach (GameObject hit in GetAllObjectsInCone(t_conePoint.forward))
        {
            ISuckable suck = hit.GetComponent<ISuckable>();
            if (suck != null)
                suck.GetRigidbody().velocity = (t_conePoint.position - hit.transform.position).normalized * f_suckForce;
            //(hit.GetComponent<ISuckable>()?.GetRigidbody()).velocity = (t_conePoint.position - hit.transform.position).normalized * f_suckForce;//.AddForce((t_conePoint.position - hit.transform.position).normalized * f_suckForce, ForceMode.VelocityChange);
        }
    }
}
