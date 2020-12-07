using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearanceChanger : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerInputManager pim_pim;
    private PhotonView view;


    [Header("Head Things")]
    [SerializeField] private GameObject[] goA_heads = new GameObject[0];
    private int i_currentHead;

    [Header("Body Things")]
    [SerializeField] private GameObject[] goA_bodies = new GameObject[0];
    private int i_currentBody;




    private void Start()
    {
        view = GetComponent<PhotonView>();

        i_currentBody = Random.Range(0, goA_bodies.Length);
        i_currentHead = Random.Range(0, goA_heads.Length);
        NextHead();
        NextBody();
    }

    #region Head things

    public void NextHead()
    {
        goA_heads[i_currentHead].SetActive(false);

        i_currentHead++;
        if (i_currentHead == goA_heads.Length)
            i_currentHead = 0;

        goA_heads[i_currentHead].SetActive(true);

        view.RPC("UpdateHeadInOthers", RpcTarget.Others, i_currentHead);
    }
    public void LastHead()
    {
        goA_heads[i_currentHead].SetActive(false);

        i_currentHead--;
        if (i_currentHead < 0)
            i_currentHead = goA_heads.Length - 1;

        goA_heads[i_currentHead].SetActive(true);

        view.RPC("UpdateHeadInOthers", RpcTarget.Others, i_currentHead);
    }

    [PunRPC]
    public void UpdateHeadInOthers(int _i_headIndex)
    {
        goA_heads[i_currentHead].SetActive(false);
        i_currentHead = _i_headIndex;
        goA_heads[i_currentHead].SetActive(true);
    }

    #endregion

    #region Body Things

    public void NextBody()
    {
        goA_bodies[i_currentBody].SetActive(false);

        i_currentBody++;
        if (i_currentBody == goA_bodies.Length)
            i_currentBody = 0;

        goA_bodies[i_currentBody].SetActive(true);

        view.RPC("UpdateBodyInOthers", RpcTarget.Others, i_currentBody);
    }
    public void LastBody()
    {
        goA_bodies[i_currentBody].SetActive(false);

        i_currentBody--;
        if (i_currentBody < 0)
            i_currentBody = goA_bodies.Length - 1;

        goA_bodies[i_currentBody].SetActive(true);

        view.RPC("UpdateBodyInOthers", RpcTarget.Others, i_currentBody);
    }


    [PunRPC]
    public void UpdateBodyInOthers(int _i_headIndex)
    {
        goA_bodies[i_currentBody].SetActive(false);
        i_currentBody = _i_headIndex;
        goA_bodies[i_currentBody].SetActive(true);
    }

    #endregion

}