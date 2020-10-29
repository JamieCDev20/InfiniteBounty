using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public static Spawner x;

    //Variables
    #region Serialised

    [SerializeField] private GameObject obj;

    #endregion

    #region Private


    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        x = this;
    }

    #endregion

    #region Private Voids

    public void Respawn()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2);
        Instantiate(obj, transform.position + Vector3.up, Quaternion.identity);
    }

    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
