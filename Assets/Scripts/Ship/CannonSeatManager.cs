using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonSeatManager : MonoBehaviour
{

    public static CannonSeatManager x;

    private int sittingCount;

    private void Awake()
    {
        x = this;
    }

    public void StartedSitting()
    {
        sittingCount++;
        if (sittingCount >= PhotonNetwork.CurrentRoom.PlayerCount)
            LoadingScreenManager.x.CallLoadLevel(ModeSelect.x.GetModeName());
    }

    public void EndedSitting()
    {
        sittingCount--;
    }

}
