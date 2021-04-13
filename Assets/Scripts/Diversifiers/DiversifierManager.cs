using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Diversifier
{
    None, //It implements itself

    JumboLodes, //Triggered in LodeSpawnZone, increases size by 50%
    LotsOLodes, //Triggered in LodeSpawnZone
    GeysersGalore, //Triggered in DiversiferManager
    MiniMiniboss, //Triggered in HandymanHealth
    MiniBunny, //Triggered in HopDogHealth
    GoofyGroobers,
    MiniFlying,

    BabyLodes, //Triggered in LodeSpawnZone, reduces size by 25%
    LessLodes, //Triggered in LodeSpawnZone
    LethalLava, //Triggered in KillBox
    ZeroGNuggs, //Triggered in NugGo in start
    Maxiboss, //Triggered in HandymanHealth
    MegaBunnies, //Triggered in HopDogHealth
    NastyGroobers,
    MaxiFlying,
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

    [Header("Geyser Things")]
    [SerializeField] private string s_geyserPath;
    [SerializeField] private Vector2 v_numberOfGeysers;


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
        print((BonusObjective)_i_objectiveIndex + " is the new bonus");
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
                    default:
                        break;

                    case Diversifier.GeysersGalore:
                        view.RPC(nameof(GeysersGaloreRPC), RpcTarget.All, Random.Range(0, 9999999));
                        break;

                }
            }
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
            if (_hit.transform.name.Contains("Mushroom")) continue;

            GameObject _go = PhotonNetwork.Instantiate(s_geyserPath, _hit.point, Quaternion.identity);
            _go.transform.up = _hit.normal;
        }
    }

    #endregion

    private Vector3 ReturnPositionWithinZone(LodeSpawnZone _zi_zone)
    {
        return new Vector3(Random.Range(0, _zi_zone.f_zoneRadius * RandomiseToNegative()), 500, Random.Range(0, _zi_zone.f_zoneRadius * RandomiseToNegative())) + _zi_zone.transform.position;
    }

    private float RandomiseToNegative()
    {
        return Random.Range(-1f, 1f);
    }
}
