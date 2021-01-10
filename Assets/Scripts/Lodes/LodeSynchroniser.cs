using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LodeSynchroniser : MonoBehaviourPunCallbacks
{

    public static LodeSynchroniser x;

    //Variables
    #region Serialised

    private LodeBase[] allLodes;
    private List<string> toSends = new List<string>();

    #endregion

    #region Private


    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        x = this;
    }

    #endregion

    #region Private Voids


    #endregion

    #region Public Voids

    public void InitialiseLodeArrayLength(int length)
    {
        //Set how many lodes we are spawning
        allLodes = new LodeBase[length];
    }

    public void AddLode(LodeBase lode, int index)
    {
        allLodes[index] = lode;
        lode.SetIndex(index);
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
