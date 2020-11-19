using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGoapAgent : MonoBehaviour
{

    //Variables
    #region Serialised
    [Header("Movement")]
    [SerializeField] private AIMovementType movementType;
    [SerializeField] private AIMovementStats movementStats;

    #endregion

    #region Private

    private AIGroundMover mover;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        mover.Move();
    }

    #endregion

    #region Private Voids

    private void Init()
    {
        switch (movementType)
        {
            case AIMovementType.ground:
                mover = new AIGroundMover(movementStats);
                mover.SetRB(GetComponent<Rigidbody>());
                break;
            case AIMovementType.air:
                break;
            default:
                break;
        }
    }

    #endregion

    #region Public Voids

    public void SetTarget(Transform target)
    {
        mover.Retarget(target, true);
    }

    public void SetTarget(Vector3 target)
    {
        mover.Retarget(target, true);
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}

public enum AIMovementType
{
    ground,
    air
}

[System.Serializable]
public struct AIMovementStats
{
    public float f_movementSpeed;
    public float f_jumpHeight;
    public Vector3 v_drag;

}