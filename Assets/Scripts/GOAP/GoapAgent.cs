using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapAgent : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] ActionSet actionSet;
    [SerializeField] GoalSet goalSet;

    #endregion

    #region Private


    #endregion

    //Methods
    #region Unity Standards


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

[System.Serializable]
public struct ActionSet
{
    [SerializeField]
    public ActionIntDictionary costedActionSet;
}

[System.Serializable]
public struct GoalSet
{
    [SerializeField]
    public GoalIntDictionary prioritisedGoalSet;
}