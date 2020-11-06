using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    private int i_playerID;
    private Vector3 v_spawnPoint;
    [SerializeField]
    private PlayerInfo playerInfo;
    public int PlayerID{ get{ return i_playerID; } set { i_playerID = value; } }

    private List<string> dataToSend = new List<string>();

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        for (int i = 0; i < dataToSend.Count; i++)
        {
            stream.SendNext(dataToSend[i]);
        }

    }

    private void FixedUpdate()
    {
        PrepareSendData(NetworkDataType.pos, transform.position.ToString());
        PrepareSendData(NetworkDataType.rot, transform.rotation.ToString());
    }

    public void PrepareSendData(NetworkDataType type, string data)
    {
        dataToSend.Add(string.Format("{1}{0}{2}{0}{3}", NetworkManager.separator, i_playerID, (int)type, data));
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("You Joined");
        PhotonNetwork.Instantiate("NetworkPrefabs/"+playerInfo.go_playerPrefab.name, v_spawnPoint, Quaternion.identity);
        Debug.Log("Spawned Player Prefab");
    }

}

public enum NetworkDataType
{
    pos,
    rot,
    anim,
    boole,
    skin

}