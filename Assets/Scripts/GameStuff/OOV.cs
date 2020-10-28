using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OOV : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private int i_pointVal = 1;
    [SerializeField] private Spawner[] spawns;

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
            if (go_heldBy != null)
                go_heldBy.GetComponent<AIPlayer>().Drop();
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
        if(go_heldBy != null)
        {
            go_heldBy.GetComponent<AIPlayer>().Drop();

        }
    }

    #endregion

    #region Private Voids

    private void ContactPlayer(GameObject _go_player)
    {
        TagManager.x.RemoveTaggedObject(GetComponent<TagableObject>());
        _go_player.GetComponent<AIPlayer>().Grab(gameObject);
    }

    private void ContactTarget()
    {
        Destroy(gameObject);
        go_heldBy.GetComponent<AIPlayer>().Drop();
        Points.x.AddPoints(i_pointVal);
        foreach (Spawner i in FindObjectsOfType<Spawner>())
        {
            i.Respawn();
        }
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
