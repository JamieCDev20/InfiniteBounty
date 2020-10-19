using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OOV : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private int i_pointVal = 1;

    #endregion

    #region Private

    private GameObject go_heldBy;

    #endregion

    //Methods
    #region Unity Standards

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ContactPlayer(other.gameObject);
            return;
        }
        if (other.CompareTag("Target"))
        {
            ContactTarget();
            return;
        }
    }

    private void OnDestroy()
    {
        go_heldBy.SendMessage("Drop");
    }

    #endregion

    #region Private Voids

    private void ContactPlayer(GameObject _go_player)
    {
        _go_player.SendMessage("Grab", gameObject);
    }

    private void ContactTarget()
    {
        Destroy(gameObject);
        go_heldBy.SendMessage("Drop");
        Points.x.AddPoints(i_pointVal);
        Spawner.x.Respawn();
    }

    private void Held(GameObject _go_heldBy)
    {
        go_heldBy = _go_heldBy;
    }

    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
