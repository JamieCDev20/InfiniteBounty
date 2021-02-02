using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiversifierManager : MonoBehaviourPunCallbacks
{
    public static DiversifierManager x;
    private PhotonView view;
    private Diversifier[] dA_activeDivers = new Diversifier[3];

    [Header("Geyser Things")]
    [SerializeField] private string s_geyserPath;
    [SerializeField] private Vector2 v_numberOfGeysers;

    private void Awake()
    {
        transform.parent = null;
        if (x != null) Destroy(gameObject);
        else
        {
            x = this;
            DontDestroyOnLoad(gameObject);
        }

        view = GetComponent<PhotonView>();
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


    public void ApplyDiversifiers(ZoneInfo[] _ziA_spawnableZones)
    {
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
                        Vector3[] _vA_positions = new Vector3[Mathf.RoundToInt(Random.Range(v_numberOfGeysers.x, v_numberOfGeysers.y))];

                        for (int x = 0; x < _vA_positions.Length; x++)
                        {
                            int _i_zoneIndex = Random.Range(0, _ziA_spawnableZones.Length);
                            _vA_positions[x] = new Vector3(Random.Range(0, _ziA_spawnableZones[_i_zoneIndex].f_zoneRadius), 500, Random.Range(0, _ziA_spawnableZones[_i_zoneIndex].f_zoneRadius)) + _ziA_spawnableZones[_i_zoneIndex].t_zone.position;
                        }

                        view.RPC(nameof(GigaGeysersRPC), RpcTarget.All, _vA_positions);
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
    public void GigaGeysersRPC(Vector3[] _vA_pointsToPlaceGeysers)
    {
        RaycastHit _hit;

        for (int i = 0; i < _vA_pointsToPlaceGeysers.Length; i++)
        {
            Physics.Raycast(_vA_pointsToPlaceGeysers[i], Vector3.down, out _hit, Mathf.Infinity);
            GameObject _go = PhotonNetwork.Instantiate(s_geyserPath, _hit.point, Quaternion.identity);
            _go.transform.up = _hit.normal;

        }
    }

    #endregion

}
