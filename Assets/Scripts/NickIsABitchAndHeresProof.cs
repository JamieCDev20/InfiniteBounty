using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NickIsABitchAndHeresProof : MonoBehaviour
{

    //Variables
    #region Serialised


    #endregion

    #region Private

    private Rigidbody TheLittleRbThatCould;
    private Vector3 NickCanSugMySchlub = Vector3.right;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        TheLittleRbThatCould = GetComponent<Rigidbody>();
        TheLittleRbThatCould.constraints = RigidbodyConstraints.FreezeRotation;
    }

    #endregion

    #region Private Voids

    private void FixedUpdate()
    {
        TheLittleRbThatCould.MovePosition(NickCanSugMySchlub * 10 * Mathf.Sin(Time.realtimeSinceStartup) + Vector3.up);
    }

    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
