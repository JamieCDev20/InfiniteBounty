using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FlyingAI : AIBase
{

    [SerializeField] private float f_orbitRange;
    private float f_shootTimer;
    [SerializeField] private float f_shootCooldown;

    #region Queries

    private bool IsInOrbitRangeQuery()
    {
        if (Vector3.SqrMagnitude(t_target.position - transform.position) < (f_orbitRange * f_orbitRange))
            return true;
        return false;
    }

    private bool CanshootQuery()
    {
        if (f_shootTimer <= 0)
            return true;
        return false;
    }

    #endregion

    #region Check Actions

    #endregion

    #region Actions

    private void Move()
    {
        
    }

    private void Shoot()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        Vector3 _dir = t_target.position - transform.position;
        photonView.RPC(nameof(RemoteThrow), RpcTarget.AllViaServer, _dir);
        f_shootTimer = f_shootCooldown;
    }

    [PunRPC]
    public void RemoteShoot(Vector3 dir)
    {
        GameObject ob = PoolManager.x?.SpawnObject(go_throwProjectile, transform.position, Quaternion.LookRotation(dir));
        ob.GetComponent<Rigidbody>().AddForce(ob.transform.forward.normalized * f_throwForce, ForceMode.Impulse);
    }


    #endregion

}