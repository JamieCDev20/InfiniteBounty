using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LodeSynchroniser : MonoBehaviourPunCallbacks
{

    public static LodeSynchroniser x;

    private List<LodeBase> lbL_allLodes = new List<LodeBase>();

    private void Awake()
    {
        x = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(SpawnLodes), RpcTarget.All, Random.Range(0, 999999));
    }

    #region Lode Spawning

    [PunRPC]
    private void SpawnLodes(int seed)
    {
        Random.InitState(seed);
        //yield return new WaitForEndOfFrame();

        foreach (LodeSpawnZone item in FindObjectsOfType<LodeSpawnZone>())
        {
            //yield return new WaitForEndOfFrame();
            lbL_allLodes.AddRange(item.SpawnLode());
        }

        for (int i = 0; i < lbL_allLodes.Count; i++)
            lbL_allLodes[i].SetID(i);
    }

    #endregion

    #region Taking Damage

    public void LodeTookDamage(int _i_lodeIndex, int _i_damage)
    {
        photonView.RPC(nameof(LodeDamageRPC), RpcTarget.All, _i_lodeIndex, lbL_allLodes[_i_lodeIndex].GetHealth() - _i_lodeIndex);
    }

    [PunRPC]
    public void LodeDamageRPC(int _i_lodeIndex, int _i_newHealth)
    {
        lbL_allLodes[_i_lodeIndex].SetHealth(_i_newHealth);
    }

    #endregion

    #region Nuggs

    internal void SpawnNuggsFromLode(int _i_id)
    {
        photonView.RPC(nameof(NuggBurstRPC), RpcTarget.All, _i_id, Random.Range(0, 999999));
    }

    [PunRPC]
    internal void NuggBurstRPC(int _i_id, int _i_seed)
    {
        lbL_allLodes[_i_id].SpawnNuggs(_i_seed);
    }


    internal void DestroyUpNugg(int lodeID, int nugid, bool collected)
    {
        photonView.RPC(nameof(NuggRPC), RpcTarget.All, lodeID, nugid, collected);
    }
    [PunRPC]
    internal void NuggRPC(int lodeID, int nugid, bool collected)
    {
        lbL_allLodes[lodeID].DestroyNug(nugid, collected);
    }

    #endregion

}
