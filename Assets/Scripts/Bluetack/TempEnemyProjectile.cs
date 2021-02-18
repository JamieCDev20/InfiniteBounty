using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyProjectile : MonoBehaviour
{

    [SerializeField] private int i_damage = 15;

    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            IHitable ih = collision.gameObject.GetComponent<IHitable>();
            if (ih != null)
                ih.TakeDamage(i_damage, false);
        }
        PoolManager.x?.ReturnObjectToPool(gameObject);
    }

}
