using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AITargetting
{

    //Variables
    #region Serialised

    [SerializeField] private float f_spottingDistance = 25;
    [SerializeField] private LayerMask spottingMask;

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
        //foreach (GameObject g in TagManager.x.GetTagSet("Player"))
        List<Transform> targets = new List<Transform>();
        foreach (PlayerInputManager p in GameObject.FindObjectsOfType<PlayerInputManager>())
        {
            origin = transform.position;
            target = p.transform.position;
            Debug.DrawRay(origin, target - origin * 2, Color.red);
            Physics.Raycast(origin, target - origin, out hit, f_spottingDistance, spottingMask);
            if (hit.collider != null)
                targets.Add(p.transform);
        }
        if (targets.Count > 0)
            return targets[Random.Range(0, targets.Count)];

        return null;
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
