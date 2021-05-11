﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Diversifier
{
    None, //It implements itself

    JumboLodes, //Triggered in LodeSpawnZone
    LotsOLodes, //Triggered in LodeSpawnZone, causes the maximum number of lodes to spawn
    GeysersGalore, //Triggered in DiversiferManager
    MiniMiniboss, //Triggered in HandymanHealth
    MiniBunny, //Triggered in HopDogHealth
    GoofyGroobers, //Triggered in GooberHealth
    MiniFlying, //Triggered in FlyingHealth

    BabyLodes, //Triggered in LodeSpawnZone
    LessLodes, //Triggered in LodeSpawnZone, causes the minimum number of lodes to spawn
    LethalLava, //Triggered in KillBox
    ZeroGNuggs, //Triggered in NugGo in start
    MaxiBoss, //Triggered in HandymanHealth
    MegaBunnies, //Triggered in HopDogHealth
    NastyGroobers, //Triggered in GrooberHealth
    MaxiFlying, //Triggered in FlyingHealth
}

public enum BonusObjective
{
    None, BonusGoo, BonusHydro, BonusTasty, BonusThunder, BonusBoom, BonusMagma
}

public class DiversifierManager : MonoBehaviourPunCallbacks
{
    public static DiversifierManager x;
    private PhotonView view;
    private Diversifier[] dA_activeDivers = new Diversifier[3];
    private LodeSpawnZone[] ziA_allZones;
    private BonusObjective bo_currentBonusObjective;

    [Header("Diver Display Info")]
    [SerializeField] internal DiversifierInfo[] diA_diversifiers = new DiversifierInfo[0];

    [Header("Geyser Things")]
    [SerializeField] private string s_geyserPath;
    [SerializeField] private Vector2 v_numberOfGeysers;

    [Header("Maxi/Mini Enemies")]
    public float EnemyGrow = 1.5f;
    public float EnemyShrink = 0.5f;

    [Header("Lode Divers")]
    [SerializeField] private float f_jumboLodes = 1.5f;
    [SerializeField] private float f_babyLodes = 0.5f;
    [Space]
    [SerializeField] private float f_lessLodes = 0.5f;
    [SerializeField] private float f_lotsoLodes = 1.5f;


    public void Init()
    {
        transform.parent = null;

        if (x != null)
        {
            x.bo_currentBonusObjective = BonusObjective.None;
            Destroy(gameObject);
        }
        else
        {
            x = this;
            DontDestroyOnLoad(gameObject);

            view = GetComponent<PhotonView>();
            view.ViewID = 84520;
            PhotonNetwork.RegisterPhotonView(view);
        }
    }

    public void ReceiveDiversifiers(Diversifier[] _dA_diversGotten)
    {
        dA_activeDivers[0] = _dA_diversGotten[0];
        dA_activeDivers[1] = _dA_diversGotten[1];
        dA_activeDivers[2] = _dA_diversGotten[2];

    }

    internal BonusObjective ReturnBonusObjective()
    {
        return bo_currentBonusObjective;
    }

    [PunRPC]
    public void SyncBonusObjective(int _i_objectiveIndex)
    {
        bo_currentBonusObjective = (BonusObjective)_i_objectiveIndex;
        HUDController.x.ChangeBonusObjective(bo_currentBonusObjective);
    }


    public bool ReturnIfDiverIsActive(Diversifier _d_toCheck)
    {
        for (int i = 0; i < dA_activeDivers.Length; i++)
            if (dA_activeDivers[i] == _d_toCheck)
                return true;

        return false;
    }

    public void ApplyDiversifiers(LodeSpawnZone[] _ziA_spawnableZones)
    {
        if (PhotonNetwork.IsMasterClient)
            view.RPC(nameof(SyncBonusObjective), RpcTarget.All, Random.Range(1, 7));

        ziA_allZones = _ziA_spawnableZones;

        if (PhotonNetwork.IsMasterClient)
            for (int i = 0; i < dA_activeDivers.Length; i++)
            {
                switch (dA_activeDivers[i])
                {
                    case Diversifier.GeysersGalore:
                        view.RPC(nameof(GeysersGaloreRPC), RpcTarget.All, Random.Range(0, 9999999));
                        break;

                }
            }
    }

    internal DiversifierInfo ReturnActiveDiversifierDisplayInfo(int _i_activeDiverIndex)
    {
        return diA_diversifiers[(int)dA_activeDivers[_i_activeDiverIndex]];
    }

    #region Diver Functions

    [PunRPC]
    public IEnumerator GeysersGaloreRPC(int _i_seed)
    {
        yield return null;
        Random.InitState(_i_seed);
        //Debug.LogError("MY SEED IS " + _i_seed);

        RaycastHit _hit;

        for (int i = 0; i < Random.Range(v_numberOfGeysers.x, v_numberOfGeysers.y); i++)
        {
            Physics.Raycast(ReturnPositionWithinZone(ziA_allZones[Random.Range(0, ziA_allZones.Length)]), Vector3.down, out _hit, Mathf.Infinity, LayerMask.NameToLayer("UGG"), QueryTriggerInteraction.Ignore);
            if (_hit.transform.name.Contains("*")) continue;

            GameObject _go = PhotonNetwork.Instantiate(s_geyserPath, _hit.point, Quaternion.identity);
            _go.transform.up = _hit.normal;
            _go.name += "*";
        }
    }

    #endregion

    #region Utils

    private Vector3 ReturnPositionWithinZone(LodeSpawnZone _zi_zone)
    {
        return new Vector3(Random.Range(0, _zi_zone.f_zoneRadius * RandomiseToNegative()), 500, Random.Range(0, _zi_zone.f_zoneRadius * RandomiseToNegative())) + _zi_zone.transform.position;
    }

    private float RandomiseToNegative()
    {
        return Random.Range(-1f, 1f);
    }

    #endregion

    #region Enemy Scaler Returns

    internal float ReturnGrooberScaler()
    {
        float _f_scaler = 1;
        for (int i = 0; i < dA_activeDivers.Length; i++)
            switch (dA_activeDivers[i])
            {
                case Diversifier.GoofyGroobers:
                    _f_scaler *= EnemyShrink;
                    break;
                case Diversifier.NastyGroobers:
                    _f_scaler *= EnemyGrow;
                    break;
            }

        return _f_scaler;
    }

    internal float ReturnHandymanScaler()
    {
        float _f_scaler = 1;
        for (int i = 0; i < dA_activeDivers.Length; i++)
            switch (dA_activeDivers[i])
            {
                case Diversifier.MiniMiniboss:
                    _f_scaler *= EnemyShrink;
                    break;
                case Diversifier.MaxiBoss:
                    _f_scaler *= EnemyGrow;
                    break;
            }

        return _f_scaler;
    }

    internal float ReturnFlyingScaler()
    {
        float _f_scaler = 1;
        for (int i = 0; i < dA_activeDivers.Length; i++)
            switch (dA_activeDivers[i])
            {
                case Diversifier.MiniFlying:
                    _f_scaler *= EnemyShrink;
                    break;
                case Diversifier.MaxiFlying:
                    _f_scaler *= EnemyGrow;
                    break;
            }

        return _f_scaler;
    }

    internal float ReturnHopdogScaler()
    {
        float _f_scaler = 1;
        for (int i = 0; i < dA_activeDivers.Length; i++)
            switch (dA_activeDivers[i])
            {
                case Diversifier.MiniBunny:
                    _f_scaler *= EnemyShrink;
                    break;
                case Diversifier.MegaBunnies:
                    _f_scaler *= EnemyGrow;
                    break;
            }

        return _f_scaler;
    }

    #endregion

    #region Lode Things

    internal float ReturnLodeScaler()
    {
        float _f_scaler = 1;
        for (int i = 0; i < dA_activeDivers.Length; i++)
            switch (dA_activeDivers[i])
            {
                case Diversifier.BabyLodes:
                    _f_scaler *= f_babyLodes;
                    break;
                case Diversifier.JumboLodes:
                    _f_scaler *= f_jumboLodes;
                    break;
            }

        return _f_scaler;
    }

    internal float ReturnLodeAmountIncrease()
    {
        float _f_scaler = 1;
        for (int i = 0; i < dA_activeDivers.Length; i++)
            switch (dA_activeDivers[i])
            {
                case Diversifier.LessLodes:
                    _f_scaler *= f_lessLodes;
                    break;
                case Diversifier.LotsOLodes:
                    _f_scaler *= f_lotsoLodes;
                    break;
            }

        return _f_scaler;
    }

    #endregion

    internal float ReturnLavaScaler()
    {
        float _f_scaler = 1;
        for (int i = 0; i < dA_activeDivers.Length; i++)
            switch (dA_activeDivers[i])
            {
                case Diversifier.LethalLava:
                    _f_scaler += 1;
                    break;
            }

        return _f_scaler;
    }


}