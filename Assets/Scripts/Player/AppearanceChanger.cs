using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearanceChanger : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerInputManager pim_pim;

    [Header("Head Things")]
    [SerializeField] private GameObject[] goA_heads = new GameObject[0];
    private int i_currentHead;

    [Header("Body Things")]
    [SerializeField] private GameObject[] goA_bodies = new GameObject[0];
    private int i_currentBody;

    [Header("Arm Things")]
    [SerializeField] private GameObjectList[] golA_arms = new GameObjectList[0];
    private int i_currentArm;
    private bool b_showLeftArm = true;
    private bool b_showRightArm = true;

    [Header("Feet Things")]
    [SerializeField] private GameObject[] goA_feet = new GameObject[0];
    private int i_currentFeet;

    [SerializeField] private bool b_networked = true;
    public int Head { get { return i_currentHead; } }
    public int Body { get { return i_currentBody; } }
    public int Arms { get { return i_currentArm; } }
    public int Feet { get { return i_currentFeet; } }



    private void Start()
    {
        if (!photonView.IsMine) return;

        SaveManager _sm = FindObjectOfType<SaveManager>();

        if (_sm.SaveData.A_appearance != null && _sm.SaveData.A_appearance.Length == 4)
        {
            i_currentBody = _sm.SaveData.A_appearance[0] != -1 ? _sm.SaveData.A_appearance[0] : Random.Range(0, goA_bodies.Length);
            i_currentHead = _sm.SaveData.A_appearance[1] != -1 ? _sm.SaveData.A_appearance[1] : Random.Range(0, goA_heads.Length); ;
            i_currentArm = _sm.SaveData.A_appearance[2] != -1 ? _sm.SaveData.A_appearance[2] : Random.Range(0, golA_arms.Length);
            i_currentFeet = _sm.SaveData.A_appearance[3] != -1 ? _sm.SaveData.A_appearance[3] : Random.Range(0, goA_feet.Length);
        }
        else
        {
            i_currentBody = Random.Range(0, goA_bodies.Length);
            i_currentHead = Random.Range(0, goA_heads.Length);
            i_currentArm = Random.Range(0, golA_arms.Length);
            i_currentFeet = Random.Range(0, goA_feet.Length);
        }

        goA_heads[i_currentHead].SetActive(true);
        goA_bodies[i_currentBody].SetActive(true);
        golA_arms[i_currentArm].goL_theList[0].SetActive(b_showLeftArm);
        golA_arms[i_currentArm].goL_theList[1].SetActive(b_showRightArm);
        goA_feet[i_currentFeet].SetActive(true);

        photonView.RPC("UpdateHeadInOthers", RpcTarget.Others, i_currentHead);
        photonView.RPC("UpdateBodyInOthers", RpcTarget.Others, i_currentBody);
        photonView.RPC("UpdateArmsInOthers", RpcTarget.Others, i_currentArm);
        photonView.RPC("UpdateFeetInOthers", RpcTarget.Others, i_currentFeet);

        HUDController.x.ChangeHeadSpriteIcon(i_currentHead);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC("UpdateHeadInOthers", RpcTarget.Others, i_currentHead);
        photonView.RPC("UpdateBodyInOthers", RpcTarget.Others, i_currentBody);
        photonView.RPC("UpdateArmsInOthers", RpcTarget.Others, i_currentArm);
        photonView.RPC("UpdateFeetInOthers", RpcTarget.Others, i_currentFeet);
    }

    #region Head things

    public void NextHead()
    {
        goA_heads[i_currentHead].SetActive(false);

        i_currentHead++;
        if (i_currentHead == goA_heads.Length)
            i_currentHead = 0;

        goA_heads[i_currentHead].SetActive(true);

        if (b_networked)
            photonView.RPC("UpdateHeadInOthers", RpcTarget.Others, i_currentHead);
        HUDController.x.ChangeHeadSpriteIcon(i_currentHead);
    }
    public void LastHead()
    {
        goA_heads[i_currentHead].SetActive(false);

        i_currentHead--;
        if (i_currentHead < 0)
            i_currentHead = goA_heads.Length - 1;

        goA_heads[i_currentHead].SetActive(true);

        if (b_networked)
            photonView.RPC("UpdateHeadInOthers", RpcTarget.Others, i_currentHead);
        HUDController.x.ChangeHeadSpriteIcon(i_currentHead);
    }

    [PunRPC]
    public void UpdateHeadInOthers(int _i_headIndex)
    {
        goA_heads[i_currentHead].SetActive(false);
        i_currentHead = _i_headIndex;
        goA_heads[i_currentHead].SetActive(true);
    }

    public int ReturnHeadIndex()
    {
        return i_currentHead;
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

        if (b_networked)
            photonView.RPC("UpdateBodyInOthers", RpcTarget.Others, i_currentBody);
    }
    public void LastBody()
    {
        goA_bodies[i_currentBody].SetActive(false);

        i_currentBody--;
        if (i_currentBody < 0)
            i_currentBody = goA_bodies.Length - 1;

        goA_bodies[i_currentBody].SetActive(true);

        if (b_networked)
            photonView.RPC("UpdateBodyInOthers", RpcTarget.Others, i_currentBody);
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

        golA_arms[i_currentArm].goL_theList[0].SetActive(b_showLeftArm);
        golA_arms[i_currentArm].goL_theList[1].SetActive(b_showRightArm);

        if (b_networked)
            photonView.RPC("UpdateArmsInOthers", RpcTarget.Others, i_currentArm);
    }
    public void LastArms()
    {
        golA_arms[i_currentArm].goL_theList[0].SetActive(false);
        golA_arms[i_currentArm].goL_theList[1].SetActive(false);

        i_currentArm--;
        if (i_currentArm < 0)
            i_currentArm = golA_arms.Length - 1;

        golA_arms[i_currentArm].goL_theList[0].SetActive(b_showLeftArm);
        golA_arms[i_currentArm].goL_theList[1].SetActive(b_showRightArm);

        if (b_networked)
            photonView.RPC("UpdateArmsInOthers", RpcTarget.Others, i_currentArm);
    }

    [PunRPC]
    public void UpdateArmsInOthers(int _i_armIndex)
    {
        golA_arms[i_currentArm].goL_theList[0].SetActive(false);
        golA_arms[i_currentArm].goL_theList[1].SetActive(false);
        i_currentArm = _i_armIndex;
        golA_arms[i_currentArm].goL_theList[0].SetActive(b_showLeftArm);
        golA_arms[i_currentArm].goL_theList[1].SetActive(b_showRightArm);
    }

    public void SetArmActive(int _i_armIndex, bool _b_active)
    {
        if (_i_armIndex == 0)
            b_showLeftArm = _b_active;
        if (_i_armIndex == 1)
            b_showRightArm = _b_active;

        if (_i_armIndex < 2)
            for (int i = 0; i < golA_arms.Length; i++)
            {
                golA_arms[i].goL_theList[0].SetActive(b_showLeftArm);
                golA_arms[i].goL_theList[1].SetActive(b_showRightArm);
            }
    }

    public void SetCurrentArmActive(int _i_armIndex, bool _b_active)
    {
        if (_i_armIndex == 0)
            b_showLeftArm = _b_active;
        else if (_i_armIndex == 1)
            b_showRightArm = _b_active;
        else
            return;
        golA_arms[i_currentArm].goL_theList[_i_armIndex].SetActive(_b_active);
        if (b_networked)
            photonView.RPC("UpdateArmsInOthers", RpcTarget.Others, i_currentArm);
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

        if (b_networked)
            photonView.RPC("UpdateFeetInOthers", RpcTarget.Others, i_currentFeet);
    }
    public void LastFeet()
    {
        goA_feet[i_currentFeet].SetActive(false);

        i_currentFeet--;
        if (i_currentFeet < 0)
            i_currentFeet = goA_feet.Length - 1;

        goA_feet[i_currentFeet].SetActive(true);

        if (b_networked)
            photonView.RPC("UpdateFeetInOthers", RpcTarget.Others, i_currentFeet);
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
