using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NugManager : SubjectBase, ObserverBase
{
    private int i_totalNugs = 0;
    /// <summary>
    /// The amount of nugs collected since last network update
    /// </summary>
    private int i_inLevelNugs = 0;
    private int i_playerID;
    private Text t_nugText;
    // Start is called before the first frame update
    void Start()
    {

        //SceneManager.sceneLoaded += OnSceneLoad;

        foreach (NugGO np in Resources.FindObjectsOfTypeAll<NugGO>())
        {
            NugGO ngo = np.GetComponent<NugGO>();
            if (ngo != null)
            {
                ngo.AddObserver(this);
            }
        }
    }


    public void EndedLevel()
    {
        i_playerID = GetComponent<PlayerInputManager>().GetID();

        photonView.RPC("Bridge", RpcTarget.All);
        //SendNugs();
    }

    [PunRPC]
    public void Bridge()
    {
        photonView.RPC("SetRemoteNugs", RpcTarget.All, i_inLevelNugs);

    }

    public void OnNotify(ObserverEvent oe_event)
    {
        switch (oe_event)
        {
            case CurrencyEvent ce:
                if (ce.AddOrSubtract)
                    CollectNugs(ce.AmountToChange);
                else
                    CollectNugs(-ce.AmountToChange);
                break;
            case PoolLoadEvent ple:
                foreach (NugGO np in FindObjectsOfType<NugGO>())
                {
                    np.AddObserver(this);
                }
                break;
        }
    }
    public void Init()
    {

    }
    public void CollectNugs(int _i_value)
    {
        if (t_nugText == null)
            return;
        i_totalNugs += _i_value;
        i_inLevelNugs += _i_value;
        t_nugText.text = i_totalNugs.ToString();
    }
    public void SendNugs()
    {
        // Send Total nugs and nugs collected

        // Send nugs
        ReceiveNugs(i_inLevelNugs);

    }

    [PunRPC]
    public void SetRemoteNugs(int nugs)
    {
        i_inLevelNugs += nugs;
        CollectNugs(0);
        photonView.RPC("SetPrefs", RpcTarget.All);
    }

    [PunRPC]
    public void SetPrefs()
    {
        PlayerPrefs.SetInt($"{i_playerID}NugCount", i_inLevelNugs);
    }

    public void ReceiveNugs(int _i_value)
    {
        i_totalNugs += _i_value;
    }

    public void SetNugTextRef(Text _txt_)
    {
        t_nugText = _txt_;
    }

}
