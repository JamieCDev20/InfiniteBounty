using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugSucker : ConeTool
{
    [SerializeField] private float f_suckForce;
    public override void Use(Vector3 _v_forwards)
    {
        base.Use(_v_forwards);

        ParticleSystem.ShapeModule sm = go_particles.GetComponentInChildren<ParticleSystem>().shape;
        sm.angle = f_angle * 0.5f;
        PlayAudio(ac_activationSound);
        PlayParticles(true);
        foreach (GameObject hit in GetAllObjectsInCone(t_conePoint.forward))
        {
            ISuckable suck = hit.GetComponent<ISuckable>();
            if(suck != null)
                suck.GetRigidbody().velocity = (t_conePoint.position - hit.transform.position).normalized * f_suckForce;
            //(hit.GetComponent<ISuckable>()?.GetRigidbody()).velocity = (t_conePoint.position - hit.transform.position).normalized * f_suckForce;//.AddForce((t_conePoint.position - hit.transform.position).normalized * f_suckForce, ForceMode.VelocityChange);
        }
    }
}
