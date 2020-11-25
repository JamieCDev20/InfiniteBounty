using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugSucker : ConeTool
{
    public override void Use(Vector3 _v_forwards)
    {
        foreach(GameObject hit in GetAllObjectsInCone())
        {
            //hit.GetComponent<IPullable>()?.
            Debug.Log(hit.name);
        }
    }
}
