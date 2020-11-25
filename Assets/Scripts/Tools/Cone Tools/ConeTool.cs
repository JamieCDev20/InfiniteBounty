using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeTool : WeaponTool
{
    [SerializeField] protected float f_angle;
    [SerializeField] protected float f_radius;
    [SerializeField] protected Transform t_conePoint;

    private bool CheckInCone(Transform _t_objectToCheck)
    {
        if ((Mathf.Acos(Vector3.Dot(t_conePoint.position, _t_objectToCheck.position)) * Mathf.Rad2Deg) <= f_angle)
            return true;
        return false;
    }

    protected virtual GameObject[] GetAllObjectsInCone()
    {
        Ray r_rad = new Ray();
        RaycastHit[] hitObjects = Physics.SphereCastAll(r_rad, f_radius);
        List<GameObject> objInCone = new List<GameObject>();
        foreach (RaycastHit hit in hitObjects)
            if (CheckInCone(hit.transform))
                objInCone.Add(hit.transform.gameObject);
        return objInCone.ToArray();
    }
}
