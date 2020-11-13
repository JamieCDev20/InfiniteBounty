using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointless : MonoBehaviour
{

    //Variables
    #region Serialised


    #endregion

    #region Private


    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        name = name.Replace("Spawn (", "");
        name = name.Replace(")", "");

        transform.position = Vector3.right * 2 * int.Parse(name);

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
