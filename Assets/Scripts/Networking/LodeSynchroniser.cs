using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LodeSynchroniser : MonoBehaviourPunCallbacks, IPunObservable
{

    public static LodeSynchroniser x;

    //Variables
    #region Serialised

    private LodeBase[] allLodes;
    private List<string> toSends;

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


    #endregion

    #region Public Voids

    public void InitialiseLodeArrayLength(int length)
    {
        allLodes = new LodeBase[length];
    }

    public void AddLode(LodeBase lode, int index)
    {
        allLodes[index] = lode;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            if(toSends.Count > 0)
            {
                stream.SendNext(toSends[0]);
                toSends.RemoveAt(0);
            }
        }
        else
        {
            if(stream.Count > 0)
            {
                string[] read = ((string)stream.ReceiveNext()).Split(',');
                int index = int.Parse(read[0]);
                int health = int.Parse(read[1]);

                allLodes[index].SetHealth(health);

            }
        }

    }

    public void SyncHealth(int health, int index)
    {
        toSends.Add(string.Format("{0},{1}", index, health));
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
