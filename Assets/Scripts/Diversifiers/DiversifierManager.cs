using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiversifierManager : MonoBehaviourPunCallbacks
{
    public static DiversifierManager x;
    private PhotonView view;
    private Diversifier[] dA_activeDivers = new Diversifier[3];
    private LodeSpawnZone[] ziA_allZones;

    [Header("Geyser Things")]
    [SerializeField] private string s_geyserPath;
    [SerializeField] private Vector2 v_numberOfGeysers;

    private void Awake()
    {
    }

    public void Init()
    {

        transform.parent = null;
        if (x != null) Destroy(gameObject);
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

    public Diversifier[] ReturnActiveDivers()
    {
        return dA_activeDivers;
    }


    public void ApplyDiversifiers(LodeSpawnZone[] _ziA_spawnableZones)
    {
        ziA_allZones = _ziA_spawnableZones;

        if (PhotonNetwork.IsMasterClient)
            for (int i = 0; i < dA_activeDivers.Length; i++)
            {
                switch (dA_activeDivers[i])
                {
                    case Diversifier.None: break;

                    case Diversifier.JackedRabbits:
                        print("THERE ARE JACKED RABBITS");
                        break;

                    case Diversifier.GigaGeysers:
                        view.RPC(nameof(GigaGeysersRPC), RpcTarget.All, Random.Range(0, 9999999));
                        break;

                    case Diversifier.SolarStorm:
                        print("THE SUN IS A DEADLY LASER");
                        break;

                    default: break;
                }
            }
    }

    #region Diver Functions

    [PunRPC]
    public void GigaGeysersRPC(int _i_seed)
    {
        Random.InitState(_i_seed);

        RaycastHit _hit;

        for (int i = 0; i < Random.Range(v_numberOfGeysers.x, v_numberOfGeysers.y); i++)
        {
            Physics.Raycast(ReturnPositionWithinZone(ziA_allZones[Random.Range(0, ziA_allZones.Length)]), Vector3.down, out _hit, Mathf.Infinity);
            if (_hit.transform.name.Contains("Mushroom")) return;

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
