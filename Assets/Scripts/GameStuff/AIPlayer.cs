using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{

    //Variables
    #region Serialised


    #endregion

    #region Private

    private bool b_holding = false;
    private GoapAgent agent;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        agent = GetComponent<GoapAgent>();
    }

    #endregion

    #region Private Voids

    public void Grab(GameObject _go_obj)
    {
        if (b_holding)
            return;
        _go_obj.transform.SetParent(transform);
        _go_obj.transform.localPosition = Vector3.zero;
        _go_obj.SendMessage("Held", gameObject);
        b_holding = true;
        agent.UpdateDomain("ObjectOfValue", true);
    }

    public void Drop()
    {
        b_holding = false;
        agent.UpdateDomain("ObjectOfValue", false);
    }

    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
