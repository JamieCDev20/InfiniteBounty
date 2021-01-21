using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeTool : WeaponTool
{
    [SerializeField] protected float f_angle;
    [SerializeField] protected float f_radius;
    [SerializeField] protected Transform t_conePoint;
    private const AugmentType augType = AugmentType.cone;

    private bool CheckInCone(Transform _t_objectToCheck, Vector3 _t_camPos)
    {
        if ((Mathf.Acos(Vector3.Dot(_t_camPos, (_t_objectToCheck.position - t_conePoint.position).normalized)) * Mathf.Rad2Deg) <= f_angle*0.5f)
            return true;
        return false;
    }

    protected virtual GameObject[] GetAllObjectsInCone(Vector3 _t_camPos)
    {
        //Ray r_rad = new Ray(t_conePoint.position, t_conePoint.up);
        //RaycastHit[] hitObjects = Physics.SphereCastAll(r_rad, f_radius);
        Collider[] hitObjects = Physics.OverlapSphere(t_conePoint.position, f_radius);
        List<GameObject> objInCone = new List<GameObject>();
        foreach (Collider hit in hitObjects)
            if (CheckInCone(hit.transform, _t_camPos))
                objInCone.Add(hit.transform.gameObject);
        return objInCone.ToArray();
    }
}
