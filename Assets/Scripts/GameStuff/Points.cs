using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{

    public static Points x;

    //Variables
    #region Serialised


    #endregion

    #region Private
    
    [SerializeField]
    private int i_points;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        x = this;
    }

    #endregion

    #region Private Voids


    #endregion

    #region Public Voids

    public void AddPoints(int _i_val)
    {
        i_points += _i_val;
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns

    public int GetPoints()
    {
        return i_points;
    }

    #endregion

}
