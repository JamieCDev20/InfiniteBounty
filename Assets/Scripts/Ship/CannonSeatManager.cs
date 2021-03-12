using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonSeatManager : MonoBehaviourPun
{

    public static CannonSeatManager x;

    private int sittingCount;

    private void Awake()
    {
        x = this;
    }

    public void StartedSitting()
    {
        photonView.RPC(nameof(RemoteSit), RpcTarget.All);
    }

    [PunRPC]
    public void RemoteSit()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        sittingCount++;
        if (sittingCount >= PhotonNetwork.CurrentRoom.PlayerCount)
            LoadingScreenManager.x.CallLoadLevel(ModeSelect.x.GetModeName());

    }

    [PunRPC]
    public void RemoteStopSitting()
    {
        if (PhotonNetwork.IsMasterClient)
            sittingCount--;
    }

    public void EndedSitting()
    {
        photonView.RPC(nameof(RemoteStopSitting), RpcTarget.All);
    }

}
