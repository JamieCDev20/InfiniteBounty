using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AITargetting
{

    //Variables
    #region Serialised

    [SerializeField] private float f_spottingDistance = 25;

    #endregion

    #region Private

    private Transform transform;

    #endregion

    //Methods
    #region Unity Standards


    #endregion

    #region Private Voids

    
    #endregion

    #region Public Voids

    public void SetTransform(Transform t)
    {
        transform = t;
    }

    public Transform GetTarget()
    {
        RaycastHit hit;
        Vector3 origin, target;
        int i = 0;
        //foreach (GameObject g in TagManager.x.GetTagSet("Player"))
        foreach (PlayerInputManager g in GameObject.FindObjectsOfType<PlayerInputManager>())
        {
            origin = transform.position + Vector3.up * 0.5f;
            target = g.transform.position + Vector3.up * 0.5f;
            Physics.Raycast(origin, target - origin, out hit, f_spottingDistance, LayerMask.GetMask("Player"));
            if (hit.collider != null)
                return g.transform;
        }

        return null;
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
