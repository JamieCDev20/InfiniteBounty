using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Domain : MonoBehaviour
{

    public static Domain x;

    //Variables
    #region Serialised

    public Dictionary<string, bool> conditions = new Dictionary<string, bool>() { { "hasItem", false }, { "pointsIncrease", false } };

    #endregion

    #region Private


    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        x = this;
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
