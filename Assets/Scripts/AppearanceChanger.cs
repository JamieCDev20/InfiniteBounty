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

    [Header("Arm Things")]
    [SerializeField] private GameObjectList[] golA_arms = new GameObjectList[0];
    private int i_currentArm;

    [Header("Feet Things")]
    [SerializeField] private GameObject[] goA_feet = new GameObject[0];
    private int i_currentFeet;


    private void Start()
    {
        view = GetComponent<PhotonView>();

        i_currentBody = Random.Range(0, goA_bodies.Length);
        i_currentHead = Random.Range(0, goA_heads.Length);
        i_currentArm = Random.Range(0, golA_arms.Length);
        i_currentFeet = Random.Range(0, goA_feet.Length);
        NextHead();
        NextBody();
        NextArms();
        NextFeet();
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

    #region Arm things

    public void NextArms()
    {
        golA_arms[i_currentArm].goL_theList[0].SetActive(false);
        golA_arms[i_currentArm].goL_theList[1].SetActive(false);

        i_currentArm++;
        if (i_currentArm == golA_arms.Length)
            i_currentArm = 0;

        golA_arms[i_currentArm].goL_theList[0].SetActive(true);
        golA_arms[i_currentArm].goL_theList[1].SetActive(true);

        view.RPC("UpdateArmsInOthers", RpcTarget.Others, i_currentArm);
    }
    public void LastArms()
    {
        golA_arms[i_currentArm].goL_theList[0].SetActive(false);
        golA_arms[i_currentArm].goL_theList[1].SetActive(false);

        i_currentArm--;
        if (i_currentArm < 0)
            i_currentArm = golA_arms.Length - 1;

        golA_arms[i_currentArm].goL_theList[0].SetActive(true);
        golA_arms[i_currentArm].goL_theList[1].SetActive(true);

        view.RPC("UpdateArmsInOthers", RpcTarget.Others, i_currentArm);        
    }

    [PunRPC]
    public void UpdateArmsInOthers(int _i_armIndex)
    {
        golA_arms[i_currentArm].goL_theList[0].SetActive(false);
        golA_arms[i_currentArm].goL_theList[1].SetActive(false);
        i_currentArm = _i_armIndex;
        golA_arms[i_currentArm].goL_theList[0].SetActive(true);
        golA_arms[i_currentArm].goL_theList[1].SetActive(true);
    }

    public void SetArmActive(int _i_armIndex, bool _b_active)
    {
        if (_i_armIndex < 2)
            for (int i = 0; i < golA_arms.Length; i++)
                golA_arms[i].goL_theList[_i_armIndex].SetActive(_b_active);
    }

    #endregion

    #region Feet things

    public void NextFeet()
    {
        goA_feet[i_currentFeet].SetActive(false);

        i_currentFeet++;
        if (i_currentFeet == goA_feet.Length)
            i_currentFeet = 0;

        goA_feet[i_currentFeet].SetActive(true);

        view.RPC("UpdateFeetInOthers", RpcTarget.Others, i_currentFeet);
    }
    public void LastFeet()
    {
        goA_feet[i_currentFeet].SetActive(false);

        i_currentFeet--;
        if (i_currentFeet < 0)
            i_currentFeet = goA_feet.Length - 1;

        goA_feet[i_currentFeet].SetActive(true);

        view.RPC("UpdateFeetInOthers", RpcTarget.Others, i_currentFeet);
    }

    [PunRPC]
    public void UpdateFeetInOthers(int _i_feetIndex)
    {
        goA_feet[i_currentFeet].SetActive(false);
        i_currentFeet = _i_feetIndex;
        goA_feet[i_currentFeet].SetActive(true);
    }

    #endregion

}

[System.Serializable]
public struct GameObjectList
{
    public List<GameObject> goL_theList;
}
