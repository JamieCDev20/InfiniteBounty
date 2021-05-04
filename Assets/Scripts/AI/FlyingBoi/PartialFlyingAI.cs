using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FlyingAI : AIBase
{

    [SerializeField] private float f_orbitRange;
    private float f_shootTimer;
    [SerializeField] private float f_shootCooldown;
    [SerializeField] private float f_shootForce;
    [SerializeField] private float f_orbitSpeed;
    private bool b_isRightWinged;


    #region Queries

    private bool IsInOrbitRangeQuery()
    {
        if (Vector3.SqrMagnitude(Vector3.Scale(t_target.position - transform.position, Vector3.one - Vector3.up)) < (f_orbitRange * f_orbitRange))
            return true;
        return false;
    }

    private bool CanshootQuery()
    {
        f_shootTimer -= Time.deltaTime;
        if (f_shootTimer <= 0)
            return true;
        return false;
    }

    #endregion

    #region Check Actions

    #endregion

    #region Actions

    private GameObject GetClosestTaggedObjectAction(string _s_tag, bool _b_ignoreHieght)
    {
        float _f_distance = 1000000000;
        GameObject go_object = null;

        foreach (GameObject item in TagManager.x.GetTagSet(_s_tag))
        {
            if (Mathf.Abs(item.transform.position.y - transform.position.y) > 10 && !_b_ignoreHieght)
                continue;
            float _f_distanceCheck = Vector3.SqrMagnitude(item.transform.position - transform.position);
            if (_f_distanceCheck < _f_distance)
            {
                go_object = item;
                _f_distance = _f_distanceCheck;
            }
        }
        return go_object;
    }

    private void GetClosestPlayerAction()
    {
        t_target = GetClosestTaggedObjectAction("Player", true).transform;
    }

    private void MoveTowardtarget()
    {
        if (t_target != null)
            mover.Move(((t_target.position + (Vector3.up * 10)) - transform.position));
        else
        {
            Vector3 pos = Vector3.zero;
            int c = 0;

            foreach (GameObject p in TagManager.x.GetTagSet("Player"))
            {
                pos += p.transform.position;
                c++;
            }
            pos /= c;

            mover.Move(((pos + (Vector3.up * 10)) - transform.position));
        }
    }

    private void OrbitTarget()
    {
        Vector3 _v_lookAt = new Vector3(t_target.position.x, transform.position.y, t_target.position.y);
        transform.LookAt(_v_lookAt);

        mover.Move(((b_isRightWinged ? transform.right : -transform.right) * f_orbitSpeed));
    }

    private void Shoot()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        print("SHOOTING!");
        Vector3 _dir = t_target.position - transform.position;
        photonView.RPC(nameof(RemoteShoot), RpcTarget.AllViaServer, _dir);
        f_shootTimer = f_shootCooldown;
    }

    [PunRPC]
    public void RemoteShoot(Vector3 dir)
    {

    }

    IEnumerator ShootingRoutine(Vector3 dir)
    {
        GameObject ob = PoolManager.x?.SpawnObject(go_throwProjectile, transform.position, Quaternion.LookRotation(dir));

        Collider c = ob.GetComponent<Collider>();
        c.enabled = false;
        ob.transform.rotation = Quaternion.LookRotation(dir);
        ob.GetComponent<Rigidbody>().AddForce(ob.transform.forward.normalized * f_shootForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f);
        
        c.enabled = true;
    }

    #endregion

}