using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NugManager : SubjectBase, ObserverBase
{
    private int i_totalNugs = 0;
    private int i_inLevelNugs = 0;
    private int i_playerID;
    private HUDController hud;
    private int[] iA_nugCount = new int[6];
    public int Nugs { get { return i_totalNugs; } }

    // Start is called before the first frame update
    void Start()
    {
        hud = gameObject.GetComponent<PlayerHealth>().hudControl;
        //SceneManager.sceneLoaded += OnSceneLoad;

        foreach (NugGO np in Resources.FindObjectsOfTypeAll<NugGO>())
        {
            NugGO ngo = np.GetComponent<NugGO>();
            if (ngo != null)
            {
                ngo.AddObserver(this);
            }
        }

        SaveManager _sm = FindObjectOfType<SaveManager>();
        if(_sm != null)
        {
            i_totalNugs = _sm.SaveData.i_totalNugs;
            //i_inLevelNugs = 
        }

#if UNITY_EDITOR
        //CollectNugs(5000, false);
        //Debug.LogError("GAINED 1000 BBs. REMOVE THIS BEFORE BUILDING");
#endif
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
            CollectNugs(500, false);
    }

    public void SetID(int _id)
    {
        i_playerID = _id;
    }

    public void EndedLevel()
    {
        i_playerID = GetComponent<PlayerInputManager>().GetID();

        //photonView.RPC("SetRemoteNugs", RpcTarget.All, i_inLevelNugs);
        SendNugs();
    }

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
                {
                    CollectNugs(ce.AmountToChange, true);
                    UniversalNugManager.x.RecieveNugs(ce.Nug);
                }
                else
                    CollectNugs(-ce.AmountToChange, true);
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

    public void CollectNugs(int _i_value, bool inLevel)
    {
        if (inLevel)
            i_inLevelNugs += _i_value;
        else
        {
            i_totalNugs += _i_value;
            HUDController.x?.SetBBTotal();
        }
        //PlayerSaveData psd = new PlayerSaveData(i_totalNugs, -1, -1, null, null, null, null, null, null, -1);
        //SaveEvent se = new SaveEvent(psd);
        //FindObjectOfType<SaveManager>().OnNotify(se);
    }


    public void SendNugs()
    {
        // Send Total nugs and nugs collected
        // Send nugs
        //ReceiveNugs(i_inLevelNugs);

    }

    public void SetRemoteNugs(int nugs)
    {
        //i_inLevelNugs += nugs;
        //CollectNugs(0);
        //SetPrefs();
        SendNugs();
    }

    public void ReceiveNugs(int _i_value)
    {
        i_totalNugs += _i_value;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.Contains("Lobby"))
            UniversalNugManager.x.Handshake(i_playerID);
    }
}
