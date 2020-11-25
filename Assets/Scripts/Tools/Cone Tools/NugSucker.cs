using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugSucker : ConeTool
{
    [SerializeField] private float f_suckForce;
    public override void Use(Vector3 _v_forwards)
    {
        PlayParticles(true);
        foreach(GameObject hit in GetAllObjectsInCone(_v_forwards))
        {
            hit.GetComponent<ISuckable>()?.GetRigidbody().AddForce((t_conePoint.position - hit.transform.position).normalized * f_suckForce, ForceMode.Impulse);
        }
    }
}
