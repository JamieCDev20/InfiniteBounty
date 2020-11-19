using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTargetter : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private NGoapAgent agent;
    [SerializeField] private Transform target;

    public bool transformFollow;

    #endregion

    #region Private


    #endregion

    //Methods
    #region Unity Standards

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);
            hit.collider.GetComponent<IHitable>()?.TakeDamage(1);
        }
    }

    private void FixedUpdate()
    {
        if (transformFollow)
        {
            agent.SetTarget(target);
        }
        else
        {
            agent.SetTarget(target.position);
        }
    }

    #endregion

    #region Private Voids


    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
